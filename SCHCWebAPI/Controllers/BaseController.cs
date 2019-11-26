using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace SCHCWebAPI.Controllers
{
  /// <summary>
  /// 基础Controller
  /// </summary>
  [Route("api/[controller]/[action]")]
  [Produces("application/json")]
  [Authorize(Policy = "Permission")]//身份验证
  [ApiController]
  public class BaseController : Controller
  {
    private string _token = "";
    private readonly IRedisCacheManager _redisCacheManager;
    private readonly IMapper _mapper;
    private readonly ILoggerHelper _loggerHelper;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;
    private User _Employee;
    /// <summary>
    /// 
    /// </summary>
    public BaseController(IMapper mapper, IRedisCacheManager redisCacheManager, ILoggerHelper loggerHelper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base()
    {
      if (httpContextAccessor.HttpContext.Request.Headers.ContainsKey("Authorization"))
        _token = httpContextAccessor.HttpContext.Request.Headers["Authorization"].ToString().Substring("Bearer ".Length).Trim();
      this._mapper = mapper;
      this._redisCacheManager = redisCacheManager;
      this._loggerHelper = loggerHelper;
      this._httpContextAccessor = httpContextAccessor;
      this._configuration = configuration;
      if (!string.IsNullOrEmpty(_token))
        _Employee = GetCachEmploye(_token, this._redisCacheManager);
      else
      {
        _Employee = new User();
      }
    }

    /// <summary>
    /// 返回Token
    /// </summary>
    public String Token
    {
      get { return _token; }
    }
    /// <summary>
    /// 返回操作人
    /// </summary>
    public String Oper
    {
      get { return _Employee == null ? "" : _Employee.UsreName; }
    }
    /// <summary>
    /// 返回操作人编码
    /// </summary>
    public String OperID
    {
      get { return _Employee == null ? "" : _Employee.UserID; }
    }
    /// <summary>
    /// 返回缓存接口信息
    /// </summary>
    public IRedisCacheManager BaseRedisCacheManager
    {
      get { return _redisCacheManager; }
    }
    /// <summary>
    /// 返回AutoMap接口
    /// </summary>
    public IMapper BaseMapper
    {
      get { return _mapper; }
    }
    /// <summary>
    /// 配置文件接口
    /// </summary>
    /// <value></value>
    public IConfiguration BaseConfiguration
    {
      get { return _configuration; }
    }
    /// <summary>
    /// 返回日志接口
    /// </summary>
    public ILoggerHelper BaseLoggerHelper
    {
      get { return _loggerHelper; }
    }
    /// <summary>
    /// 获取HttpContext
    /// </summary>
    public HttpContext Current
    {
      get
      {
        return _httpContextAccessor.HttpContext;
      }
    }
    /// <summary>
    /// 获取访问的ip
    /// </summary>
    public string IP
    {
      get
      {
        return Current.Request.Host.ObjToString();
      }
    }

    /// <summary>
    /// 获取用户信息
    /// </summary>
    /// <value></value>
    public User user
    {
      get
      {
        return _Employee;
      }
    }
    /// <summary>
    /// 获取缓存中的人员实体
    /// </summary>
    /// <param name="token"></param>
    /// <param name="_redisCacheManager"></param>
    /// <returns></returns>
    private User GetCachEmploye(string token, IRedisCacheManager _redisCacheManager)
    {

      try
      {
        if (_redisCacheManager.Get("userToken:" + token))
        {
          SignedUser signedUser = _redisCacheManager.Get<SignedUser>("userToken:" + token);
          if (signedUser != null)
          {
            return signedUser.employee;
          }
          else
          {
            return null;
          }
        }
        else
          return null;
      }
      catch (Exception e)
      {
        return null;
      }
    }
  }
}
