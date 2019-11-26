using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;

namespace SCHCWebAPI.Controllers
{
    /// <summary>
    /// 材料接口
    /// </summary>
    public class OrderController : BaseController
    {
        private readonly CatBatchCheckDocImp catBatchCheckDocImp = new CatBatchCheckDocImp();
        private readonly CatDocImp catDocImp = new CatDocImp();
        private readonly OrderImp orderImp = new OrderImp();
        private readonly ImageImp imageImp = new ImageImp();
        private readonly UserImp userImp = new UserImp();
        private readonly string fileurl = "";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapper"></param>
        /// <param name="redisCacheManager"></param>
        /// <param name="loggerHelper"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public OrderController(IMapper mapper, IRedisCacheManager redisCacheManager, ILoggerHelper loggerHelper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(mapper, redisCacheManager, loggerHelper, httpContextAccessor, configuration)
        {
            var audienceConfig = BaseConfiguration.GetSection("AppSettings");
            fileurl = audienceConfig["FileServer"].ToString();
        }
        #region 获取记录
        /// <summary>
        /// 获取药检单记录记录
        /// </summary>
        /// <param name="CatName">品种检索</param>
        /// <param name="opdatebegin">开始时间</param>
        /// <param name="batchno">批号</param>
        /// <param name="pageindex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetCatBatchCheckDoc(string CatName, string opdatebegin, string batchno, int pageindex)
        {
            var result = ResponseResult.Default();
            try
            {
                var s = await catBatchCheckDocImp.GetCatBatchCheckDoc(OperID, CatName, opdatebegin, batchno, pageindex);
                result = ResponseResult.Success(s);
                result.totalRowsCount = await catBatchCheckDocImp.GetCatBatchCount(OperID, CatName, opdatebegin, batchno);
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取品种材料记录
        /// </summary>
        /// <param name="CatName">品种检索</param>
        /// <param name="pageindex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetCatDoc(string CatName, int pageindex)
        {
            var result = ResponseResult.Default();
            try
            {
                var s = await catDocImp.GetCatDoc(OperID, CatName, pageindex);
                result = ResponseResult.Success(s);
                result.totalRowsCount = await catDocImp.GetCatDocCount(OperID, CatName);
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 获取两票制订单
        /// </summary>
        /// <param name="CatName">品种检索</param>
        /// <param name="batchno">批号检索</param>
        /// <param name="Sal_ID">订单检索</param>
        /// <param name="begindate">开始时间</param>
        /// <param name="endate">结束时间</param>
        /// <param name="pageindex">页码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> GetInvOrderInfo(string CatName, string batchno, string Sal_ID, string begindate, string endate, int pageindex)
        {
            var result = ResponseResult.Default();
            try
            {
                var s = await orderImp.GetOrderInfo(OperID, CatName, batchno, Sal_ID, begindate, endate, pageindex);
                result = ResponseResult.Success(s);
                result.totalRowsCount = await orderImp.GetOrderCount(OperID, CatName, batchno, Sal_ID, begindate, endate);
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }
        #endregion

        #region 预览图片
        /// <summary>
        /// 预览品种,批号对应的药检单路径明细
        /// </summary>
        /// <param name="CatBatchStr">110@77^88@99</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> ViewCatBatchDetl(string CatBatchStr)
        {
            var result = ResponseResult.Default();
            try
            {
                var s = await imageImp.GetCatBatchDetl(fileurl, CatBatchStr);
                result = ResponseResult.Success(s);
                result.totalRowsCount = s.Count;
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }
        /// <summary>
        /// 预览品种对应的材料路径明细
        /// </summary>
        /// <param name="Cat_IDStr"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> ViewCatDocDetl(string Cat_IDStr)
        {
            var result = ResponseResult.Default();
            try
            {
                var s = await imageImp.GetCatDocDetl(fileurl, Cat_IDStr);
                result = ResponseResult.Success(s);
                result.totalRowsCount = s.Count;
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }

        /// <summary>
        /// 预览对应销售明细单号的材料路径明细
        /// </summary>
        /// <param name="Sal_DIDStr">明细单号字符串,如:1832@24323</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<object> ViewInvOrderDocDetl(string Sal_DIDStr)
        {
            var result = ResponseResult.Default();
            try
            {
                BaseLoggerHelper.Info("url", fileurl);
                var s = await imageImp.GetInvOrderDocDetl(fileurl, Sal_DIDStr);
                result = ResponseResult.Success(s);
                result.totalRowsCount = s.Count;
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }
        #endregion

        #region 批量下载图片
        /// <summary>
        /// 批量下载品种,批号药检单材料
        /// </summary>
        /// <param name="CatBatchStr"></param>
        [HttpPost]
        public async Task<byte[]> DownCatBatchDetl(string CatBatchStr)
        {
            List<ImageUrl> s = await imageImp.GetCatBatchDetl(fileurl, CatBatchStr);
            if (s.Count > 0)
            {
                byte[] result = new FileHelper().CreateZipFileByte(s, "CatBatch");
                return result;
            }
            else
                return null;
        }

        /// <summary>
        /// 批量下载品种材料
        /// </summary>
        /// <param name="Cat_IDStr"></param>
        [HttpPost]
        public async Task<byte[]> DownCatDoc(string Cat_IDStr)
        {
            List<ImageUrl> s = await imageImp.GetCatDocDetl(fileurl, Cat_IDStr);
            if (s.Count > 0)
            {
                byte[] result = new FileHelper().CreateZipFileByte(s, "Cat");
                return result;
            }
            else
                return null;
        }
        /// <summary>
        /// 批量下载两票制材料
        /// </summary>
        /// <param name="Sal_DIDStr"></param>
        [HttpPost]
        public async Task<byte[]> DownInvOrderDoc(string Sal_DIDStr)
        {
            List<ImageUrl> s = await imageImp.GetInvOrderDocDetl(fileurl, Sal_DIDStr);
            if (s.Count > 0)
            {
                byte[] result = new FileHelper().CreateZipFileByte(s, "SalOrder");
                return result;
            }
            else
                return null;
        }
        #endregion

        #region 获取用户信息
        [HttpGet]
        public async Task<object> getUserInfo()
        {

            var result = ResponseResult.Default();
            try
            {
                result = ResponseResult.Success(user);
                result.totalRowsCount = 1;
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }
        #endregion

        #region 退出登录
        /// <summary>
        /// 退出登陆
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Logout()
        {
            var result = ResponseResult.Default();
            try
            {
                if (BaseRedisCacheManager.Get("userToken:" + Token))
                {
                    BaseRedisCacheManager.Remove("userToken:" + Token);
                }

                result = ResponseResult.Success();
                result.totalRowsCount = 1;
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="Pwd"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> ChangePasswd([Required] string Pwd)
        {
            var result = ResponseResult.Default();
            try
            {
                var u = await userImp.ChangePwd(user.UserID, Pwd, user.LoginType);
                if (u)
                {
                    if (BaseRedisCacheManager.Get("userToken:" + Token))
                    {
                        BaseRedisCacheManager.Remove("userToken:" + Token);
                    }
                    result = ResponseResult.Success();
                    result.totalRowsCount = 1;
                }
                else
                {
                    result = ResponseResult.Error();
                    result.totalRowsCount = 1;
                }
            }
            catch (Exception ex)
            {
                result = ResponseResult.Error(ex.Message);
            }
            return result;
        }

    }
}
