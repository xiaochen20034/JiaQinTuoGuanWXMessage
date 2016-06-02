
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
       obj.SysUserInfoLazy = new Func<int, SysUser>(GetInstance<SysUserData>().Info);

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

    public School getSchoolInfoByContactUserId(int userId)
    {

        string key = GetCacheKey(typeof(School), userId);

        SchoolLazy obj = DataCached.GetItem<SchoolLazy>(key);

        if (obj != null)
        {
            return obj;
        }

        Executor.addParameter("@id", userId);
        obj = Executor.executeForSingleObject<SchoolLazy>("select * from School where userId=@id");
        if (obj == null)
        {//非管理员用户，也就是教师用户
            Executor.addParameter("@id", userId);
            obj = Executor.executeForSingleObject<SchoolLazy>("select s.* from school s inner join teacher t on s.id=t.schoolId where t.userId=@id");
            
        }
        if (obj==null)
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

            SysUserData userData=GetInstance<SysUserData>();
            if (!userData.Exist(obj.ContactPhone))
            {
                SysRole role = GetInstance<SysRoleData>().Info("school");
                Department department = GetInstance<DepartmentData>().Info("school");
                if (role==null || department==null)
                {
                    throw new Exception("系统数据不正确，没有校区角色，或者校区部门");
                }
                userData.Insert(new SysUser() { 
                    Code=obj.ContactPhone,
                    Name=obj.ContactName,
                    Phone=obj.ContactPhone,
                    UserName=obj.ContactPhone,
                    Password=new Zhyj.Common.Base64Encoding().Encode("999999")
                },department.Id,role.Id);
            }
            SysUser user= userData.Info(obj.ContactPhone);
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [school]
           (
                name,
                des,
                contactPhone,
                contactName,userId

            )
     VALUES
   (

	            @name,

	            @des,

	            @contactPhone,

	            @contactName,@userId

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@name",obj.Name},

	        {"@des",obj.Des},

	        {"@contactPhone",obj.ContactPhone},

	        {"@contactName",obj.ContactName},
            {"@userId",user.Id}

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