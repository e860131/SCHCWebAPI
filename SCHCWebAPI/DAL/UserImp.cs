using Newtonsoft.Json;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;

namespace SCHCWebAPI
{
    public class UserImp : BaseRepository<User>
    {
        /// <summary>
        /// 登陆验证
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="Platform">平台</param>
        /// <returns></returns>
        public async Task<User> Login(string username, string password, string Platform)
        {
            var r = await Task.Run(() =>
            Context.Db.Ado.SqlQuery<User>(
              @"    select top 1 UserID=Cst_ID,UsreName=Cst_Name,LoginType=2 from Bas_Customer where Cst_Logon=@account and Cst_Password=@password
              union select top 1 UserID=GdP_ID,UsreName=GdP_Name,LoginType=1 from Bas_GdsProvider where GdP_Logon=@account and GdP_Password=@password
              union select top 1 UserID=Psn_ID,UsreName=Psn_Name,LoginType=0 from Bas_Personnel where Psn_Logon=@account and Psn_Password=@password",
              new { account = username, password = password }));
            return r.FirstOrDefault();
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="Cst_ID">系统编码</param>
        /// <param name="password">密码</param>
        /// <param name="LoginType">类型(0,1,2)</param>
        /// <returns></returns>
        public async Task<bool> ChangePwd(string Cst_ID, string password, int LoginType)
        {
            if (LoginType == 0)
            {
                var r = await Task.Run(() => Context.Db.Ado.SqlQuery<User>(" update Bas_Personnel set Psn_Password=@password where Psn_ID=@Cst_ID; select top 1 UserID=Psn_ID,UsreName=Psn_Name from Bas_Personnel where Psn_ID=@Cst_ID ", new { Cst_ID = Cst_ID, password = password }));
                return r.Count > 0;
            }
            else if (LoginType == 1)
            {
                var r = await Task.Run(() => Context.Db.Ado.SqlQuery<User>(" update Bas_GdsProvider set GdP_Password=@password where GdP_ID=@Cst_ID; select top 1 UserID=GdP_ID,UsreName=GdP_Name from Bas_GdsProvider where GdP_ID=@Cst_ID ", new { Cst_ID = Cst_ID, password = password }));
                return r.Count > 0;
            }
            else
            {
                var r = await Task.Run(() => Context.Db.Ado.SqlQuery<User>(" update Bas_Customer set Cst_Password=@password where Cst_ID=@Cst_ID; select top 1 UserID=Cst_ID,UsreName=Cst_Name from Bas_Customer where Cst_ID=@Cst_ID ", new { Cst_ID = Cst_ID, password = password }));
                return r.Count > 0;
            }
        }
    }
}
