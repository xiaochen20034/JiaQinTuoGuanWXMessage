
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
        /// <summary>
        /// 教师列表，根据姓名进行查询（所有的教师，不只是当前学校）
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pagesize"></param>
        /// <param name="pagenum"></param>
        /// <param name="rowcount"></param>
        /// <returns></returns>
        public Teacher[] List(int schoolId,string name,int pagesize, int pagenum, out int rowcount){

            string key = GetCacheKey(new Type[]{
                            typeof(Teacher)
                     }, new object[] { schoolId,pagesize, pagenum, name });

            string keyRowcount = key + " rowcount";

            TeacherLazy[] list = DataCached.GetItem<TeacherLazy[]>(key);

            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }
            string where = null;
            if (!string.IsNullOrEmpty(name))
            {
                where = "[name] like @name";
                Executor.addParameter("@name", "%" + name + "%");
            }
            list = Executor.executePage<TeacherLazy>("* ", " select t.*,u.userName,u.Name from sysUser u inner join teacher t on u.id=t.userId   and t.schoolId=" + schoolId, "schoolId,userName,id desc", where, pagesize, pagenum, out rowcount).ToArray();

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
        public Teacher[] ListByTagId(int tagId)
        {

            string key = GetCacheKey(typeof(Teacher), tagId);

            TeacherLazy[] list = DataCached.GetItem<TeacherLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@tagId", tagId);
            list = Executor.executeForListObject<TeacherLazy>("select stu.* from student stu inner join studentTag t on stu.id=t.studentId where t.tagId=@tagId").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }
        void Lazy(TeacherLazy obj){

        obj.SysUserInfoLazy=new Func<int, SysUser>(GetInstance<SysUserData>().Info);

        obj.SchoolInfoLazy=new Func<int, School>(GetInstance<SchoolData>().getSchoolInfoById);
        obj.TagListLazy = new Func<int, Tag[]>(GetInstance<TagData>().ListByTeacherId);
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

    public Teacher getTeacherInfoByUserId(int userId)
    {

        string key = GetCacheKey(typeof(Teacher), userId);

        TeacherLazy obj = DataCached.GetItem<TeacherLazy>(key);

        if (obj != null)
        {
            return obj;
        }

        Executor.addParameter("@userId", userId);
        obj = Executor.executeForSingleObject<TeacherLazy>("select * from Teacher where userId=@userId");
        if (obj == null)
        {
            return null;
        }
        this.Lazy(obj);
        DataCached[key] = obj;
        return obj;

    }

        public void Add(SysUser obj,int schoolId,string[]tag){
           
                SysUserData userData = GetInstance<SysUserData>();
                if (!userData.Exist(obj.Phone))
                {
                    SysRole role = GetInstance<SysRoleData>().Info("teacher");
                    Department department = GetInstance<DepartmentData>().Info("teacher");
                    if (role == null || department == null)
                    {
                        throw new Exception("系统数据不正确，没有教师角色，或者教师部门");
                    }
                    userData.Insert(obj, department.Id, role.Id);
                }
                SysUser user = userData.Info(obj.Phone);
                int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [teacher]
           (

                userId,

                schoolId,

                addDate

            )
     VALUES
   (

	            @userId,

	            @schoolId,

	            @addDate

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@userId",user.Id},

	        {"@schoolId",schoolId},

	        {"@addDate",DateTime.Now}

            }));
                StringBuilder sb = new StringBuilder();
                foreach (string item in tag)
                {
                    if (Convert.ToInt32(Executor.executeSclar("select count(1) from teacherTag where teacherId=@teacherId and tagId=@tagId", CommandType.Text,new object[,]{
                            {"@teacherId",identityValue},
                            {"@tagId",Convert.ToInt32(item)},
                        }))==0)
                    {
                        Executor.executeNonQuery("insert into teacherTag(teacherId,tagId)values(@teacherId,@tagId)", CommandType.Text,new object[,]{
                            {"@teacherId",identityValue},
                            {"@tagId",Convert.ToInt32(item)},
                        });
                    }
                    sb.Append("," + item);
                }
                if (sb.Length>0)
                {
                    sb.Remove(0,1);
                    Executor.executeNonQuery("delete teacherTag where  teacherId =@teacherId and  tagId not in("+sb.ToString()+")", CommandType.Text, new object[,]{
                            {"@teacherId",identityValue}
                        });
                }
            
                removeCache(typeof(Teacher));
                removeCache(typeof(Tag));
          
    
        }

        public void Update(SysUser obj, string[] tag)
        {

            SysUserData userData = GetInstance<SysUserData>();

            Teacher teacherInfo = getTeacherInfoByUserId(obj.Id);
            userData.UpdateBasicInfo(obj);
            StringBuilder sb = new StringBuilder();
            foreach (string item in tag)
            {
                if (Convert.ToInt32(Executor.executeSclar("select count(1) from teacherTag where teacherId=@teacherId and tagId=@tagId", CommandType.Text, new object[,]{
                            {"@teacherId",teacherInfo.Id},
                            {"@tagId",Convert.ToInt32(item)},
                        })) == 0)
                {
                    Executor.executeNonQuery("insert into teacherTag(teacherId,tagId)values(@teacherId,@tagId)", CommandType.Text, new object[,]{
                            {"@teacherId",teacherInfo.Id},
                            {"@tagId",Convert.ToInt32(item)},
                        });
                }
                sb.Append("," + item);
            }
            if (sb.Length > 0)
            {
                sb.Remove(0, 1);
                Executor.executeNonQuery("delete teacherTag where  teacherId =@teacherId and  tagId not in(" + sb.ToString() + ")", CommandType.Text, new object[,]{
                            {"@teacherId",teacherInfo.Id}
                        });
            }

            removeCache(typeof(Teacher));
            removeCache(typeof(Tag));


        }



        public void Restore(int id)
        {
            Executor.addParameter("@userId", id);
            Executor.executeNonQuery("update teacher set deleteDate=null where  userId=@userId");
            removeCache(typeof(Teacher));
        }
        public void Delete(int id) {
            Executor.addParameter("@date", DateTime.Now);
            Executor.addParameter("@userId", id);
            Executor.executeNonQuery("update teacher set deleteDate=@date where  userId=@userId");
            removeCache(typeof(Teacher));
        }
    }
}