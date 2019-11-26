using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthorizePolicy.JWT;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using HCAPI;
using System.Linq;

namespace SCHCWebAPI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("api/Login")]
    public class LoginController : Controller
    {
        private string ClassName = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name;
        private readonly PermissionRequirement _requirement;
        private readonly IRedisCacheManager _redisCacheManager;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly ILoggerHelper _loggerHelper;
        private readonly UserImp userImp = new UserImp();

        /// <summary>
        /// 构造函数注入
        /// </summary>
        /// <param name="redisCacheManager"></param>
        /// <param name="_mapper"></param>
        /// <param name="contextAccessor"></param>
        /// <param name="loggerHelper"></param>
        /// <param name="requirement"></param>
        public LoginController(
            IRedisCacheManager redisCacheManager,
            IMapper _mapper,
            IHttpContextAccessor contextAccessor,
            ILoggerHelper loggerHelper,
            PermissionRequirement requirement
            )
        {
            this._requirement = requirement;
            this._redisCacheManager = redisCacheManager;
            this._mapper = _mapper;
            this._contextAccessor = contextAccessor;
            this._loggerHelper = loggerHelper;
        }

        /// <summary>
        /// 登陆认证
        /// </summary>
        /// <param name="account">用户名/手机号</param>
        /// <param name="password">密码/验证码</param>
        /// <param name="Platform">平台(web,android,ios,other)</param>
        /// <param name="appid">app编码</param>
        /// <param name="ismobile">手机号登录(1:是,0:否),默认0</param>
        /// <returns></returns>
        [HttpPost("/api/LoginAPI")]
        public async Task<object> LoginAPI([Required]string account, [Required]string password, [Required]string Platform, [Required] int ismobile = 0)
        {
            var employeeid = await userImp.Login(account, password, Platform);//验证登陆
            if (employeeid == null)
            {
                return ResponseResult.Error(MessageHelper.FailedToVerifyUsernamePassword);
            }

            #region Token
            //如果是基于用户的授权策略，这里要添加用户;如果是基于角色的授权策略，这里要添加角色
            Claim[] claims = new Claim[] { new Claim(ClaimTypes.Name, account), new Claim(ClaimTypes.Expiration, DateTime.Now.AddSeconds(_requirement.Expiration.TotalSeconds).ToString()) };
            //用户标识
            var identity = new ClaimsIdentity(JwtBearerDefaults.AuthenticationScheme);
            identity.AddClaims(claims);
            //登录
            dynamic token = JwtToken.BuildJwtToken(claims, _requirement);
            #endregion


            Login login = JsonConvert.DeserializeObject<Login>(JsonConvert.SerializeObject(token));
            login.employee = employeeid;


            _loggerHelper.Info(login.expires_in.ObjToString(), ClassName + "Login");

            //保存登陆用户
            SignedUser su = new SignedUser();
            su.userToken = login.access_token;
            su.UserID = employeeid.UserID;
            su.validateTime = DateTime.Now.AddSeconds(login.expires_in);
            su.employee = employeeid;
            su.LoginType = employeeid.LoginType;
            su.Platform = Platform;

            //RedisCaching 
            try
            {
                if (_redisCacheManager.Get<object>("userToken:" + su.userToken) != null)
                {
                    _redisCacheManager.Remove("userToken:" + su.userToken);
                }
            }
            catch (Exception e)
            {
            }
            //添加登陆用户信息
            _redisCacheManager.Set("userToken:" + su.userToken, su, TimeSpan.FromSeconds(login.expires_in));

            return new JsonResult(new
            {
                code = APIStatusCode.Success,
                message = MessageHelper.EMP_LOGIN + MessageHelper.SUCCESS,
                newID = employeeid.UserID,
                entity = login,

            });
        }

        /// <summary>
        /// 拒绝访问
        /// </summary>
        /// <returns></returns>
        [HttpGet("/api/Denied")]
        public IActionResult Denied()
        {
            return new JsonResult(new
            {
                Status = false,
                Message = "你无权限访问"
            });
        }
    }
}