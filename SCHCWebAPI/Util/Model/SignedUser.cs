using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
    /// <summary>
    /// 已经登录的用户
    /// </summary>
    public class SignedUser
    {
        /// <summary>
        /// jwt签名凭证
        /// </summary>
        public string userToken { get; set; }
        /// <summary>
        /// 用户唯一id名
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public DateTime? validateTime { get; set; }
        /// <summary>
        /// 用户信息
        /// </summary>
        public User employee { get; set; }
        /// <summary>
        /// 平台
        /// </summary>
        public string Platform { get; set; }
        /// <summary>
        /// 登录类型
        /// </summary>
        /// <value></value>
        public int LoginType { get; set; }
    }
}
