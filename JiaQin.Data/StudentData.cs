
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
    /// 学生，基础信息来自user
    /// </summary>
    public class StudentData:IDataExecutorImp
    {
        public Student[] List(string where,int pagesize, int pagenum, out int rowcount){

            string key = GetCacheKey(new Type[]{
                            typeof(Student)
                     }, new object[]{pagesize,pagenum,where});

            string keyRowcount = key + " rowcount";

            StudentLazy[] list = DataCached.GetItem<StudentLazy[]>(key);

            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }

            list = Executor.executePage<StudentLazy>("*", "student", "id desc", where, pagesize, pagenum, out rowcount).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;

        }
        public Student[] List(){

            string key = GetCacheKey( typeof(Student));

            StudentLazy[] list = DataCached.GetItem<StudentLazy[]>(key);

            if (list != null)
            {
                return list;
            }

            list = Executor.executeForListObject<StudentLazy>( "select * from student").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        void Lazy(StudentLazy obj){

        obj.SchoolInfoLazy=new Func<int, School>(GetInstance<SchoolData>().getSchoolInfoById);

       obj.ParentListLazy=new Func<int, Parent[]>(GetInstance<ParentData>().getParentListByStudentID);

       obj.VipUserInfoLazy=new Func<int, VipUser>(GetInstance<VipUserData>().getVipUserInfoById);

       obj.SignRecordListLazy=new Func<int, SignRecord[]>(GetInstance<SignRecordData>().getSignRecordListByStuId);

        }

        public Student[] getStudentListBySchoolId(int schoolId){  

          int id=schoolId;
          string key = GetCacheKey( typeof(Student), id);

            StudentLazy[] list = DataCached.GetItem<StudentLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<StudentLazy>( "select * from student where schoolId="+id).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

    public Student getStudentInfoById(int Id){

            string key = GetCacheKey(typeof(Student), Id);

            StudentLazy obj = DataCached.GetItem<StudentLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<StudentLazy>("select * from Student where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;

        }
        public void Add(Student obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [student]
           (

                vipUserID,

                times,

                taglist,

                schoolId

            )
     VALUES
   (

	            @vipUserID,

	            @times,

	            @taglist,

	            @schoolId

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@vipUserID",obj.VipUserID},

	        {"@times",obj.Times},

	        {"@taglist",obj.Taglist},

	        {"@schoolId",obj.SchoolId}

            }));

				obj.ID=identityValue;

            removeCache(typeof(Student));
        }

        public void Update(Student obj){
            Executor.executeNonQuery(@"update [student] set 

                vipUserID=@vipUserID,

                times=@times,

                taglist=@taglist,

                schoolId=@schoolId

         where ID=@ID", System.Data.CommandType.Text, new object[,]{
            {"@ID",obj.ID},

	        {"@vipUserID",obj.VipUserID},

	        {"@times",obj.Times},

	        {"@taglist",obj.Taglist},

	        {"@schoolId",obj.SchoolId}

            });
            removeCache(typeof(Student));
        }
        public void Delete(int ID){
            Executor.executeNonQuery("delete student where ID=@ID", System.Data.CommandType.Text, new object[,]{                 
                {"@ID",ID }
            });
            removeCache(typeof(Student));            
        }

    }
}