
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using JiaQin.Entity;
using JiaQin.Entity.Lazy;
namespace JiaQin.Data
{

    /// <summary>
    /// 
    /// </summary>
    public class SysInfoData : IDataExecutorImp
    {


        public SysInfo getSysInfo()
        {

            string key = GetCacheKey(typeof(SysInfo));

            SysInfo obj = DataCached.GetItem<SysInfo>(key);

            if (obj != null)
            {
                return obj;
            }

            obj = Executor.executeForSingleObject<SysInfo>("select top 1 * from SysInfo ");
            if (obj == null)
            {
                return null;
            }
            return obj;

        }
        public void SetBasicInfo(SysInfo obj)
        {
            SysInfo obj2 = getSysInfo();
            if (obj2 == null)
            {
                int stuId = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [sysInfo]
           (

                Name,

                WeiXinAppID,

                WeiXinAppSecret,

                WeiXinAppToken,

                WeiXinEncodingAESKey,
                wxUserName

            )
     VALUES
   (

	            @Name,

	            @WeiXinAppID,

	            @WeiXinAppSecret,

	            @WeiXinAppToken,

	            @WeiXinEncodingAESKey,

	            @wxUserName

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@Name",obj.Name},

	        {"@WeiXinAppID",obj.WeiXinAppID},

	        {"@WeiXinAppSecret",obj.WeiXinAppSecret},

	        {"@WeiXinAppToken",obj.WeiXinAppToken},

	        {"@WeiXinEncodingAESKey",obj.WeiXinEncodingAESKey},

	        {"@wxUserName",obj.WxUserName}

            }));

            }
            else
            {
                obj.Id = obj2.Id;
                Executor.executeNonQuery(@"update [sysInfo] set 

                Name=@Name,

                WeiXinAppID=@WeiXinAppID,

                WeiXinAppSecret=@WeiXinAppSecret,

                WeiXinAppToken=@WeiXinAppToken,

                WeiXinEncodingAESKey=@WeiXinEncodingAESKey,

                wxUserName=@wxUserName

         where id=@id", System.Data.CommandType.Text, new object[,]{
                        {"@id",obj.Id},

	                    {"@Name",obj.Name},

	                    {"@WeiXinAppID",obj.WeiXinAppID},

	                    {"@WeiXinAppSecret",obj.WeiXinAppSecret},

	                    {"@WeiXinAppToken",obj.WeiXinAppToken},

	                    {"@WeiXinEncodingAESKey",obj.WeiXinEncodingAESKey},


	                    {"@wxUserName",obj.WxUserName}

            });

            }
            removeCache(typeof(SysInfo));
        }
        public void SetMchInfo(SysInfo obj)
        {
            SysInfo obj2 = getSysInfo();
            if (obj2 == null)
            {
                int stuId = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [sysInfo]
           (
WeiXinMchID,WeiXinMchKey
            )
     VALUES
   (
@WeiXinMchID,@WeiXinMchKey
);select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@WeiXinMchID",obj.WeiXinMchID},

	        {"@WeiXinMchKey",obj.WeiXinMchKey}

            }));

            }
            else
            {
                obj.Id = obj2.Id;
                Executor.executeNonQuery(@"update [sysInfo] set 

                WeiXinMchID=@WeiXinMchID,WeiXinMchKey=@WeiXinMchKey
         where id=@id", System.Data.CommandType.Text, new object[,]{
                        {"@id",obj.Id},

	                      {"@WeiXinMchID",obj.WeiXinMchID},

	                 {"@WeiXinMchKey",obj.WeiXinMchKey}

            });

            }
            removeCache(typeof(SysInfo));
        }

        public void Delete(SysInfo obj)
        {
            Executor.executeNonQuery("delete sysInfo where id=@id", System.Data.CommandType.Text, new object[,]{                 
                {"@id",obj.Id}
            });
            removeCache(typeof(SysInfo));
        }

    }
}