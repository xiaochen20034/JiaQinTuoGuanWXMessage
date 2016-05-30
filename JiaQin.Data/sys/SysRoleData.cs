using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
using JiaQin.Entity.Lazy;
namespace JiaQin.Data
{
    public class SysRoleData : IDataExecutorImp
    {
        public SysRole[] List(int userId)
        {
            string key = GetCacheKey(typeof(SysRole),  userId);
            SysRoleLazy[] list = DataCached.GetItem<SysRoleLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@userId", userId);
            list = Executor.executeForListObject<SysRoleLazy>("select * from sysRole r inner join  [sysUser-sysRole]  ur on r.id=ur.sysRoleId where ur.sysUserId=@userId").ToArray();
            Lazy(list);
            DataCached[key] = list;
            return list;
        }
        public SysRole[] List() {
            string key = GetCacheKey(typeof(SysRole));
            SysRoleLazy[] list = DataCached.GetItem<SysRoleLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            list = Executor.executeForListObject<SysRoleLazy>("select * from sysRole").ToArray();
            Lazy(list);
            DataCached[key] = list;
            return list;
        }

        public SysRole[] ListByPermissionId(int permissionId)
        {
            string key = GetCacheKey(typeof(SysRole), permissionId);
            SysRoleLazy[] list = DataCached.GetItem<SysRoleLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@permissionId", permissionId);
            list = Executor.executeForListObject<SysRoleLazy>("select * from sysRole r inner join  [sysRole-sysPermission]  rp on r.id=rp.sysRoleId where rp.sysPermissionId=@permissionId").ToArray();
            Lazy(list);
            DataCached[key] = list;
            return list;
        }
        public SysRole Info(string code) {
            string key = GetCacheKey(typeof(SysRole), code);
            SysRoleLazy obj = DataCached.GetItem<SysRoleLazy>(key);
            if (obj != null)
            {
                return obj;
            }
            Executor.addParameter("@code", code);
            obj = Executor.executeForSingleObject<SysRoleLazy>("select * from sysRole where code=@code");
            Lazy(obj);
            DataCached[key] = obj;
            return obj;

        }
        public SysRole Info(int id)
        {
            string key = GetCacheKey(typeof(SysRole), id);
            SysRoleLazy obj = DataCached.GetItem<SysRoleLazy>(key);
            if (obj != null)
            {
                return obj;
            }
            Executor.addParameter("@id", id);
            obj = Executor.executeForSingleObject<SysRoleLazy>("select * from sysRole where id=@id");
            Lazy(obj);
            DataCached[key] = obj;
            return obj;

        }
        void Lazy(SysRoleLazy[] list)
        {
            foreach (SysRoleLazy item in list)
            {
                Lazy(item);
            }
        }
        void Lazy(SysRoleLazy obj)
        {
            if (obj==null)
            {
                return;
            }
            obj.SysPermissionListLazy = new Func<int, SysPermission[]>(GetInstance<SysPermissionData>().ListByRoleId);
            obj.SysUserListLazy = new Func<int, SysUser[]>(GetInstance<SysUserData>().ListByRoleId);
        }
        public void Add(SysRole role) {
            Executor.executeNonQuery("insert into sysRole(code,name,parentId,fullParent)values(@code,@name,@parentId,@fullParent)", System.Data.CommandType.Text, new object[,] { 
                {"@code",role.Code},{"@name",role.Name},{"@parentId",role.ParentId},{"@fullParent",role.FullParent}
            });
            removeCache(typeof(SysRole));
        }
        public void Update(SysRole role)
        {
            Executor.executeNonQuery(" update sysRole set name=@name where id=@id", System.Data.CommandType.Text, new object[,] { 
                {"@name",role.Name},{"@id",role.Id}
            });
            removeCache(typeof(SysRole));
        }
        public void Delete(int roleId)
        {
            Executor.executeNonQuery(" delete from sysRole where id=@id", System.Data.CommandType.Text, new object[,] { 
                {"@id",roleId}
            });
            removeCache(typeof(SysRole));
        }


    }
}
