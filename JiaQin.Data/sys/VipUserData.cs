
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
    public class VipUserData:IDataExecutorImp
    {
        
        

   


        public VipUser[] List(string name,int pagesize, int pagenum, out int rowcount){

            string key = GetCacheKey(new Type[]{
                            typeof(VipUser)
                     }, new object[]{pagesize,pagenum,name});

            string keyRowcount = key + " rowcount";

            VipUserLazy[] list = DataCached.GetItem<VipUserLazy[]>(key);

            if (list != null)
            {
                rowcount = DataCached.GetItem<int>(keyRowcount);
                return list;
            }
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrEmpty(name))
            {
                sb.Append(" name like @name");
                Executor.addParameter("@name","%"+name+"%");
            }
            list = Executor.executePage<VipUserLazy>("*", "vipUser", "id desc", sb.ToString(), pagesize, pagenum, out rowcount).ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;
            DataCached[keyRowcount] = rowcount;
            return list;

        }
        public VipUser[] List(string name){

            string key = GetCacheKey( typeof(VipUser),name);

            VipUserLazy[] list = DataCached.GetItem<VipUserLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@name","%"+name+"%");
            list = Executor.executeForListObject<VipUserLazy>("select top 20 * from vipUser where [name] like @name or  userName like @name").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }
            DataCached[key] = list;
            return list;

        }
       

        void Lazy(VipUserLazy obj){
            if (obj == null)
            {
                return;
            }

        }
    




        public void setVipUserWxOpenIdNull(Int64 vipUserId) {
            Executor.executeNonQuery(@"update [vipUser] set 
                openId=null,unionId=null,nickname=null,bindDate=null,bindAgent=null where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",vipUserId}
            });
            removeCache(typeof(VipUser));
        }
        /// <summary>
        /// 设置用户的微信信息
        /// </summary>
        /// <param name="user"></param>
        public void setVipUserWxInfo(VipUser user) {

            Executor.executeNonQuery(@"update [vipUser] set 
                openId=@openId,unionId=@unionId,headimgurl=@headimgurl,nickname=@nickname,bindDate=@bindDate,bindAgent=@bindAgent
         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",user.Id},

	        {"@openId",user.OpenId},

	        {"@unionId",user.Unionid},

	        {"@headimgurl",user.Headimgurl},

	        {"@nickname",user.Nickname},

	        {"@bindDate",user.BindDate},

	        {"@bindAgent",user.BindAgent}
            });
            removeCache(typeof(VipUser));
        }
        public VipUser getVipUserInfoByOpenId(string openId) {
            string key = GetCacheKey(typeof(VipUser), openId);
                VipUserLazy user = DataCached.GetItem<VipUserLazy>(key);
                if (user != null)
                {
                    return user;
                }
                Executor.addParameter("@openId", openId);
                user = Executor.executeForSingleObject<VipUserLazy>("select * from [vipUser] where openId=@openId");
                Lazy(user);
                DataCached[key] = user;
                return user;
        }

    public VipUser getVipUserInfoById(int Id){

            string key = GetCacheKey(typeof(VipUser), Id);

            VipUserLazy obj = DataCached.GetItem<VipUserLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<VipUserLazy>("select * from VipUser where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
            return obj;

    }/// <summary>
    /// 根据用户名获取用户信息
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public VipUser Info(string username)
    {
        string key = GetCacheKey(typeof(VipUser), username);
        VipUserLazy user = DataCached.GetItem<VipUserLazy>(key);
        if (user != null)
        {
            return user;
        }
        Executor.addParameter("@username", username);
        user = Executor.executeForSingleObject<VipUserLazy>("select * from [vipUser] where username=@username or phone=@username");
        Lazy(user);
        DataCached[key] = user;
        return user;
    }

    /// <summary>
    /// 指定的用户名是否存在
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    public bool Exist(string phone)
    {
        return Convert.ToInt32(Executor.executeSclar("select count(1) from [vipUser] where phone=@phone or username=@phone", System.Data.CommandType.Text, new object[,]{
                    {"@phone",phone}
            })) > 0;
    }

    public bool Exist(string phone,int id)
    {
        return Convert.ToInt32(Executor.executeSclar("select count(1) from [vipUser] where (phone=@phone or username=@phone) and id<>@id", System.Data.CommandType.Text, new object[,]{
                    {"@phone",phone},
                    {"@id",id}
            })) > 0;
    }



    public VipUser Info(int id) { return Info((Int64)id); }
    public VipUser Info(Int64 id)
    {
        string key = GetCacheKey(typeof(VipUser), id);
        VipUserLazy user = DataCached.GetItem<VipUserLazy>(key);
        if (user != null)
        {
            return user;
        }
        Executor.addParameter("@id", id);
        user = Executor.executeForSingleObject<VipUserLazy>("select * from [vipUser] where id=@id");
        Lazy(user);
        DataCached[key] = user;
        return user;
    }
    /// <summary>
    /// 根据用户名密码获取用户信息，密码是经过加密处理的，不进行缓存查询
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public VipUser Info(string userName, string password)
    {
        Executor.addParameter("@username", userName);
        Executor.addParameter("@phone", userName);
        Executor.addParameter("@password", password);
        VipUserLazy user = Executor.executeForSingleObject<VipUserLazy>("select * from [vipUser] where (username=@username or phone=@phone) and [passWord]=@password");
        Lazy(user);
        return user;

    }

        /// <summary>
        /// 添加基本信息（连同微信信息）
        /// </summary>
        /// <param name="obj"></param>
    public void AddBasicInfoWithWx(VipUser obj)
    {
        if (string.IsNullOrEmpty(obj.Code))
        {
            obj.Code = "wx"+obj.UserName;
        }
        int stuId = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [vipUser]
           (
                code,
                userName,
                passWord,

                name,

                gender,

                phone,

                photo,

                createDate,
openId,unionId,headimgurl,nickname,bindDate,bindAgent
            )
     VALUES
   (

	            @code,

	            @userName,

	            @passWord,

	            @name,

	            @gender,

	            @phone,

	            @photo,

	            @createDate,
@openId,@unionId,@headimgurl,@nickname,@bindDate,@bindAgent

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@code",obj.Code},

	        {"@userName",obj.UserName},

	        {"@passWord",obj.PassWord},

	        {"@name",obj.Name},

	        {"@gender",obj.Gender},

	        {"@phone",obj.Phone},

	        {"@photo",obj.Photo},

	        {"@createDate",obj.CreateDate},
            
            {"@openId",obj.OpenId},

	        {"@unionId",obj.Unionid},

	        {"@headimgurl",obj.Headimgurl},

	        {"@nickname",obj.Nickname},

	        {"@bindDate",obj.BindDate},

	        {"@bindAgent",obj.BindAgent}

            }));
        obj.Id = stuId;
        removeCache(typeof(VipUser));
    }

        /// <summary>
        /// 更新用户基本信息，包括，name，birthDate，gender,photo
        /// </summary>
        /// <param name="obj"></param>
    public void SetBasicInfo(VipUser obj)
    {
        Executor.executeNonQuery("update vipUser set [name]=@name,birthDate=@birthDate,gender=@gender,Photo=@photo where id=@id",CommandType.Text,new object[,]{
            {"@id",obj.Id},
            {"@name",obj.Name},
            {"@birthDate",obj.BirthDate},
            {"@gender",obj.Gender},
            {"@photo",obj.Photo}
        });
        removeCache(typeof(VipUser));
    }
    public void Register(VipUser obj)
    {
        int stuId = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [vipUser]
           (
                name,
                code,
                userName,
                passWord,
                phone,gender,                
                createDate,photo,
                Headimgurl,OpenId,Unionid,Nickname,BindDate,BindAgent
            )
     VALUES
   (            @name,
	            @code,
	            @userName,
	            @passWord,     
	            @phone,@gender,	           
	            @createDate,
                @photo,
                @Headimgurl,@OpenId,@Unionid,@Nickname,@BindDate,@BindAgent
    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{
            {"@name",obj.Name},
	        {"@code",obj.Code},
	        {"@userName",obj.UserName},
	        {"@passWord",obj.PassWord},
	             
	        {"@phone",obj.Phone},	  
            {"@gender",obj.Gender},
	        {"@createDate",DateTime.Now},

	        {"@photo",obj.Photo},

            {"@Headimgurl",obj.Headimgurl},
	        {"@OpenId",obj.OpenId},	        
	        {"@Unionid",obj.Unionid},	       
            {"@Nickname",obj.Nickname},	       
            {"@BindDate",DateTime.Now},	       
	        {"@BindAgent",obj.BindAgent}

            }));
        obj.Id = stuId;
        removeCache(typeof(VipUser));
    }
        public void Add(VipUser obj){
    int stuId = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [vipUser]
           (

                code,

                userName,

                passWord,

                name,

                gender,

                phone,

                photo,

                createDate

            )
     VALUES
   (

	            @code,

	            @userName,

	            @passWord,

	            @name,

	            @gender,

	            @phone,

	            @photo,

	            @createDate

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@code",obj.Code},

	        {"@userName",obj.UserName},

	        {"@passWord",obj.PassWord},

	        {"@name",obj.Name},

	        {"@gender",obj.Gender},

	        {"@phone",obj.Phone},

	        {"@photo",obj.Photo},

	        {"@createDate",obj.CreateDate}

            }));
            removeCache(typeof(VipUser));
        }

        public void Update(VipUser obj)
        {
            Executor.executeNonQuery(@"update [vipUser] set 

                name=@name,

                gender=@gender

                photo=@photo
         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",obj.Id},
	        {"@name",obj.Name},

	        {"@gender",obj.Gender},

	        {"@photo",obj.Photo}

            });
            removeCache(typeof(VipUser));
        }

        public int UpdateVip(VipUser obj, string[] paths,int temp)
        {

            if (temp==1) 
            {
                Executor.addParameter("@userName", obj.UserName);
                VipUser obj1 = Executor.executeForSingleObject<VipUserLazy>("select * from VipUser where userName=@userName");
                if (obj1 != null)
                    return 1;

            }            
            if (paths == null)
            {
                Executor.executeNonQuery(@"update [vipUser] set                 

                userName=@userName,

                name=@name,

                gender=@gender
         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",obj.Id},

            {"@userName",obj.UserName},

	        {"@name",obj.Name},

	        {"@gender",obj.Gender}
            });
            }
            else 
            {
                Executor.executeNonQuery(@"update [vipUser] set  

                userName=@userName,               

                name=@name,

                gender=@gender,                

                photo=@photo

         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",obj.Id},	        

	        {"@userName",obj.UserName},	        

	        {"@name",obj.Name},

	        {"@gender",obj.Gender},	        

	        {"@photo",paths[0]}
            });
            }

            removeCache(typeof(VipUser));
            return 0;
        }

        public void UpdateUserPhone(int id, string phone)
        {           

            Executor.executeNonQuery(@"update [vipUser] set          

                phone=@phone               

         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",id},
	        
	        {"@phone",phone}

            });
            removeCache(typeof(VipUser));
        }

        public void UpdatePwd(int id, string pwd)
        {

            Executor.executeNonQuery(@"update [vipUser] set          

                passWord=@passWord               

         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",id},
	        
	        {"@passWord",pwd}

            });
            removeCache(typeof(VipUser));
        }

        public void Delete(VipUser obj){
            Executor.executeNonQuery("delete vipUser where id=@id", System.Data.CommandType.Text, new object[,]{                 
                {"@id",obj.Id}
            });
            removeCache(typeof(VipUser));            
        }

    }
}