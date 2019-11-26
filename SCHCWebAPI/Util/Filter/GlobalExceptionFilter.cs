using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
  /// <summary>
  /// 全局异常错误日志
  /// </summary>
  public class GlobalExceptionsFilter : IExceptionFilter
  {
    private readonly IHostingEnvironment _env;
    private readonly ILoggerHelper _loggerHelper;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="env"></param>
    /// <param name="loggerHelper"></param>
    public GlobalExceptionsFilter(IHostingEnvironment env, ILoggerHelper loggerHelper)
    {
      _env = env;
      _loggerHelper = loggerHelper;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="context"></param>
    public void OnException(ExceptionContext context)
    {
      var json = new JsonErrorResponse();
      json.code = APIStatusCode.Failed;
      json.message = context.Exception.Message;//错误信息

      context.Result = new InternalServerErrorObjectResult(json);

      //采用log4net 进行错误日志记录
      _loggerHelper.Error(json.message, WriteLog(json.message, context.Exception));
    }

    /// <summary>
    /// 自定义返回格式
    /// </summary>
    /// <param name="throwMsg"></param>
    /// <param name="ex"></param>
    /// <returns></returns>
    public string WriteLog(string throwMsg, Exception ex)
    {
      return string.Format("【自定义错误】：{0} \r\n【异常类型】：{1} \r\n【异常信息】：{2} \r\n【堆栈调用】：{3}", new object[] { throwMsg,
                ex.GetType().Name, ex.Message, ex.StackTrace });
    }

  }
  /// <summary>
  /// 
  /// </summary>
  public class InternalServerErrorObjectResult : ObjectResult
  {
    ///
    public InternalServerErrorObjectResult(object value) : base(value)
    {
      StatusCode = StatusCodes.Status500InternalServerError;
    }
  }
  /// <summary>
  /// 
  /// </summary>
  //返回错误信息
  public class JsonErrorResponse
  {
    /// <summary>
    /// 代码
    /// </summary>
    /// <value></value>
    public APIStatusCode code { get; set; }
    /// <summary>
    /// 生产环境的消息
    /// </summary>
    public string message { get; set; }
  }

}
