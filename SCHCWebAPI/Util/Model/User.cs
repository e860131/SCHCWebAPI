//=====================================================================================  
// All Rights Reserved , Copyright © weijian 2017  xujian
//=====================================================================================  

using System;

namespace SCHCWebAPI
{
    /// <summary>
    /// 员工、职员 
    /// </summary>
    public partial class User
    {
        /// <summary>
        /// 编码
        /// </summary>
        /// <value></value>
        public string UserID { get; set; }
        /// <summary>
        /// 名称
        /// </summary>		
        public string UsreName { get; set; }

        /// <summary>
        /// 登录类型(0:人员,1:供货人,2:客户)
        /// </summary>
        /// <value></value>
        public int LoginType { get; set; }

    }
}