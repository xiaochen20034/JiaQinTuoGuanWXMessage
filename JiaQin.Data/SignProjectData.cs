
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
    public class SignProjectData:IDataExecutorImp
    {
        public SignProject[] List(string where,int pagesize, int pagenum, out int rowcount){

            string key = GetCacheKey(new Type[]{
                            typeof(SignProject)
                     }, new object[]{pagesize,pagenum,where});

            string keyRowcount = key + " rowcount";

            SignProjectLazy[] list = DataCached.GetItem<SignProjectLazy[]>(key);

            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }

            list = Executor.executePage<SignProjectLazy>("*", "SignProject", "id desc", where, pagesize, pagenum, out rowcount).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;

        }
        public SignProject[] List(){

            string key = GetCacheKey( typeof(SignProject));

            SignProjectLazy[] list = DataCached.GetItem<SignProjectLazy[]>(key);

            if (list != null)
            {
                return list;
            }

            list = Executor.executeForListObject<SignProjectLazy>( "select * from SignProject").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        void Lazy(SignProjectLazy obj){

       obj.SignRecordListLazy=new Func<int, SignRecord[]>(GetInstance<SignRecordData>().getSignRecordListBySignProjectId);

        }

    public SignProject getSignProjectInfoById(int Id){

            string key = GetCacheKey(typeof(SignProject), Id);

            SignProjectLazy obj = DataCached.GetItem<SignProjectLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<SignProjectLazy>("select * from SignProject where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;

        }
        public void Add(SignProject obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [SignProject]
           (

                name

            )
     VALUES
   (

	            @name

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@name",obj.Name}

            }));

				obj.Id=identityValue;

            removeCache(typeof(SignProject));
        }

        public void Update(SignProject obj){
            Executor.executeNonQuery(@"update [SignProject] set 

                name=@name

         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",obj.Id},

	        {"@name",obj.Name}

            });
            removeCache(typeof(SignProject));
        }
        public void Delete(int id){
            Executor.executeNonQuery("delete SignProject where id=@id", System.Data.CommandType.Text, new object[,]{                 
                {"@id",id }
            });
            removeCache(typeof(SignProject));            
        }

    }
}