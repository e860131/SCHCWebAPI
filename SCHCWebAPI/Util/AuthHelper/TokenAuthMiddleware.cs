using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace SCHCWebAPI
{
  /// <summary>
  /// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
  /// </summary>
  public class TokenAuthMiddleware
  {
    private string ClassName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name;
    private readonly RequestDelegate _next;
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IConfiguration _configuration;
    private readonly ILoggerHelper _loggerHelper;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="next"></param>
    /// <param name="redisCacheManager"></param>
    /// <param name="configuration"></param>
    /// <param name="loggerHelper"></param>
    public TokenAuthMiddleware(RequestDelegate next, IRedisCacheManager redisCacheManager, IConfiguration configuration, ILoggerHelper loggerHelper)
    {
      _next = next;
      _redisCacheManager = redisCacheManager;
      _configuration = configuration;
      _loggerHelper = loggerHelper;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="httpContext"></param>
    /// <returns></returns>
    public Task Invoke(HttpContext httpContext)
    {
      //检测是否包含'Authorization'请求头，如果不包含返回context进行下一个中间件，用于访问不需要认证的API
      if (!httpContext.Request.Headers.ContainsKey("Authorization"))
        return _next(httpContext);
      else
      {
        var tokenHeader = httpContext.Request.Headers["Authorization"];
        try
        {
          tokenHeader = tokenHeader.ToString().Substring("Bearer ".Length).Trim();
          SignedUser result = null;
          try
          {
            if (_redisCacheManager.Get("userToken:" + tokenHeader))
            {
              result = _redisCacheManager.Get<SignedUser>("userToken:" + tokenHeader);
            }

          }
          catch (Exception e)
          {
          }

          if (result == null)
          {
            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(ResponseResult.Error(APIStatusCode.NeedRelogin, "token错误,请重新登陆!"), new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
          }
          if (result.validateTime <= DateTime.Now)
          {
            _redisCacheManager.Remove("userToken:" + tokenHeader);
            return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(ResponseResult.Error(APIStatusCode.NeedRelogin, "token过期,请重新登陆!"), new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
          }
          else
          {
            var audienceConfig = _configuration.GetSection("Audience");
            var ValidateTime = audienceConfig["ValidateTime"].ObjToInt();
            _loggerHelper.Info(audienceConfig["ValidateTime"], ClassName + "Invoke");
            //更新Token时间
            result.validateTime = DateTime.Now.AddSeconds(ValidateTime);
            _redisCacheManager.Set("userToken:" + tokenHeader, result, TimeSpan.FromSeconds(ValidateTime));
            //执行下一个中间件
            return _next(httpContext);
          }
        }
        catch
        {
          return httpContext.Response.WriteAsync(JsonConvert.SerializeObject(ResponseResult.Error("验证token出错!"), new JsonSerializerSettings() { ContractResolver = new CamelCasePropertyNamesContractResolver() }));
        }
      }
    }
  }

  /// <summary>
  /// Extension method used to add the middleware to the HTTP request pipeline.
  /// </summary>
  public static class TokenAuthMiddlewareExtensions
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseTokenAuthMiddleware(this IApplicationBuilder builder)
    {
      return builder.UseMiddleware<TokenAuthMiddleware>();
    }
  }
}
