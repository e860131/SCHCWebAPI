﻿using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
namespace SCHCWebAPI
{
    public class DbContext
    {

        private static string _connectionString;
        private static List<SlaveConnectionConfig> _readconnectionString;
        private static DbType _dbType;
        private SqlSugarClient _db;

        /// <summary>
        /// 连接字符串 
        /// 
        /// </summary>
        public static string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        public static List<SlaveConnectionConfig> ReadConnectionString
        {
            get { return _readconnectionString; }
            set { _readconnectionString = value; }
        }
        /// <summary>
        /// 数据库类型 
        ///  
        /// </summary>
        public static DbType DbType
        {
            get { return _dbType; }
            set { _dbType = value; }
        }
        /// <summary>
        /// 数据连接对象 
        ///  
        /// </summary>
        public SqlSugarClient Db
        {
            get { return _db; }
            private set { _db = value; }
        }

        /// <summary>
        /// 数据库上下文实例（自动关闭连接）
        ///  
        /// </summary>
        public static DbContext Context
        {
            get
            {
                return new DbContext();
            }

        }


        /// <summary>
        /// 功能描述:构造函数
        /// 作　　者:
        /// </summary>
        private DbContext()
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentNullException("数据库连接字符串为空");
            _db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = _dbType,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute,
                IsShardSameThread = true,
                SlaveConnectionConfigs= _readconnectionString,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //DataInfoCacheService = new HttpRuntimeCache()
                },
                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                },
            });
            _db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            {
                Console.WriteLine(sql);
            };
            _db.Aop.OnLogExecuting = (sql, pars) =>//SQL执行前事件
            {
                Console.WriteLine(_db.Ado.Connection.ConnectionString + "\r\n" + sql + "\r\n" + _db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();

            };
            _db.Aop.OnError = (exp) =>//执行SQL 错误事件
            {
                Console.WriteLine(exp.Parametres);
                //exp.sql exp.parameters 可以拿到参数和错误Sql             
            };
        }

        /// <summary>
        /// 功能描述:构造函数
        /// 作　　者:
        /// </summary>
        /// <param name="blnIsAutoCloseConnection">是否自动关闭连接</param>
        private DbContext(bool blnIsAutoCloseConnection)
        {
            if (string.IsNullOrEmpty(_connectionString))
                throw new ArgumentNullException("数据库连接字符串为空");
            _db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = _dbType,
                IsAutoCloseConnection = blnIsAutoCloseConnection,
                IsShardSameThread = true,
                InitKeyType = InitKeyType.Attribute,
                SlaveConnectionConfigs = _readconnectionString,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //DataInfoCacheService = new HttpRuntimeCache()
                },
                MoreSettings = new ConnMoreSettings()
                {
                    //IsWithNoLockQuery = true,
                    IsAutoRemoveDataCache = true
                }
            });

            _db.Aop.OnLogExecuted = (sql, pars) => //SQL执行完事件
            {
                Console.WriteLine(sql);
            };
            _db.Aop.OnLogExecuting = (sql, pars) =>//SQL执行前事件
            {
                Console.WriteLine(_db.Ado.Connection.ConnectionString + "\r\n" + sql + "\r\n" + _db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
            _db.Aop.OnError = (exp) =>//执行SQL 错误事件
            {
                Console.WriteLine(exp.Parametres);
                //exp.sql exp.parameters 可以拿到参数和错误Sql    
            };
        }

        #region 实例方法
        /// <summary>
        /// 功能描述:获取数据库处理对象
        /// 作　　者:
        /// </summary>
        /// <returns>返回值</returns>
        public SimpleClient<T> GetEntityDB<T>() where T : class, new()
        {
            return new SimpleClient<T>(_db);
        }
        /// <summary>
        /// 功能描述:获取数据库处理对象
        /// 作　　者:
        /// </summary>
        /// <param name="db">db</param>
        /// <returns>返回值</returns>
        public SimpleClient<T> GetEntityDB<T>(SqlSugarClient db) where T : class, new()
        {
            return new SimpleClient<T>(db);
        }

        #endregion

        #region 静态方法

        /// <summary>
        /// 功能描述:获得一个DbContext
        /// 作　　者:
        /// </summary>
        /// <param name="blnIsAutoCloseConnection">是否自动关闭连接（如果为false，则使用接受时需要手动关闭Db）</param>
        /// <returns>返回值</returns>
        public static DbContext GetDbContext(bool blnIsAutoCloseConnection = true)
        {
            return new DbContext(blnIsAutoCloseConnection);
        }

        /// <summary>
        /// 功能描述:设置初始化参数
        /// 作　　者:
        /// </summary>
        /// <param name="strConnectionString">连接字符串</param>
        /// <param name="enmDbType">数据库类型</param>
        public static void Init(string strConnectionString, List<SlaveConnectionConfig> readconnectionString, DbType enmDbType = SqlSugar.DbType.SqlServer)
        {
            _connectionString = strConnectionString;
            _readconnectionString = readconnectionString;
            _dbType = enmDbType;
        }

        /// <summary>
        /// 功能描述:创建一个链接配置
        /// 作　　者:
        /// </summary>
        /// <param name="blnIsAutoCloseConnection">是否自动关闭连接</param>
        /// <param name="blnIsShardSameThread">是否夸类事务</param>
        /// <returns>ConnectionConfig</returns>
        public static ConnectionConfig GetConnectionConfig(bool blnIsAutoCloseConnection = true, bool blnIsShardSameThread = false)
        {
            ConnectionConfig config = new ConnectionConfig()
            {
                ConnectionString = _connectionString,
                DbType = _dbType,
                IsAutoCloseConnection = blnIsAutoCloseConnection,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    //DataInfoCacheService = new HttpRuntimeCache()
                },
                IsShardSameThread = blnIsShardSameThread
            };
            return config;
        }

        /// <summary>
        /// 功能描述:获取一个自定义的DB
        /// 作　　者:
        /// </summary>
        /// <param name="config">config</param>
        /// <returns>返回值</returns>
        public static SqlSugarClient GetCustomDB(ConnectionConfig config)
        {
            return new SqlSugarClient(config);
        }
        /// <summary>
        /// 功能描述:获取一个自定义的数据库处理对象
        /// 作　　者:
        /// </summary>
        /// <param name="sugarClient">sugarClient</param>
        /// <returns>返回值</returns>
        public static SimpleClient<T> GetCustomEntityDB<T>(SqlSugarClient sugarClient) where T : class, new()
        {
            return new SimpleClient<T>(sugarClient);
        }
        /// <summary>
        /// 功能描述:获取一个自定义的数据库处理对象
        /// 作　　者:
        /// </summary>
        /// <param name="config">config</param>
        /// <returns>返回值</returns>
        public static SimpleClient<T> GetCustomEntityDB<T>(ConnectionConfig config) where T : class, new()
        {
            SqlSugarClient sugarClient = GetCustomDB(config);
            return GetCustomEntityDB<T>(sugarClient);
        }
        #endregion
    }
}
