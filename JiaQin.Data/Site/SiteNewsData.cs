
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using JiaQin.Entity;
using JiaQin.Entity.Lazy;
using JiaQin.Entity;
namespace JiaQin.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class SiteNewsData:IDataExecutorImp
    {
        public SiteNew[] List(int columnId, int pagesize, int newsId)
        {

            string key = GetCacheKey(new Type[]{
                            typeof(SiteNew)
                     }, new object[] { pagesize, newsId, columnId });

            SiteNewLazy[] list = DataCached.GetItem<SiteNewLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@columnId", columnId);
            list = Executor.executeForListObject<SiteNewLazy>("select top " + pagesize + " * from  siteNews where siteColumnId=@columnId " + (newsId > 0 ? " and id<" + newsId : string.Empty) + " order by id desc").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;
            return list;

        }
        public SiteNew [] GetSiteColumnDataList(int columnId, int? count = null, string orderby = "id desc", string where = null)
        {
            string key = GetCacheKey(typeof(SiteNew),new object[]{
                columnId,count,orderby,where
            });
            SiteNewLazy[] news = DataCached.GetItem<SiteNewLazy[]>(key);
            if (news!=null)
            {
                return news;
            }
            count = count == null ? 10 : count;
            SiteColumnData columnData = GetInstance<SiteColumnData>();
            SiteColumn column = columnData.getSiteColumnInfoById(columnId);

            news = Executor.executeForListObject<SiteNewLazy>("select top " + count + " * from siteNews where siteColumnId=" + columnId + " order by id desc").ToArray();
            Lazy(news);
            DataCached[key] = news;
            return news;
        }

        public SiteNew[] GetSiteColumnDataListWithPager(int columnId, int? count , string orderby , string where, int pagesize, int pagenum, out int rowcount)
        {
            string key = GetCacheKey(typeof(SiteNew), new object[]{
                columnId,count,orderby,where,pagesize,pagenum
            }); string keyRowcount = key + " rowcount";
            SiteNewLazy[] news = DataCached.GetItem<SiteNewLazy[]>(key);
            if (news != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return news;
            }
            count = count == null ? 10 : count;
            orderby = string.IsNullOrEmpty(orderby) ? "id desc" : orderby;
            SiteColumnData columnData = GetInstance<SiteColumnData>();
            SiteColumn column = columnData.getSiteColumnInfoById(columnId);

            news = Executor.executePage<SiteNewLazy>("*","select top " + count + " * from siteNews where siteColumnId=" + columnId ,orderby,where,pagesize,pagenum,out rowcount).ToArray();
            
            Lazy(news);
            DataCached[key] = news;
            DataCached[keyRowcount] = rowcount;
            return news;
        }
        public SiteNew[] List(int columnId,int pagesize, int pagenum, out int rowcount){
            string key = GetCacheKey(new Type[]{
                            typeof(SiteNew)
                     }, new object[] { pagesize, pagenum, columnId });
            string keyRowcount = key + " rowcount";
            SiteNewLazy[] list = DataCached.GetItem<SiteNewLazy[]>(key);
            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }
            Executor.addParameter("@columnId",columnId);
            list = Executor.executePage<SiteNewLazy>("*", "siteNews", "id desc", "siteColumnId=@columnId", pagesize, pagenum, out rowcount).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;
        }
        public SiteNew[] List(){
            string key = GetCacheKey( typeof(SiteNew));
            SiteNewLazy[] list = DataCached.GetItem<SiteNewLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<SiteNewLazy>( "select * from siteNews").ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;
        }
        void Lazy(SiteNewLazy[] obj)
        {
            foreach (SiteNewLazy item in obj)
            {
                Lazy(item);
            }
            
        }
        void Lazy(SiteNewLazy obj){
        obj.SiteColumnInfoLazy=new Func<int, SiteColumn>(GetInstance<SiteColumnData>().getSiteColumnInfoById);
        SysUserData userD = GetInstance<SysUserData>();
        obj.UserInfoLazy = new Func<int, SysUser>(userD.Info);
            
        }
        public SiteNew[] getSiteNewsListBySiteColumnId(int siteColumnId){  
          int id=siteColumnId;
          string key = GetCacheKey( typeof(SiteNew), id);
            SiteNewLazy[] list = DataCached.GetItem<SiteNewLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<SiteNewLazy>( "select * from siteNews where siteColumnId="+id).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;
        }

        public SiteNew getSiteNewsInfoByColumnId(int columnId) {
            string key = GetCacheKey(typeof(SiteNew), columnId);

            SiteNewLazy obj = DataCached.GetItem<SiteNewLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@columnId", columnId);
            obj = Executor.executeForSingleObject<SiteNewLazy>("select * from siteNews where siteColumnId=@columnId");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
            DataCached[key] = obj;
            return obj;
        }
    public SiteNew getSite_newsInfoById(int Id){
            string key = GetCacheKey(typeof(SiteNew), Id);
            SiteNewLazy obj = DataCached.GetItem<SiteNewLazy>(key);
            if (obj != null)
            {
                return obj;
            }
            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<SiteNewLazy>("select * from siteNews where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;
        }
        public void Add(SiteNew obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [siteNews]
           (
                UserId,
                Title,
                Content,
                publishDate,
                TypeId,
                visitCount,
                siteColumnId,
                blogImg,
                audit,
                auditDate,
                auditUserId
            )
     VALUES
   (
	            @UserId,
	            @Title,
	            @Content,
	            @publishDate,
	            @TypeId,
	            @visitCount,
	            @siteColumnId,
	            @blogImg,
	            @audit,
	            @auditDate,
	            @auditUserId
    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{
	        {"@UserId",obj.UserId},
	        {"@Title",obj.Title},
	        {"@Content",obj.Content},
	        {"@publishDate",obj.PublishDate},
	        {"@TypeId",obj.TypeId},
	        {"@visitCount",obj.VisitCount},
	        {"@siteColumnId",obj.SiteColumnId},
	        {"@blogImg",obj.BlogImg},
	        {"@audit",obj.Audit},
	        {"@auditDate",obj.AuditDate},
	        {"@auditUserId",obj.AuditVipUserId}
            }));
				obj.Id=identityValue;
            removeCache(typeof(SiteNew));
        }
        public void Update(SiteNew obj){
            Executor.executeNonQuery(@"update [siteNews] set 
                Title=@Title,
                Content=@Content,
                publishDate=@publishDate,
                blogImg=@blogImg
         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",obj.Id},
	        {"@Title",obj.Title},
	        {"@Content",obj.Content},
	        {"@publishDate",obj.PublishDate},
	        {"@blogImg",obj.BlogImg}
            });
            removeCache(typeof(SiteNew));
        }
        public void Delete(int id){
            Executor.executeNonQuery("delete siteNews where id=@id", System.Data.CommandType.Text, new object[,]{                 
                {"@id",id }
            });
            removeCache(typeof(SiteNew));            
        }
    }
}