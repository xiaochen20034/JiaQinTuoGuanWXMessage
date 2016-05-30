
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
    public class SiteColumnData:IDataExecutorImp
    {



        public SiteColumn[] List(string where,int pagesize, int pagenum, out int rowcount){

            string key = GetCacheKey(new Type[]{
                            typeof(SiteColumn)
                     }, new object[]{pagesize,pagenum,where});

            string keyRowcount = key + " rowcount";

            SiteColumnLazy[] list = DataCached.GetItem<SiteColumnLazy[]>(key);

            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }

            list = Executor.executePage<SiteColumnLazy>("*", "siteColumns", "id desc", where, pagesize, pagenum, out rowcount).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;

        }
        public SiteColumn[] List(int id) {
            return List(id,false);
        }
        public SiteColumn[] List(int id,bool all){

            string key = GetCacheKey( typeof(SiteColumn),new object[]{id,all});

            SiteColumnLazy[] list = DataCached.GetItem<SiteColumnLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            string where = "";
            if (!all)
            {
                where = " and showSys=1 ";
            }
            Executor.addParameter("@parentColumn",id);
            list = Executor.executeForListObject<SiteColumnLazy>("select * from siteColumns  where parentColumn=@parentColumn " + where).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        void Lazy(SiteColumnLazy obj){

           obj.SiteNewsListLazy=new Func<int, SiteNew[]>(GetInstance<SiteNewsData>().getSiteNewsListBySiteColumnId);
           obj.ParentColumnInfoLazy = new Func<int, SiteColumn>(getSiteColumnInfoById);
           obj.ChildrenColumnListLazy = new Func<int, SiteColumn[]>(List);
        }

        public SiteColumn getSiteColumnInfoByCode(string code) {


            string key = GetCacheKey(typeof(SiteColumn), code);

            SiteColumnLazy obj = DataCached.GetItem<SiteColumnLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@code", code);
            obj = Executor.executeForSingleObject<SiteColumnLazy>("select * from siteColumns where code=@code");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
            DataCached[key] = obj;
            return obj;
        }
    public SiteColumn getSiteColumnInfoById(int Id){

            string key = GetCacheKey(typeof(SiteColumn), Id);

            SiteColumnLazy obj = DataCached.GetItem<SiteColumnLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<SiteColumnLazy>("select * from siteColumns where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;

        }
        public void Add(SiteColumn obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [siteColumns]
           (

                name,

                code,

                parentColumn,

                fullParentColumn,

                sort,

                listTemplate,

                contentTemplate,

                mobileListTemplate,

                mobileContentTemplate,WebLink,SingleNews,showSite,showSys

            )
     VALUES
   (

	            @name,

	            @code,

	            @parentColumn,

	            @fullParentColumn,

	            @sort,

	            @listTemplate,

	            @contentTemplate,

	            @mobileListTemplate,

	            @mobileContentTemplate,@WebLink,@SingleNews,@showSite,@showSys

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@name",obj.Name},

	        {"@code",obj.Code},

	        {"@parentColumn",obj.ParentColumn},

	        {"@fullParentColumn",obj.FullParentColumn},

	        {"@sort",obj.Sort},

	        {"@listTemplate",obj.ListTemplate},

	        {"@contentTemplate",obj.ContentTemplate},

	        {"@mobileListTemplate",obj.MobileListTemplate},

	        {"@mobileContentTemplate",obj.MobileContentTemplate},
            {"@WebLink",obj.WebLink},{"@SingleNews",obj.SingleNews},
            {"@showSite",obj.ShowSite},{"@showSys",obj.ShowSys}
            }));

				obj.ID=identityValue;

            removeCache(typeof(SiteColumn));
        }

        public void Update(SiteColumn obj){
            Executor.executeNonQuery(@"update [siteColumns] set 

                name=@name,

                code=@code,

                parentColumn=@parentColumn,

                fullParentColumn=@fullParentColumn,

                sort=@sort,

                listTemplate=@listTemplate,

                contentTemplate=@contentTemplate,

                mobileListTemplate=@mobileListTemplate,

                mobileContentTemplate=@mobileContentTemplate,WebLink=@WebLink,SingleNews=@SingleNews,
showSite=@showSite,showSys=@showSys
         where ID=@ID", System.Data.CommandType.Text, new object[,]{
            {"@ID",obj.ID},

	        {"@name",obj.Name},

	        {"@code",obj.Code},

	        {"@parentColumn",obj.ParentColumn},

	        {"@fullParentColumn",obj.FullParentColumn},

	        {"@sort",obj.Sort},

	        {"@listTemplate",obj.ListTemplate},

	        {"@contentTemplate",obj.ContentTemplate},

	        {"@mobileListTemplate",obj.MobileListTemplate},

	        {"@mobileContentTemplate",obj.MobileContentTemplate},
            {"@WebLink",obj.WebLink},{"@SingleNews",obj.SingleNews},
            {"@showSite",obj.ShowSite},{"@showSys",obj.ShowSys}


            });
            removeCache(typeof(SiteColumn));
        }
        public void Delete(int ID){
            Executor.executeNonQuery("delete siteColumns where ID=@ID", System.Data.CommandType.Text, new object[,]{                 
                {"@ID",ID }
            });
            removeCache(typeof(SiteColumn));            
        }

    }
}