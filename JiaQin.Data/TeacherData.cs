
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
    public class TeacherData:IDataExecutorImp
    {
        public Teacher[] List(string where,int pagesize, int pagenum, out int rowcount){

            string key = GetCacheKey(new Type[]{
                            typeof(Teacher)
                     }, new object[]{pagesize,pagenum,where});

            string keyRowcount = key + " rowcount";

            TeacherLazy[] list = DataCached.GetItem<TeacherLazy[]>(key);

            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }

            list = Executor.executePage<TeacherLazy>("*", "teacher", "id desc", where, pagesize, pagenum, out rowcount).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;

        }
        public Teacher[] List(){

            string key = GetCacheKey( typeof(Teacher));

            TeacherLazy[] list = DataCached.GetItem<TeacherLazy[]>(key);

            if (list != null)
            {
                return list;
            }

            list = Executor.executeForListObject<TeacherLazy>( "select * from teacher").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        void Lazy(TeacherLazy obj){

        obj.VipUserInfoLazy=new Func<int, VipUser>(GetInstance<VipUserData>().getVipUserInfoById);

        obj.SchoolInfoLazy=new Func<int, School>(GetInstance<SchoolData>().getSchoolInfoById);

        }

        public Teacher[] getTeacherListByVipUserId(int vipUserId){  

          int id=vipUserId;
          string key = GetCacheKey( typeof(Teacher), id);

            TeacherLazy[] list = DataCached.GetItem<TeacherLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<TeacherLazy>( "select * from teacher where vipUserId="+id).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        public Teacher[] getTeacherListBySchoolId(int schoolId){  

          int id=schoolId;
          string key = GetCacheKey( typeof(Teacher), id);

            TeacherLazy[] list = DataCached.GetItem<TeacherLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<TeacherLazy>( "select * from teacher where schoolId="+id).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

    public Teacher getTeacherInfoById(int Id){

            string key = GetCacheKey(typeof(Teacher), Id);

            TeacherLazy obj = DataCached.GetItem<TeacherLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<TeacherLazy>("select * from Teacher where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;

        }
        public void Add(Teacher obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [teacher]
           (

                vipUserId,

                schoolId,

                addDate

            )
     VALUES
   (

	            @vipUserId,

	            @schoolId,

	            @addDate

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@vipUserId",obj.VipUserId},

	        {"@schoolId",obj.SchoolId},

	        {"@addDate",obj.AddDate}

            }));

            removeCache(typeof(Teacher));
        }

    }
}