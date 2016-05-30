using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Zhyj.DBUtils;
using Zhyj.ICached;
namespace JiaQin.Data
{

    public class IDataExecutorImp
    {
        /// <summary>
        /// 数据库 EducationalAdmin 执行类
        /// </summary>
        public IDBExecutor Executor
        {
            get;
            set;
        }
        
        /// <summary>
        /// 当前用户
        /// </summary>
       // public Edu.XiaoXin.Entity.User CurrentUser { set; get; }
        /// <summary>
        /// 当前用户所属学校的数据缓存实例
        /// </summary>
        public ICached DataCached
        {
            set;
            get;
        }

        
        /// <summary>
        /// 获取类的实例，并且赋予 IDataExecutor 的实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetInstance<T>(bool flag = true) where T : new()
        {
            T instance = new T();
            if (instance is IDataExecutorImp)
            {
                IDataExecutorImp data = instance as IDataExecutorImp;
                if (Executor != null && flag)
                {
                    data.Executor = new MsSqlExecutor();
                    data.Executor.setConnnectionStr(Executor.DbStr);
                }
                else
                {
                    data.Executor = Executor; ;
                }
                data.DataCached = DataCached;   
                //data.CurrentUser = CurrentUser;
            }
            return instance;
        }


        public string GetCacheKey(Type[] keyType, object[] key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object item in key)
            {
                sb.AppendFormat(" {0}", item == null ? string.Empty : item);
            }

            StringBuilder sb2 = new StringBuilder();
            foreach (Type item in keyType)
            {
                sb2.AppendFormat("{0} ", item.FullName);
            }
            return GetCacheKey(sb2, sb.ToString());

        }

        public string GetCacheKey(Type keyType, object[] key)
        {
            StringBuilder sb = new StringBuilder();
            foreach (object item in key)
            {
                sb.AppendFormat(" {0}", item == null ? string.Empty : item);
            }
            return GetCacheKey(new StringBuilder(keyType.FullName), sb.ToString());
        }
        public string GetCacheKey(Type keyType)
        {
            return GetCacheKey(new StringBuilder(keyType.FullName), string.Empty);

        }
        /// <summary>
        /// 根据 Type与key 生成缓存键
        /// </summary>
        /// <param name="keyType">Type类型， typeOf(object)</param>
        /// <param name="key">特定的key</param>
        /// <returns></returns>
        public string GetCacheKey(Type keyType, object key)
        {

            return GetCacheKey(new StringBuilder(keyType.FullName), key);

        }
        /// <summary>
        /// 根据 Type与key 生成缓存键
        /// </summary>
        /// <param name="keyType">Type类型集合， new Type[]{typeOf(object)}</param>
        /// <param name="key">特定的key</param>
        /// <returns></returns>
        public string GetCacheKey(Type[] keyType, object key)
        {
            key = key == null ? string.Empty : key;
            StringBuilder sb = new StringBuilder();
            foreach (Type item in keyType)
            {
                sb.AppendFormat("{0} ", item.FullName);
            }
            return GetCacheKey(sb, key);
        }
        private string GetCacheKey(StringBuilder sb, object key)
        {
            StackTrace st = new StackTrace();
            StackFrame frame = st.GetFrame(2);
            MethodBase method = frame.GetMethod();
            ParameterInfo[] pars = method.GetParameters();
            StringBuilder sb2 = new StringBuilder(method.Name);
            sb2.Append("[");
            foreach (ParameterInfo item in pars)
            {

                sb2.Append(item.ParameterType.FullName);
                sb2.Append(" ");
                sb2.Append(item.Name);
                sb2.Append(",");
            }
            sb2.Append("]");

            return sb2.AppendFormat("  {0} - {1}", sb.ToString(), (key+"").ToString()).ToString();

        }
        /// <summary>
        /// 根据 Type 移除缓存
        /// </summary>
        /// <param name="keyType">Type类型， typeOf(object)</param>
        public void removeCache(Type keyType)
        {
            string key = string.Format("{0}", keyType.FullName);
            List<KeyValuePair<string, object>> list = DataCached.Items(key);
            foreach (KeyValuePair<string, object> item in list)
            {
                DataCached.Remove(item.Key);
            }
        }

        /// <summary>
        /// 根据 Type组合 移除缓存
        /// </summary>
        /// <param name="keyType"> Type类型集合， new Type[]{typeOf(object)}</param>
        public void removeCache(Type[] keyType)
        {
            StringBuilder sb = new StringBuilder();
            foreach (Type item in keyType)
            {
                sb.AppendFormat("{0} ", item.FullName);
            }

            List<KeyValuePair<string, object>> list = DataCached.Items(sb.ToString());
            foreach (KeyValuePair<string, object> item in list)
            {
                DataCached.Remove(item.Key);
            }
        }
        /// <summary>
        /// 根据指定的Key移除缓存
        /// </summary>
        /// <param name="key"></param>
        public void removeCacheItem(object key)
        {
            List<KeyValuePair<string, object>> list = DataCached.Items(key.ToString());
            foreach (KeyValuePair<string, object> item in list)
            {
                DataCached.Remove(item.Key);
            }
        }


        

       

    }
}
