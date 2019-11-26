using Newtonsoft.Json;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
    /// <summary>
    /// 响应结果类
    /// </summary>
    public class ResponseResult
    {
        /// <summary>
        /// 状态(0-default, 1-success, -1-error)
        /// </summary>
        public APIStatusCode code
        {
            get;
            set;
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        public string message
        {
            get;
            set;
        }
        /// <summary>
        /// 数据
        /// </summary>
        public object entity
        {
            get;
            set;
        }
        /// <summary>
        /// 总行数
        /// </summary>
        public int totalRowsCount
        {
            get;
            set;
        }
        /// <summary>
        /// 响应消息封装类
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseResult Default(string message = null)
        {
            var result = new ResponseResult();
            result.code = APIStatusCode.Unknown;
            result.message = message;

            return result;
        }
        /// <summary>
        /// 响应消息封装类
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ResponseResult Success( string message = null)
        {
            var result = new ResponseResult();
            result.code = APIStatusCode.Success;
            result.message = MessageHelper.OKMESSAGE + " " + message;

            return result;
        }
        ///响应消息封装类
        public static ResponseResult Success(object data, string message = null)
        {
            var result = new ResponseResult();
            result.code = APIStatusCode.Success;
            result.message = MessageHelper.OKMESSAGE + " " + message;
            result.entity = data;

            return result;
        }

        /// <summary>
        /// 响应消息封装类
        /// </summary>
        /// <returns></returns>
        public static ResponseResult Error(string errorMessage =null)
        {
            var result = new ResponseResult();
            result.code = APIStatusCode.Failed;
            result.message = MessageHelper.ERRORMESSAGE + " " + errorMessage;
            return result;
        }

        /// <summary>
        /// Http 响应消息封装类
        /// </summary>
        /// <param name="message"></param>
        /// <param name="SuccessCode"></param>
        /// <returns></returns>
        public static ResponseResult Error(APIStatusCode SuccessCode, string message = null)
        {
            var result = new ResponseResult();
            result.entity = null;
            result.code = SuccessCode;
            result.message = MessageHelper.ERRORMESSAGE + " " + message;

            return result;
        }
        ///
        public static ResponseResult Deserialize(string message)
        {
            var result = ServiceStack.Text.JsonSerializer.DeserializeFromString<ResponseResult>(message);
            return result;
        }
    }
}
