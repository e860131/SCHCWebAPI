using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AuthorizePolicy.JWT;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using AutoMapper;
using Castle.Core.Logging;
using log4net;
using log4net.Config;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using static SCHCWebAPI.CustomApiVersion;

namespace SCHCWebAPI
{
  /// <summary>
  /// 
  /// </summary>
  public class Startup
  {
    /// <summary>
    /// log4net 仓储库
    /// </summary>
    public static ILoggerRepository repository { get; set; }
    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="configuration"></param>
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
      //log4net
      repository = LogManager.CreateRepository("SCHCWebAPI");
      //指定配置文件，如果这里你遇到问题，应该是使用了InProcess模式，请查看Blog.Core.csproj,并删之
      XmlConfigurator.Configure(repository, new FileInfo("log4net.config"));
    }
    /// <summary>
    /// 配置接口
    /// </summary>
    public IConfiguration Configuration { get; }
    private const string ApiName = "华诚医药";

    /// <summary>
    /// This method gets called by the runtime. Use this method to add services to the container.
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public IServiceProvider ConfigureServices(IServiceCollection services)
    {

      #region HttpContext
      services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      #endregion



      #region 部分服务注入-netcore自带方法
      //缓存注入
      services.AddScoped<ICaching, MemoryCaching>();
      services.AddSingleton<IMemoryCache>(factory =>
      {
        var cache = new MemoryCache(new MemoryCacheOptions());
        return cache;
      });
      //Redis注入
      services.AddSingleton<IRedisCacheManager, RedisCacheManager>();
      #endregion

      //log日志注入
      services.AddSingleton<ILoggerHelper, LogHelper>();

      #region Automapper
      services.AddAutoMapper(typeof(Startup));
      #endregion

      #region CORS
      //跨域第二种方法，声明策略，记得下边app中配置
      services.AddCors(c =>
      {
        //↓↓↓↓↓↓↓注意正式环境不要使用这种全开放的处理↓↓↓↓↓↓↓↓↓↓
        c.AddPolicy("AllRequests", policy =>
        {
          policy
                      .AllowAnyOrigin()//允许任何源
                      .AllowAnyMethod()//允许任何方式
                      .AllowAnyHeader()//允许任何头
                      .AllowCredentials();//允许cookie
        });
        //↑↑↑↑↑↑↑注意正式环境不要使用这种全开放的处理↑↑↑↑↑↑↑↑↑↑


        // //一般采用这种方法
        // c.AddPolicy("LimitRequests", policy =>
        // {
        //     policy
        //     .WithOrigins("http://127.0.0.1:1818", "http://localhost:8080", "http://localhost:8021", "http://localhost:8081", "http://localhost:1818")//支持多个域名端口，注意端口号后不要带/斜杆：比如localhost:8000/，是错的
        //     .AllowAnyHeader()//Ensures that the policy allows any header.
        //     .AllowAnyMethod();
        // });
      });

      #endregion

      #region Swagger UI Service
      var basePath = Microsoft.DotNet.PlatformAbstractions.ApplicationEnvironment.ApplicationBasePath;
      services.AddSwaggerGen(c =>
      {
        ////遍历出全部的版本，做文档信息展示
        typeof(ApiVersions).GetEnumNames().ToList().ForEach(version =>
        {
          c.SwaggerDoc(version, new Info
          {
            // {ApiName} 定义成全局变量，方便修改
            Version = version,
            Title = $"{ApiName} 接口文档",
            Description = $"{ApiName} HTTP API " + version,
            TermsOfService = "None",
            Contact = new Contact { Name = "SCHCWebAPI", Email = "feng_xujian906@126.com" }
          });
          // 按相对路径排序，作者：Alby
          c.OrderActionsBy(o => o.RelativePath);
        });

        // 为 Swagger JSON and UI设置xml文档注释路径
        var xmlPath = Path.Combine(basePath, "SCHCWebAPI.xml");
        c.IncludeXmlComments(xmlPath);


        #region Token绑定到ConfigureServices

        c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
        {
          Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
          Name = "Authorization",
          In = "header"
        });
        #endregion

        c.DocumentFilter<SwaggerSecurityRequirementsDocumentFilter>();
      });

