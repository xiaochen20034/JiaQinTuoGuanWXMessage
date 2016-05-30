
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
    public class TagData:IDataExecutorImp
    {
        public Tag[] List(string where,int pagesize, int pagenum, out int rowcount){
            string key = GetCacheKey(new Type[]{
                            typeof(Tag)
                     }, new object[]{pagesize,pagenum,where});
            string keyRowcount = key + " rowcount";
            TagLazy[] list = DataCached.GetItem<TagLazy[]>(key);
            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }
            list = Executor.executePage<TagLazy>("*", "tag", "id desc", where, pagesize, pagenum, out rowcount).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;
        }
        public Tag[] List(){
            string key = GetCacheKey( typeof(Tag));
            TagLazy[] list = DataCached.GetItem<TagLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<TagLazy>( "select * from tag").ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;
        }
        void Lazy(TagLazy obj){
        obj.SchoolInfoLazy=new Func<int, School>(GetInstance<SchoolData>().getSchoolInfoById);
        }
        public Tag[] getTagListBySchoolId(int schoolId){  
          int id=schoolId;
          string key = GetCacheKey( typeof(Tag), id);
            TagLazy[] list = DataCached.GetItem<TagLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<TagLazy>( "select * from tag where schoolId="+id).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;
        }
    public Tag getTagInfoById(int Id){
            string key = GetCacheKey(typeof(Tag), Id);
            TagLazy obj = DataCached.GetItem<TagLazy>(key);
            if (obj != null)
            {
                return obj;
            }
            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<TagLazy>("select * from Tag where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;
        }
        public void Add(Tag obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [tag]
           (
                name,
                schoolId
            )
     VALUES
   (
	            @name,
	            @schoolId
    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{
	        {"@name",obj.Name},
	        {"@schoolId",obj.SchoolId}
            }));
				obj.Id=identityValue;
            removeCache(typeof(Tag));
        }
    }
}