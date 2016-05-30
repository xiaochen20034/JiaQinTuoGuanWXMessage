
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
    /// 项目签到发消息记录
    /// </summary>
    public class SignRecordData:IDataExecutorImp
    {
        public SignRecord[] List(string where,int pagesize, int pagenum, out int rowcount){

            string key = GetCacheKey(new Type[]{
                            typeof(SignRecord)
                     }, new object[]{pagesize,pagenum,where});

            string keyRowcount = key + " rowcount";

            SignRecordLazy[] list = DataCached.GetItem<SignRecordLazy[]>(key);

            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }

            list = Executor.executePage<SignRecordLazy>("*", "signRecord", "id desc", where, pagesize, pagenum, out rowcount).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;

        }
        public SignRecord[] List(){

            string key = GetCacheKey( typeof(SignRecord));

            SignRecordLazy[] list = DataCached.GetItem<SignRecordLazy[]>(key);

            if (list != null)
            {
                return list;
            }

            list = Executor.executeForListObject<SignRecordLazy>( "select * from signRecord").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        void Lazy(SignRecordLazy obj){

        obj.SignProjectInfoLazy=new Func<int, SignProject>(GetInstance<SignProjectData>().getSignProjectInfoById);

        obj.StudentInfoLazy=new Func<int, Student>(GetInstance<StudentData>().getStudentInfoById);

        obj.VipUserInfoLazy=new Func<int, VipUser>(GetInstance<VipUserData>().getVipUserInfoById);

        }

        public SignRecord[] getSignRecordListBySignProjectId(int signProjectId){  

          int id=signProjectId;
          string key = GetCacheKey( typeof(SignRecord), id);

            SignRecordLazy[] list = DataCached.GetItem<SignRecordLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<SignRecordLazy>( "select * from signRecord where signProjectId="+id).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        public SignRecord[] getSignRecordListByStuId(int stuId){  

          int id=stuId;
          string key = GetCacheKey( typeof(SignRecord), id);

            SignRecordLazy[] list = DataCached.GetItem<SignRecordLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<SignRecordLazy>( "select * from signRecord where stuId="+id).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

        public SignRecord[] getSignRecordListByTeaVipUserId(int teaVipUserId){  

          int id=teaVipUserId;
          string key = GetCacheKey( typeof(SignRecord), id);

            SignRecordLazy[] list = DataCached.GetItem<SignRecordLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<SignRecordLazy>( "select * from signRecord where teaVipUserId="+id).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }

    public SignRecord getSignRecordInfoById(int Id){

            string key = GetCacheKey(typeof(SignRecord), Id);

            SignRecordLazy obj = DataCached.GetItem<SignRecordLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<SignRecordLazy>("select * from SignRecord where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
			DataCached[key] = obj;
            return obj;

        }

        public void Add(SignRecord obj){
    int identityValue = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [signRecord]
           (

                signProjectId,

                stuVipUserId,

                stuId,

                signDate,

                readDate,

                teaVipUserId,

                teaId

            )
     VALUES
   (

	            @signProjectId,

	            @stuVipUserId,

	            @stuId,

	            @signDate,

	            @readDate,

	            @teaVipUserId,

	            @teaId

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@signProjectId",obj.SignProjectId},

	        {"@stuVipUserId",obj.StuVipUserId},

	        {"@stuId",obj.StuId},

	        {"@signDate",obj.SignDate},

	        {"@readDate",obj.ReadDate},

	        {"@teaVipUserId",obj.TeaVipUserId},

	        {"@teaId",obj.TeaId}

            }));

				obj.Id=identityValue;

            removeCache(typeof(SignRecord));
        }

        public void Update(SignRecord obj){
            Executor.executeNonQuery(@"update [signRecord] set 

                signProjectId=@signProjectId,

                stuVipUserId=@stuVipUserId,

                stuId=@stuId,

                signDate=@signDate,

                readDate=@readDate,

                teaVipUserId=@teaVipUserId,

                teaId=@teaId

         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",obj.Id},

	        {"@signProjectId",obj.SignProjectId},

	        {"@stuVipUserId",obj.StuVipUserId},

	        {"@stuId",obj.StuId},

	        {"@signDate",obj.SignDate},

	        {"@readDate",obj.ReadDate},

	        {"@teaVipUserId",obj.TeaVipUserId},

	        {"@teaId",obj.TeaId}

            });
            removeCache(typeof(SignRecord));
        }
        public void Delete(int id){
            Executor.executeNonQuery("delete signRecord where id=@id", System.Data.CommandType.Text, new object[,]{                 
                {"@id",id }
            });
            removeCache(typeof(SignRecord));            
        }

    }
}