      #endregion

      #region MVC + GlobalExceptions
      //注入全局异常捕获
      services.AddMvc(o =>
      {
        // 全局异常过滤
        o.Filters.Add(typeof(GlobalExceptionsFilter));
      })
      .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
      // 取消默认驼峰
      .AddJsonOptions(options => { options.SerializerSettings.ContractResolver = new DefaultContractResolver(); });

      //统一返回
      services.Configure<ApiBehaviorOptions>(options =>
      {
        options.InvalidModelStateResponseFactory = (context) =>
              {
                var errors = context.ModelState.Values.SelectMany(x => x.Errors.Select(p => p.ErrorMessage)).ToList();

                var result = ResponseResult.Error(String.Join(",", errors));

                return new BadRequestObjectResult(result);
              };
      });

      #endregion

      #region
      //读取配置文件
      var audienceConfig = Configuration.GetSection("Audience");
      var symmetricKeyAsBase64 = audienceConfig["Secret"].ToString();
      var ValidateTime = audienceConfig["ValidateTime"].ObjToInt();

      var keyByteArray = Encoding.ASCII.GetBytes(symmetricKeyAsBase64);
      var signingKey = new SymmetricSecurityKey(keyByteArray);
      var tokenValidationParameters = new TokenValidationParameters
      {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = signingKey,
        ValidateIssuer = true,
        ValidIssuer = audienceConfig["Issuer"],//发行人
        ValidateAudience = true,
        ValidAudience = audienceConfig["Audience"],//订阅人
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        RequireExpirationTime = true,

      };
      var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);//签名验证
                                                                                                 ////这个集合模拟用户权限表,可从数据库中查询出来
      var permission = new List<Permission> {
                              new Permission {  Url="/", Name="admin"},
                              new Permission {  Url="/api/values", Name="admin"},
                              new Permission {  Url="/", Name="system"},
                              new Permission {  Url="/api/values1", Name="system"}
                          };
      //第一个参数:无权限action
      //如果第三个参数，是ClaimTypes.Role，上面集合的每个元素的Name为角色名称，如果ClaimTypes.Name，即上面集合的每个元素的Name为用户名,TimeSpan过期时间
      //
      var permissionRequirement = new PermissionRequirement(
           "/api/denied", permission,
           ClaimTypes.Role,
           audienceConfig["Issuer"],
           audienceConfig["Audience"],
           signingCredentials,
           expiration: TimeSpan.FromSeconds(ValidateTime)//身份有效时间4小时
           );
      services.AddAuthorization(options =>
      {
        options.AddPolicy("Permission", policy => policy.Requirements.Add(permissionRequirement));
      }).AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      })
      .AddJwtBearer(o =>
      {
        /*
         * OnMessageReceived:接收请求时调用
         * TokenValidated：在Token验证通过后调用。
         * AuthenticationFailed: 认证失败时调用。
         * Challenge: 未授权时调用。
         */
        //不使用https
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters = tokenValidationParameters;
        o.Events = new JwtBearerEvents
        {
          //自定义token获取方式
          //OnMessageReceived = context =>
          //{
          //    context.Token = context.Request.Query["access_token"];
          //    return Task.CompletedTask;
          //},

          //验证通过
          OnTokenValidated = context =>
          {
            if (context.Request.Path.Value.ToString() == "/api/logout")
            {
              var token = ((context as TokenValidatedContext).SecurityToken as JwtSecurityToken).RawData;
            }
            return Task.CompletedTask;
          },

          //验证失败
          //OnAuthenticationFailed = context =>
          //{
          //    return Task.CompletedTask;
          //}
        };
      });
      //注入授权Handler
      services.AddSingleton<IAuthorizationHandler, PermissionHandler>();
      services.AddSingleton(permissionRequirement);
      #endregion


      #region AutoFac DI
      //实例化 AutoFac  容器   
      var builder = new ContainerBuilder();
      //注册要通过反射创建的组件
      builder.RegisterType<CacheAOP>();//可以直接替换其他拦截器
      builder.RegisterType<RedisCacheAOP>();//可以直接替换其他拦截器
      builder.RegisterType<LogAOP>();//这样可以注入第二个

      // ※※★※※ 如果你是第一次下载项目，请先F6编译，然后再F5执行，※※★※※

      #region 带有接口层的服务注入
      //获取项目绝对路径，请注意，这个是实现类的dll文件，不是接口 IBusinessRepository.dll ，注入容器当然是Activatore
      try
      {
        // AOP 开关，如果想要打开指定的功能，只需要在 appsettigns.json 对应对应 true 就行。
        var cacheType = new List<Type>();
        if (Appsettings.app(new string[] { "AppSettings", "RedisCaching", "Enabled" }).ObjToBool())
        {
          cacheType.Add(typeof(RedisCacheAOP));
        }
        if (Appsettings.app(new string[] { "AppSettings", "MemoryCachingAOP", "Enabled" }).ObjToBool())
        {
          cacheType.Add(typeof(CacheAOP));
        }
        if (Appsettings.app(new string[] { "AppSettings", "LogoAOP", "Enabled" }).ObjToBool())
        {
          cacheType.Add(typeof(LogAOP));
        }

        builder.RegisterAssemblyTypes().AsImplementedInterfaces().InstancePerLifetimeScope()
                  .EnableInterfaceInterceptors()//引用Autofac.Extras.DynamicProxy;
                                                // 如果你想注入两个，就这么写  InterceptedBy(typeof(CacheAOP), typeof(LogAOP));
                                                // 如果想使用Redis缓存，请必须开启 redis 服务，端口号6319，如果不一样还是无效，否则请使用memory缓存 
                  .InterceptedBy(cacheType.ToArray());//允许将拦截器服务的列表分配给注册。 

      }
      catch (Exception ex)
      {
        throw new Exception(ex.Message);
      }
      #endregion



      //将services填充到Autofac容器生成器中
      builder.Populate(services);

      //使用已进行的组件登记创建新容器
      var ApplicationContainer = builder.Build();

      #endregion

      return new AutofacServiceProvider(ApplicationContainer);//第三方IOC接管 core内置DI容器
    }

    /// <summary>
    /// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="env"></param>
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {

      #region Environment
      if (env.IsDevelopment())
      {
        // 在开发环境中，使用异常页面，这样可以暴露错误堆栈信息，所以不要放在生产环境。
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        // 在非开发环境中，使用HTTP严格安全传输(or HSTS) 对于保护web安全是非常重要的。
        // 强制实施 HTTPS 在 ASP.NET Core，配合 app.UseHttpsRedirection
        //app.UseHsts();

      }
      #endregion

      #region Swagger
      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        //根据版本名称倒序 遍历展示
        typeof(ApiVersions).GetEnumNames().OrderByDescending(e => e).ToList().ForEach(version =>
        {
          c.SwaggerEndpoint($"/swagger/{version}/swagger.json", $"{ApiName} {version}");
        });
        // 将swagger首页，设置成我们自定义的页面，记得这个字符串的写法：解决方案名.index.html
        //c.IndexStream = () => GetType().GetTypeInfo().Assembly.GetManifestResourceStream("SCHCWebAPI.index.html");//这里是配合MiniProfiler进行性能监控的，《文章：完美基于AOP的接口性能分析》，如果你不需要，可以暂时先注释掉，不影响大局。
        c.RoutePrefix = ""; //路径配置，设置为空，表示直接在根域名（localhost:8001）访问该文件,注意localhost:8001/swagger是访问不到的，去launchSettings.json把launchUrl去掉
      });
      #endregion

      #region CORS
      //跨域第二种方法，使用策略，详细策略信息在ConfigureService中
      app.UseCors("AllRequests");//将 CORS 中间件添加到 web 应用程序管线中, 以允许跨域请求。
      #endregion

      // 使用静态文件
      app.UseStaticFiles();
      // 使用cookie
      app.UseCookiePolicy();
      // 返回错误码
      app.UseStatusCodePages();//把错误码返回前台，比如是404
                               //token验证中间件
      app.UseMiddleware<TokenAuthMiddleware>();

      app.UseMvc();
    }
  }
}
