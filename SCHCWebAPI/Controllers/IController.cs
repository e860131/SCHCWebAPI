using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WJAPI.Controllers
{
    /// <summary>
    /// 基础方法接口(新增,修改,删除,查询实体)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IController<T>
    {
        /// <summary>
        /// 新增记录
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<object> Insert([Required]T t);
        /// <summary>
        /// 修改记录
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        Task<object> Update([Required]T t);
        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<object> Delete([Required] string key);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<object> GetEntity([Required] string key);
    }
}
