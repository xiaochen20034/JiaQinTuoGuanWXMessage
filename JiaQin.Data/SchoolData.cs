
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
    public class SchoolData:IDataExecutorImp
    {
        public School[] List(string name,int pagesize, int pagenum, out int rowcount){

            string key = GetCacheKey(new Type[]{
                            typeof(School)
                     }, new object[] { pagesize, pagenum, name });

            string keyRowcount = key + " rowcount";

            SchoolLazy[] list = DataCached.GetItem<SchoolLazy[]>(key);

            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }
            string where = null;
            if (!string.IsNullOrEmpty(name))
            {
                where = "[name] like @name";
                Executor.addParameter("@name","%"+name+"%");
            }
            list = Executor.executePage<SchoolLazy>("*", "school", "id desc", where, pagesize, pagenum, out rowcount).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;

        }
        public School[] List(){

            string key = GetCacheKey( typeof(School));

            SchoolLazy[] list = DataCached.GetItem<SchoolLazy[]>(key);

            if (list != null)
            {
                return list;
            }

            list = Executor.executeForListObject<SchoolLazy>( "select * from school").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        void Lazy(SchoolLazy obj){

       obj.StudentListLazy=new Func<int, Student[]>(GetInstance<StudentData>().getStudentListBySchoolId);

       obj.TeacherListLazy=new Func<int, Teacher[]>(GetInstance<TeacherData>().getTeacherListBySchoolId);

       obj.TagListLazy=new Func<int, Tag[]>(GetInstance<TagData>().getTagListBySchoolId);

        }

    public School getSchoolInfoById(int Id){

            string key = GetCacheKey(typeof(School), Id);

            SchoolLazy obj = DataCached.GetItem<SchoolLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<SchoolLazy>("select * from School where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;

        }
    public bool HasStudentOrTeacher(int id) {
        Executor.addParameter("@id", id);
        return Convert.ToInt32(Executor.executeSclar("select count(1) from student s,teacher t where s.schoolId=@id or t.schoolId=@id")) > 0;
    }
    public bool ExistName(string name) {
        Executor.addParameter("@name",name);
        return Convert.ToInt32( Executor.executeSclar("select count(1) from school where [name]=@name"))>0;
    }
    public bool ExistName(string name,int notId)
    {
        Executor.addParameter("@name", name);
        Executor.addParameter("@id", notId);
        return Convert.ToInt32(Executor.executeSclar("select count(1) from school where [name]=@name and id<>@id")) > 0;
    }

        public void Add(School obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [school]
           (

                name,

                des,

                contactPhone,

                contactName

            )
     VALUES
   (

	            @name,

	            @des,

	            @contactPhone,

	            @contactName

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@name",obj.Name},

	        {"@des",obj.Des},

	        {"@contactPhone",obj.ContactPhone},

	        {"@contactName",obj.ContactName}

            }));

				obj.ID=identityValue;

            removeCache(typeof(School));
        }

        public void Update(School obj){
            Executor.executeNonQuery(@"update [school] set 

                name=@name,

                des=@des,

                contactPhone=@contactPhone,

                contactName=@contactName

         where ID=@ID", System.Data.CommandType.Text, new object[,]{
            {"@ID",obj.ID},

	        {"@name",obj.Name},

	        {"@des",obj.Des},

	        {"@contactPhone",obj.ContactPhone},

	        {"@contactName",obj.ContactName}

            });
            removeCache(typeof(School));
        }
        public void Delete(int ID){
            Executor.executeNonQuery("delete school where ID=@ID", System.Data.CommandType.Text, new object[,]{                 
                {"@ID",ID }
            });
            removeCache(typeof(School));            
        }

    }
}