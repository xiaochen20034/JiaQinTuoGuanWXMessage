using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiaQin.Entity;
namespace JiaQin.Data
{
    public class WxTemplateData : IDataExecutorImp
    {
        public WxTemplate InfoByCode(string code)
        {
            string key = GetCacheKey(typeof(WxTemplate), code);
            WxTemplate temp = DataCached.GetItem<WxTemplate>(key);
            //if (temp != null)
            //{
            //    return temp;
            //}
            Executor.addParameter("@code", code);
            temp = Executor.executeForSingleObject<WxTemplate>("select * from wx_template where templateCode=@code");
            DataCached[key] = temp;
            return temp;
        }
        public void Insert(WxTemplate template)
        {
            Executor.addParameter("@templateCode", template.TemplateCode);
            if (Convert.ToInt32(Executor.executeSclar("select count(1) from wx_template where templateCode=@templateCode"))>0)
            {
                return;
            }
            object o = Executor.executeSclar(@"insert into wx_template(title,trade,trade2,templateId,templateCode)values(@title,@trade,@trade2,@templateId,@templateCode);select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{
                {"@title",template.Title},
                {"@trade",template.Trade},
                {"@trade2",template.Trade2},
                {"@templateId",template.TemplateId},
                {"@templateCode",template.TemplateCode}
            });
            template.Id = Convert.ToInt32(o);
            removeCache(typeof(WxTemplate));
        }


    }
}
