using System;
using System.Collections.Generic;
using System.Text;

namespace SCHCWebAPI
{
    ///状态码
    public enum APIStatusCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 重新登陆
        /// </summary>
        NeedRelogin = 0,
        /// <summary>
        /// Token验证失败
        /// </summary>
        Failed = -1,
        /// <summary>
        /// 需要更新
        /// </summary>
        NeedAppUpdate = -2,
        /// <summary>
        /// 未知码
        /// </summary>
        Unknown = 2
    }
}
