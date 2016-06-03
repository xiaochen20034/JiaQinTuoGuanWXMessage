
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
        public DataTable TagTableBySchoolId(int schoolId) {
            string key = GetCacheKey(typeof(Tag),schoolId);
            DataTable dt = DataCached.GetItem<DataTable>(key);
            if (dt!=null)
            {
                return dt;
            }
            Executor.addParameter("@schoolId",schoolId);
            dt = Executor.executeForDataTable("select t.Id,t.Name,count(1) as StudentCount from student stu inner join studentTag st on stu.id=st.studentId inner join tag t on st.tagId=t.id where t.schoolId=@schoolId or t.schoolId is null group by t.Name,t.Id");
            DataCached[key] = dt;
            return dt;
        }
        public Tag[] List(string name,int pagesize, int pagenum, out int rowcount){
            string key = GetCacheKey(new Type[]{
                            typeof(Tag)
                     }, new object[]{pagesize,pagenum,name});
            string keyRowcount = key + " rowcount";
            TagLazy[] list = DataCached.GetItem<TagLazy[]>(key);
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

        public Tag[] ListByTeacherId(int teacherId)
        {

            string key = GetCacheKey(typeof(Tag), teacherId);

            TagLazy[] list = DataCached.GetItem<TagLazy[]>(key);

            if (list != null)
            {
                return list;
            }

            list = Executor.executeForListObject<TagLazy>("select t.* from tag t inner join teacherTag tt on t.id=tt.tagId where tt.teacherId="+teacherId).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }
        public Tag[] ListByStudentId(int studentId)
        {

            string key = GetCacheKey(typeof(Tag), studentId);

            TagLazy[] list = DataCached.GetItem<TagLazy[]>(key);

            if (list != null)
            {
                return list;
            }

            list = Executor.executeForListObject<TagLazy>("select t.* from tag t inner join studentTag tt on t.id=tt.tagId where tt.studentId=" + studentId).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }
        void Lazy(TagLazy obj){
            obj.SchoolInfoLazy=new Func<int, School>(GetInstance<SchoolData>().getSchoolInfoById);
            obj.StudentListLazy = new Func<int, Student[]>(GetInstance<StudentData>().ListByTagId);
            obj.TeacherListLazy = new Func<int, Teacher[]>(GetInstance<TeacherData>().ListByTagId);
            obj.StudentCountLazy = new Func<int, int>(TagStudentCount);
            obj.TeacherCountLazy = new Func<int, int>(TagTeacherCount);
        }
        int TagStudentCount(int tagId) {
            Executor.addParameter("@tagId",tagId);
            return Convert.ToInt32(Executor.executeSclar("select count(1) from studentTag where tagId=@tagId"));        }
        int TagTeacherCount(int tagId)
        {
            Executor.addParameter("@tagId", tagId);
            return Convert.ToInt32(Executor.executeSclar("select count(1) from teacherTag where tagId=@tagId"));        }        
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
    public bool ExistName(string name)
    {
        Executor.addParameter("@name", name);
        return Convert.ToInt32(Executor.executeSclar("select count(1) from tag where [name]=@name")) > 0;
    }
    public bool ExistName(string name, int notId)
    {
        Executor.addParameter("@name", name);
        Executor.addParameter("@id", notId);
        return Convert.ToInt32(Executor.executeSclar("select count(1) from tag where [name]=@name and id<>@id")) > 0;
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
        public void Update(Tag obj)
        {
            Executor.executeNonQuery(@"update [tag]
          set  name=@name where id=@id", System.Data.CommandType.Text, new object[,]{
	        {"@name",obj.Name},
	        {"@id",obj.Id}
            });
            removeCache(typeof(Tag));
        }
        /// <summary>
        /// 标签是否被学生或者老师使用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool HasStudentOrTeacher(int id) {
            Executor.addParameter("@tagId",id);
            return Convert.ToInt32(Executor.executeSclar("select count(1) from teacherTag s,studentTag t where s.tagId=@tagId or t.tagId=@tagId"))>0;
        }
        public void Delete(int id)
        {
            try
            {
                Executor.executeNonQuery(@"delete from  [teacherTag]
           where tagId=@tagId", System.Data.CommandType.Text, new object[,]{
	        {"@tagId",id}
            }, false);
                Executor.executeNonQuery(@"delete from  [studentTag]
           where tagId=@tagId", System.Data.CommandType.Text, new object[,]{
	        {"@tagId",id}
            }, false);
                Executor.executeNonQuery(@"delete from  [tag]
           where id=@tagId", System.Data.CommandType.Text, new object[,]{
	        {"@tagId",id}
            }, false);

                Executor.transOver(true);
            }
            catch (Exception)
            {
                Executor.transOver(false);
                throw;
            }
            removeCache(typeof(Tag));
        }
    }
}