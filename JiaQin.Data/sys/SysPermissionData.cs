using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
using JiaQin.Entity.Lazy;
namespace JiaQin.Data
{
    public class SysPermissionData:IDataExecutorImp
    {
        public SysPermission[] RootList() {
            string key = GetCacheKey(typeof(SysPermission));
            SysPermissionLazy[] list = DataCached.GetItem<SysPermissionLazy[]>(key);
            if (list != null)
            {
                return list;
            }

            list = Executor.executeForListObject<SysPermissionLazy>("select * from sysPermission where parentId=0 or parentId is null ").ToArray();
            foreach (SysPermissionLazy item in list)
            {
                Lazy(item);
            }
            DataCached[key] = list;
            return list;
        }
        public SysPermission[] List(int permissionId) {
            string key = GetCacheKey(typeof(SysPermission), permissionId);
            SysPermissionLazy[] list = DataCached.GetItem<SysPermissionLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            string where = null;
            if (permissionId == 0)
            {
                where = " where  (parentId=0 or parentId is null)";
            }
            else
            {
                where = " where  parentId=" + permissionId;
            }

            list = Executor.executeForListObject<SysPermissionLazy>("select  * from sysPermission " + where + "  order by sort ").ToArray();
            DataCached[key] = list;
            Lazy(list);
            return list;
        }
        public SysPermission[] ListByRoleId(int roleId){
            string key = GetCacheKey(typeof(SysPermission),roleId);
            SysPermissionLazy[] list = DataCached.GetItem<SysPermissionLazy[]>(key);
            if (list!=null)
            {
                return list;
            }
            Executor.addParameter("@roleId",roleId);
            list = Executor.executeForListObject<SysPermissionLazy>("select DISTINCT p.* from sysPermission p inner join [sysRole-sysPermission] rp on p.id=rp.sysPermissionId where rp.sysRoleId=@roleId   order by sort").ToArray();
            DataCached[key] = list;
            Lazy(list);
            return list;
        }
        public SysPermission[] List(int roleId,int permissionId)
        {
            string key = GetCacheKey(typeof(SysPermission), new object[]{roleId,permissionId});
            SysPermissionLazy[] list = DataCached.GetItem<SysPermissionLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            string where = null;
            if (permissionId == 0)
            {
                where = " and (parentId=0 or parentId is null)";
            }
            else {
                where = " and parentId="+permissionId;
            }
            Executor.addParameter("@roleId", roleId);
            list = Executor.executeForListObject<SysPermissionLazy>("select DISTINCT p.* from sysPermission p inner join [sysRole-sysPermission] rp on p.id=rp.sysPermissionId where rp.sysRoleId=@roleId" + where + "   order by sort").ToArray();
            DataCached[key] = list;
            Lazy(list);
            return list;
        }
        public SysPermission[] ListByUserId(int userId) {

            string key = GetCacheKey(typeof(SysPermission), new object[] { userId });
            SysPermissionLazy[] list = DataCached.GetItem<SysPermissionLazy[]>(key);
            if (list != null)
            {
                return list;
            }

            Executor.addParameter("@userId", userId);
            list = Executor.executeForListObject<SysPermissionLazy>(@"select DISTINCT p.* from     sysPermission p  
inner join [sysRole-sysPermission] rp on p.id=rp.sysPermissionId
inner join [sysUser-sysRole] ur on ur.sysRoleId=rp.sysRoleId   and  ur.sysUserId=@userId     order by sort")
                                                        .ToArray();
            DataCached[key] = list;
            Lazy(list);
            return list;
        }
        public SysPermission[] ListByUserId(int userId,int parentId=0) {
            string key = GetCacheKey(typeof(SysPermission), new object[]{ userId,parentId});
            SysPermissionLazy[] list = DataCached.GetItem<SysPermissionLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            StringBuilder sb = new StringBuilder();
            if (parentId == 0)
            {
                sb.Append(" parentId=0 or parentId is null ");
            }
            else {
                sb.AppendFormat(" parentId={0} ",parentId);
            }
            Executor.addParameter("@userId", userId);
            list = Executor.executeForListObject<SysPermissionLazy>(@"select DISTINCT p.* from     sysPermission p  
inner join [sysRole-sysPermission] rp on p.id=rp.sysPermissionId and (p.levelNum is null or p.levelNum<2)
inner join [sysUser-sysRole] ur on ur.sysRoleId=rp.sysRoleId   and  ur.sysUserId=@userId where  " + sb.ToString() + "   order by sort")
                                                        .ToArray();
            DataCached[key] = list;
            Lazy(list);
            return list;
        }
        public SysPermission Info(int id) {
            string key = GetCacheKey(typeof(SysPermission),id);
            SysPermissionLazy obj = DataCached.GetItem<SysPermissionLazy>(key);
            if (obj!=null)
            {
                return obj;
            }
            Executor.addParameter("@id",id);
            obj = Executor.executeForSingleObject<SysPermissionLazy>("select * from sysPermission where id=@id  order by sort");
            Lazy(obj);
            DataCached[key] = obj;
            return obj;
            
        }
        public SysPermission Info(string code)
        {
            string key = GetCacheKey(typeof(SysPermission), code);
            SysPermissionLazy obj = DataCached.GetItem<SysPermissionLazy>(key);
            if (obj != null)
            {
                return obj;
            }
            Executor.addParameter("@code", code);
            obj = Executor.executeForSingleObject<SysPermissionLazy>("select * from sysPermission where code=@code");
            Lazy(obj);
            DataCached[key] = obj;
            return obj;

        }
        void Lazy(SysPermissionLazy[]list) {
            foreach (SysPermissionLazy item in list)
            {
                Lazy(item);
            }
        }
        void Lazy(SysPermissionLazy obj)
        {
            if (obj==null)
            {
                return;
            }
            obj.SysRoleListLazy = new Func<int, SysRole[]>(GetInstance<SysRoleData>().List);
            obj.ParentPermissionInfoLazy = new Func<int, SysPermission>(Info);
            obj.ChildrenPermissionListLazy = new Func<int, SysPermission[]>(List);
        }
        /// <summary>
        /// false移除，true添加
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="permissionId"></param>
        /// <returns></returns>
        public bool SwitchRolePermission(int roleId, int permissionId) {
            object[,] arg=new object[,]{
                {"@roleId",roleId},{"@sysPermissionId",permissionId},
            };
            if(Convert.ToInt32( Executor.executeSclar("select count(1) from [sysRole-sysPermission] where sysRoleId=@roleId and sysPermissionId=@sysPermissionId",System.Data.CommandType.Text,arg))>0){
                Executor.executeNonQuery("delete from  [sysRole-sysPermission] where sysRoleId=@roleId and sysPermissionId=@sysPermissionId", System.Data.CommandType.Text, arg);
                removeCache(typeof(SysRole));
                removeCache(typeof(SysPermission));
                return false;
            }
            else
            {
                Executor.executeNonQuery("insert into [sysRole-sysPermission](sysRoleId,sysPermissionId)values(@roleId,@sysPermissionId)", System.Data.CommandType.Text, arg);
                removeCache(typeof(SysRole));
                removeCache(typeof(SysPermission));

                return true;
            }
          
        }

        public void Add(SysPermission obj)
        {
            int stuId = Convert.ToInt32(Executor.executeSclar(@"INSERT INTO [sysPermission]
           (

                code,

                name,

                remark,

                parentId,

                fullParent,

                levelNum,

                sort,

                action

            )
     VALUES
   (

	            @code,

	            @name,

	            @remark,

	            @parentId,

	            @fullParent,

	            @levelNum,

	            @sort,

	            @action

    );select SCOPE_IDENTITY()", System.Data.CommandType.Text, new object[,]{

	        {"@code",obj.Code},

	        {"@name",obj.Name},

	        {"@remark",obj.Remark},

	        {"@parentId",obj.ParentId},

	        {"@fullParent",obj.FullParent},

	        {"@levelNum",obj.LevelNum},

	        {"@sort",obj.Sort},

	        {"@action",obj.Action}

            }));
            removeCache(typeof(SysPermission));
        }

        public void Update(SysPermission obj)
        {
            Executor.executeNonQuery(@"update [sysPermission] set 

                code=@code,

                name=@name,

                remark=@remark,

                parentId=@parentId,

                fullParent=@fullParent,

                levelNum=@levelNum,

                sort=@sort,

                action=@action

         where id=@id", System.Data.CommandType.Text, new object[,]{
            {"@id",obj.Id},

	        {"@code",obj.Code},

	        {"@name",obj.Name},

	        {"@remark",obj.Remark},

	        {"@parentId",obj.ParentId},

	        {"@fullParent",obj.FullParent},

	        {"@levelNum",obj.LevelNum},

	        {"@sort",obj.Sort},

	        {"@action",obj.Action}

            });
            removeCache(typeof(SysPermission));
        }
        public void Delete(SysPermission obj)
        {
            Executor.executeNonQuery("delete sysPermission where id=@id", System.Data.CommandType.Text, new object[,]{                 
                {"@id",obj.Id}
            });
            removeCache(typeof(SysPermission));
        }

    }
}
