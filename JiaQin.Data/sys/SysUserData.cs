using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
using JiaQin.Entity.Lazy;
namespace JiaQin.Data
{
    public class SysUserData:IDataExecutorImp
    {
        public SysUser[] ListByRoleId(int roleId)
        {
            
            string key = GetCacheKey(typeof(SysUser), roleId);
            SysUserLazy[] list = DataCached.GetItem<SysUserLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@roleId", roleId);
            list = Executor.executeForListObject<SysUserLazy>("select distinct u.* from sysUser u inner join  [sysUser-sysRole]  ur on u.id=ur.sysUserId  where ur.sysRoleId=@roleId").ToArray();
            Lazy(list);
            DataCached[key] = list;
            return list;
        }
        public SysUser[] ListByDpartmentId(int depId) {
            string key = GetCacheKey(typeof(SysUser), depId);
            SysUserLazy[] list = DataCached.GetItem<SysUserLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@DepartmentId", depId);
            list = Executor.executeForListObject<SysUserLazy>(@"select * from sysUser u
                            inner join [sysUser-Department] ud on u.id=ud.UserId
                            where ud.DepartmentId=@DepartmentId").ToArray();
            Lazy(list);
            DataCached[key] = list;
            return list;
        }

        public SysUser[] ListByDpartmentId(string name,int depId,int PageSize,int PageNum,out int rowCount)
        {
            string key = GetCacheKey(typeof(SysUser), new object[]{
                name,depId,PageSize,PageNum
            });
            string keyR = key + "rowcount";
            SysUserLazy[] list = DataCached.GetItem<SysUserLazy[]>(key);
            if (list != null)
            {
                rowCount = DataCached.GetItem<int>(keyR);
                return list;
            }
            StringBuilder sb = new StringBuilder("ud.DepartmentId=@DepartmentId");
            if (!string.IsNullOrEmpty(name))
            {
                sb.AppendFormat(" and u.Name like '%{0}%'", name);
            }
            Executor.addParameter("@DepartmentId", depId);
            list = Executor.executePage<SysUserLazy>("*", @"select * from sysUser u
                            inner join [sysUser-Department] ud on u.id=ud.sysUserId
                            where  " + sb.ToString(),"id desc",null,PageSize,PageNum,out rowCount).ToArray();
            Lazy(list);
            DataCached[key] = list;
            DataCached[keyR] = rowCount;
            return list;
        }

        /// <summary>
        /// 根据用户名获取用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public SysUser Info(string username)
        {
            string key = GetCacheKey(typeof(SysUser), username);
            SysUserLazy user = DataCached.GetItem<SysUserLazy>(key);
            if (user != null)
            {
                return user;
            }
            Executor.addParameter("@username", username);
            user = Executor.executeForSingleObject<SysUserLazy>("select * from [sysUser] where username=@username");
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
        public SysUser Info(string userName, string password)
        {
            Executor.addParameter("@username", userName);
            Executor.addParameter("@password", password);
            SysUserLazy user= Executor.executeForSingleObject<SysUserLazy>("select * from [SysUser] where username=@username and [passWord]=@password");
            Lazy(user);
            return user;
                 
        }
        public SysUser Info(int? id) {
            return id==null?null: Info(id.Value);
        }
        public SysUser Info(int id) {
            return Info((Int64)id);
        }
        public SysUser Info(Int64 id)
        {
            string key = GetCacheKey(typeof(SysUser), id);
            SysUserLazy obj = DataCached.GetItem<SysUserLazy>(key);
            if (obj != null)
            {
                return obj;
            }
            Executor.addParameter("@id", id);
            obj = Executor.executeForSingleObject<SysUserLazy>("select * from sysUser where id=@id");
            Lazy(obj);
            DataCached[key] = obj;
            return obj;

        }
        void Lazy(SysUserLazy[] list)
        {
            foreach (SysUserLazy item in list)
            {
                Lazy(item);
            }
        }
        void Lazy(SysUserLazy obj)
        {
            if (obj==null)
            {
                return;
            }
            obj.SysRoleListLazy = new Func<int, SysRole[]>(GetInstance<SysRoleData>().List);
            obj.SysPermissionListLazy = new Func<int, SysPermission[]>(GetInstance<SysPermissionData>().ListByUserId);
            obj.SchoolInfoLazy = new Func<int, School>(GetInstance<SchoolData>().getSchoolInfoByContactUserId);
        }
        /// <summary>
        /// 指定的用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool Exist(string userName)
        {
            return Convert.ToInt32( Executor.executeSclar("select count(1) from sysUser where userName=@userName", System.Data.CommandType.Text, new object[,]{
                    {"@userName",userName}
            }))>0;
        }
        /// <summary>
        /// 除去指定的记录，用户名是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Exist(string userName,int id)
        {
            return Convert.ToInt32(Executor.executeSclar("select count(1) from sysUser where userName=@userName and id<>@id", System.Data.CommandType.Text, new object[,]{
                    {"@userName",userName},{"@id",id}
            })) > 0;
        }



        public void Insert(SysUser user,int dept,int roleId) {
            try
            {
                int rowId = Convert.ToInt32(Executor.executeSclar("insert into sysUser(code,userName,[passWord],[name],gender,phone)values(@code,@userName,@passWord,@name,@gender,@phone);select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{
                    {"@code",string.IsNullOrEmpty(user.Code)?user.UserName:user.Code},
                    {"@userName",user.UserName},
                    {"@passWord",user.Password},
                    {"@name",user.Name},
                    {"@gender",user.Gender},
                    {"@phone",user.Phone}
                }, true));
                Executor.executeNonQuery("insert into [sysUser-Department](sysUserId,DepartmentId)values(@UserId,@departmentId)", System.Data.CommandType.Text, new object[,]{
                    {"@UserId",rowId},{"@departmentId",dept}
                },false);
                Executor.executeNonQuery("insert into [sysUser-sysRole](sysUserId,sysRoleId)values(@UserId,@sysRoleId)", System.Data.CommandType.Text, new object[,]{
                    {"@UserId",rowId},{"@sysRoleId",roleId}
                },false);
                
                Executor.transOver(true);
                removeCache(typeof(SysUser));
                removeCache(typeof(SysRole));
                removeCache(typeof(Department));
            }
            catch (Exception ea) {
                Executor.transOver(false);
                throw ea;
            }
           
        }
        /// <summary>
        /// 根据Id更新用户数据，但不更新用户名密码
        /// </summary>
        /// <param name="user"></param>
        public void Update(SysUser user,int roleId)
        {
            try
            {
                Executor.executeNonQuery("update sysUser set [name]=@name,gender=@gender,phone=@phone where id=@id", System.Data.CommandType.Text, new object[,]{               
                    {"@name",user.Name},
                    {"@gender",user.Gender},
                    {"@phone",user.Phone},
                    {"@id",user.Id}
            }, false);
                Executor.executeNonQuery("delete from  [sysUser-sysRole] where sysUserId=@sysUserId", System.Data.CommandType.Text, new object[,]{
                {"@sysUserId",user.Id}
            }, false);
                Executor.executeNonQuery("insert into [sysUser-sysRole](sysUserId,sysRoleId)values(@UserId,@sysRoleId)", System.Data.CommandType.Text, new object[,]{
                    {"@UserId",user.Id},{"@sysRoleId",roleId}
                }, false);
                Executor.transOver(true);
                removeCache(typeof(SysUser));
                removeCache(typeof(SysRole));
            }
            catch (Exception e) {
                Executor.transOver(false);
            }
            
        }

        public void UpdateBasicInfo(SysUser user)
        {
                Executor.executeNonQuery("update sysUser set UserName=@UserName, [name]=@name,gender=@gender,phone=@phone where id=@id", System.Data.CommandType.Text, new object[,]{               
                    {"@UserName",user.UserName},
                    {"@name",user.Name},
                    {"@gender",user.Gender},
                    {"@phone",user.Phone},
                    {"@id",user.Id}
            });
               
                removeCache(typeof(SysUser));

        }


        public void Delete(int id) {
            Executor.addParameter("@id",id);
            Executor.executeNonQuery("delete from sysUser where id=@id");
            removeCache(typeof(SysUser));
        }
        public void UpdatePwd(int id,string pwd) {
            Executor.addParameter("@id",id);
            Executor.addParameter("@passWord", pwd);
            Executor.executeNonQuery("update sysUser set passWord=@passWord where id=@id");
        }

        
    }
}
