
using Microsoft.Extensions.Configuration;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SCHCWebAPI
{
    public class BaseDBConfig
    {
        static string MasterConnectionWrite = Appsettings.app(new string[] { "AppSettings", "SqlServer", "MasterConnectionWrite" });//主数据库写获取连接字符串

        /// <summary>
        /// 主数据库连接字符串(写)
        /// </summary>
        public static string MasterWrite = MasterConnectionWrite;
    }
}
