
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
        public Student[] List(int schoolId, string name, int pagesize, int pagenum, out int rowcount)
        {

            string key = GetCacheKey(new Type[]{
                            typeof(Student)
                     }, new object[] { schoolId, pagesize, pagenum, name });

            string keyRowcount = key + " rowcount";

            StudentLazy[] list = DataCached.GetItem<StudentLazy[]>(key);

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
            list = Executor.executePage<StudentLazy>("* ", " select t.*,u.userName,u.Name from vipUser u inner join student t on u.id=t.vipUserID  and t.schoolId=" + schoolId, "schoolId,userName,id desc", where, pagesize, pagenum, out rowcount).ToArray();

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
        public Student[] ListByTagId(int tagId)
        {

            string key = GetCacheKey(typeof(Student), tagId);

            StudentLazy[] list = DataCached.GetItem<StudentLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@tagId",tagId);
            list = Executor.executeForListObject<StudentLazy>("select stu.* from student stu inner join studentTag t on stu.id=t.studentId where t.tagId=@tagId").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        public Student[] ListByTagIdOfSchoolId(int tagId,int schoolId,string studentName)
        {

            string key = GetCacheKey(new Type[] { typeof(Student), typeof(School) },
                new object[] { tagId, schoolId, studentName });

            StudentLazy[] list = DataCached.GetItem<StudentLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@tagId", tagId);
            Executor.addParameter("@schoolId", schoolId);
        
            if (!string.IsNullOrEmpty(studentName))
            {
                Executor.addParameter("@studentName", "%" + studentName + "%");
                list = Executor.executeForListObject<StudentLazy>("select stu.* from student stu inner join studentTag t on stu.id=t.studentId inner join vipUser u on u.id=stu.vipUserID where t.tagId=@tagId and stu.schoolId=@schoolId and u.Name like @studentName ").ToArray();
            }
            else
            {
                list = Executor.executeForListObject<StudentLazy>("select stu.* from student stu inner join studentTag t on stu.id=t.studentId where t.tagId=@tagId and stu.schoolId=@schoolId ").ToArray();
            }
            

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
       //obj.TagListLazy = new Func<int, Tag[]>(GetInstance<TagData>().ListByStudentId);
       obj.ParentInfoLazy = new Func<int, Parent>(GetInstance<ParentData>().getParentInfoByStudentId);
       obj.StudentTagListLazy = new Func<int,StudentTag[]>(GetInstance<StudentTagData>().ListByStudentId);

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
        public void Add(string studentName,string studentGender,int age,string parentName,string parentPhone,int schoolId,int times,string []tags){
            try
            {
                int stuVipUserId = Convert.ToInt32(Executor.executeSclar("insert into vipUser(code,userName,password,[name],gender,createDate)values(@code,@userName,@password,@name,@gender,@createDate);select SCOPE_IDENTITY()", CommandType.Text, new object[,]{
                {"@code","s_"+parentPhone},
                {"@userName","s_"+parentPhone},
                {"@password",null},
                {"@name",studentName},
                {"@gender",studentGender},
                {"@createDate",DateTime.Now}
            }, true));

                int parentVipUserId = Convert.ToInt32(Executor.executeSclar("insert into vipUser(code,userName,password,[name],createDate,Phone)values(@code,@userName,@password,@name,@createDate,@Phone);select SCOPE_IDENTITY()", CommandType.Text, new object[,]{
                {"@code",parentPhone},
                {"@userName",parentPhone},
                {"@password",new Zhyj.Common.Base64Encoding().Encode("999999")},
                {"@name",parentName},
                {"@createDate",DateTime.Now},
                {"@Phone",parentPhone},
            }, true));



                int stuId = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [student]
           (

                vipUserID,

                times,

                schoolId,age

            )
     VALUES
   (

	            @vipUserID,

	            @times,

	            @schoolId,@age

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@vipUserID",stuVipUserId},

	        {"@times",times},

	        {"@schoolId",schoolId},
            {"@age",age}

            }, true));


                int parentId = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [parent]
           (

                vipUserID,

                stuVipUserId,

                studentId
            )
     VALUES
   (

	            @vipUserID,

	            @stuVipUserId,

	            @studentId

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@vipUserID",parentVipUserId},

	        {"@stuVipUserId",stuVipUserId},

	        {"@studentId",stuId}

            }, true));

                StringBuilder sb = new StringBuilder();
                if (tags==null)
                {
                    tags = new string[0];
                }
                foreach (string item in tags)
                {
                    if (Convert.ToInt32(Executor.executeSclar("select count(1) from studentTag where studentId=@studentId and tagId=@tagId", CommandType.Text, new object[,]{
                            {"@studentId",stuId},
                            {"@tagId",Convert.ToInt32(item)},
                        },true)) == 0)
                    {
                        Executor.executeNonQuery("insert into studentTag(studentId,tagId)values(@studentId,@tagId)", CommandType.Text, new object[,]{
                            {"@studentId",stuId},
                            {"@tagId",Convert.ToInt32(item)},
                        },false);
                    }
                    sb.Append("," + item);
                }
                if (sb.Length > 0)
                {
                    sb.Remove(0, 1);
                    Executor.executeNonQuery("delete studentTag where  studentId =@studentId and  tagId not in(" + sb.ToString() + ") and isnull(times,0)<=0", CommandType.Text, new object[,]{
                            {"@studentId",stuId}
                        },false);
                }
                Executor.transOver(true);
                removeCache(typeof(Student));
                removeCache(typeof(Parent));
                removeCache(typeof(VipUser));
            }
            catch (Exception ea)
            {
                Executor.transOver(false);
                throw;
            }
        }

        public void Update(string studentName, string studentGender, int age,int times, string parentName, string parentPhone, int studentId, int studentVipUserId,int parentVipUserId, string[] tags)
        {

            try
            {
                Executor.executeNonQuery("update vipUser set code=@code,userName=@userName,name=@name,gender=@gender where id=@studentVipUserId", CommandType.Text, new object[,]{
                {"@code","s_"+parentPhone},
                {"@userName","s_"+parentPhone},
                {"@name",studentName},
                {"@gender",studentGender},
                {"@studentVipUserId",studentVipUserId},
            }, false);

                Executor.executeNonQuery("update student set age=@age,times=@times  where id=@studentId", CommandType.Text, new object[,]{
                {"@age",age},
                {"@times",times},
                {"@studentId",studentId},
            }, false);

                Executor.executeNonQuery("update vipUser set code=@code,userName=@userName,name=@name,Phone=@Phone where id=@parentVipUserId", CommandType.Text, new object[,]{
                {"@code",parentPhone},
                {"@userName",parentPhone},
                {"@name",parentName},
                {"@Phone",parentPhone},
                {"@parentVipUserId",parentVipUserId},
            }, false);



                StringBuilder sb = new StringBuilder();
                if (tags==null)
                {
                    tags = new string[0];
                }
                foreach (string item in tags)
                {
                    if (Convert.ToInt32(Executor.executeSclar("select count(1) from studentTag where studentId=@studentId and tagId=@tagId", CommandType.Text, new object[,]{
                            {"@studentId",studentId},
                            {"@tagId",Convert.ToInt32(item)},
                        },true)) == 0)
                    {
                        Executor.executeNonQuery("insert into studentTag(studentId,tagId)values(@studentId,@tagId)", CommandType.Text, new object[,]{
                            {"@studentId",studentId},
                            {"@tagId",Convert.ToInt32(item)},
                        },false);
                    }
                    sb.Append("," + item);
                }
                if (sb.Length > 0)
                {
                    sb.Remove(0, 1);
                    Executor.executeNonQuery("delete studentTag where  studentId =@studentId and  tagId not in(" + sb.ToString() + ")", CommandType.Text, new object[,]{
                            {"@studentId",studentId}
                        },false);
                }
                else {
                    Executor.executeNonQuery("delete studentTag where  studentId =@studentId ", CommandType.Text, new object[,]{
                            {"@studentId",studentId}
                        }, false);
                }
                Executor.transOver(true);
                removeCache(typeof(Student));
                removeCache(typeof(Parent));
                removeCache(typeof(Tag));
                removeCache(typeof(VipUser));
            }
            catch (Exception ea)
            {
                Executor.transOver(false);
                throw;
            }


        }
      

        public void Restore(int id)
        {
            Executor.addParameter("@userId", id);
            Executor.executeNonQuery("update student set deleteDate=null where  id=@userId");
            removeCache(typeof(Student));
        }
        public void Delete(int id)
        {
            Executor.addParameter("@date", DateTime.Now);
            Executor.addParameter("@userId", id);
            Executor.executeNonQuery("update student set deleteDate=@date where  id=@userId");
            removeCache(typeof(Student));
        }



    }
}