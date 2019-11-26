using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SCHCWebAPI
{
    /// <summary>
    /// 
    /// </summary>
    public class Login
    {
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 身份令牌
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 有效期
        /// </summary>
        public double expires_in { get; set; }
        /// <summary>
        /// 身份令牌类型
        /// </summary>
        public string token_type { get; set; }
        /// <summary>
        /// 员工实体
        /// </summary>
        public User employee { get; set; }

    }
}
