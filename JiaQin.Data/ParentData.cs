
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
    /// 学生家长
    /// </summary>
    public class ParentData:IDataExecutorImp
    {
        public Parent[] List(string where,int pagesize, int pagenum, out int rowcount){
            string key = GetCacheKey(new Type[]{
                            typeof(Parent)
                     }, new object[]{pagesize,pagenum,where});
            string keyRowcount = key + " rowcount";
            ParentLazy[] list = DataCached.GetItem<ParentLazy[]>(key);
            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }
            list = Executor.executePage<ParentLazy>("*", "parent", "id desc", where, pagesize, pagenum, out rowcount).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;
        }
        public Parent[] List(){
            string key = GetCacheKey( typeof(Parent));
            ParentLazy[] list = DataCached.GetItem<ParentLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<ParentLazy>( "select * from parent").ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;
        }
        void Lazy(ParentLazy obj){
        obj.StudentInfoLazy=new Func<int, Student>(GetInstance<StudentData>().getStudentInfoById);
        obj.VipUserInfoLazy=new Func<int, VipUser>(GetInstance<VipUserData>().getVipUserInfoById);
        obj.VipUserInfoLazy=new Func<int, VipUser>(GetInstance<VipUserData>().getVipUserInfoById);
        }
        public Parent[] getParentListByStudentID(int studentID){  
          int id=studentID;
          string key = GetCacheKey( typeof(Parent), id);
            ParentLazy[] list = DataCached.GetItem<ParentLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<ParentLazy>( "select * from parent where studentID="+id).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;
        }
        public Parent[] getParentListByVipUserID(int vipUserID){  
          int id=vipUserID;
          string key = GetCacheKey( typeof(Parent), id);
            ParentLazy[] list = DataCached.GetItem<ParentLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<ParentLazy>( "select * from parent where vipUserID="+id).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;
        }
        public Parent[] getParentListByStuVipUserId(int stuVipUserId){  
          int id=stuVipUserId;
          string key = GetCacheKey( typeof(Parent), id);
            ParentLazy[] list = DataCached.GetItem<ParentLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<ParentLazy>( "select * from parent where stuVipUserId="+id).ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;
        }
    public Parent getParentInfoById(int Id){
            string key = GetCacheKey(typeof(Parent), Id);
            ParentLazy obj = DataCached.GetItem<ParentLazy>(key);
            if (obj != null)
            {
                return obj;
            }
            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<ParentLazy>("select * from Parent where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;
        }

    public Parent getParentInfoByVipUserId(int vipUserId)
    {

        string key = GetCacheKey(typeof(Parent), vipUserId);

        ParentLazy obj = DataCached.GetItem<ParentLazy>(key);

        if (obj != null)
        {
            return obj;
        }

        Executor.addParameter("@vipUserId", vipUserId);
        obj = Executor.executeForSingleObject<ParentLazy>("select * from Parent where vipUserId=@vipUserId");
        if (obj == null)
        {
            return null;
        }
        this.Lazy(obj);
        DataCached[key] = obj;
        return obj;

    }


    public Parent getParentInfoByStudentId(int studentId)
    {

        string key = GetCacheKey(typeof(Parent), studentId);

        ParentLazy obj = DataCached.GetItem<ParentLazy>(key);

        if (obj != null)
        {
            return obj;
        }

        Executor.addParameter("@studentId", studentId);
        obj = Executor.executeForSingleObject<ParentLazy>("select * from Parent where studentID=@studentId");
        if (obj == null)
        {
            return null;
        }
        this.Lazy(obj);
        DataCached[key] = obj;
        return obj;

    }
        public void Add(Parent obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [parent]
           (
                studentID,
                vipUserID,
                stuVipUserId
            )
     VALUES
   (
	            @studentID,
	            @vipUserID,
	            @stuVipUserId
    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{
	        {"@studentID",obj.StudentID},
	        {"@vipUserID",obj.VipUserID},
	        {"@stuVipUserId",obj.StuVipUserId}
            }));
				obj.ID=identityValue;
            removeCache(typeof(Parent));
        }
        public void Update(Parent obj){
            Executor.executeNonQuery(@"update [parent] set 
                studentID=@studentID,
                vipUserID=@vipUserID,
                stuVipUserId=@stuVipUserId
         where ID=@ID", System.Data.CommandType.Text, new object[,]{
            {"@ID",obj.ID},
	        {"@studentID",obj.StudentID},
	        {"@vipUserID",obj.VipUserID},
	        {"@stuVipUserId",obj.StuVipUserId}
            });
            removeCache(typeof(Parent));
        }
        public void Delete(int ID){
            Executor.executeNonQuery("delete parent where ID=@ID", System.Data.CommandType.Text, new object[,]{                 
                {"@ID",ID }
            });
            removeCache(typeof(Parent));            
        }
    }
}