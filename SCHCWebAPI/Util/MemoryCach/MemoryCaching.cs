﻿using Microsoft.Extensions.Caching.Memory;
using System;

namespace SCHCWebAPI
{
    /// <summary>
    /// 实例化缓存接口ICaching
    /// </summary>
    public class MemoryCaching : ICaching
    {
        //引用Microsoft.Extensions.Caching.Memory;这个和.net 还是不一样，没有了Httpruntime了
        private readonly IMemoryCache _cache;
        //还是通过构造函数的方法，获取
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        public MemoryCaching(IMemoryCache cache)
        {
            _cache = cache;
        }
        ///获取缓存  
        public object Get(string cacheKey)
        {
            return _cache.Get(cacheKey);
        }
        ///设置缓存
        public void Set(string cacheKey, object cacheValue)
        {
            _cache.Set(cacheKey, cacheValue, TimeSpan.FromSeconds(7200));
        }
    }

}
