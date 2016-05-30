using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
using JiaQin.Entity.Lazy;
namespace JiaQin.Data
{
    public class DepartmentData:IDataExecutorImp
    {
        /// <summary>
        /// 用户所属的部门列表
        /// </summary>
        /// <returns></returns>
        public Department[] UserDepartmentList(int userId) {
            string key = GetCacheKey(new Type[] { typeof(Department) ,typeof(SysUser)}, userId);
            Department[] list = DataCached.GetItem<Department[]>(key);
            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@userId", userId);
            list = Executor.executeForListObject<DepartmentLazy>(@"select * from Department d
                        inner join [sysUser-Department] sd on sd.DepartmentId=d.id 
                        where sd.UserId=@userId").ToArray();
            foreach (DepartmentLazy item in list)
            {
                Lazy(item);
            }
            DataCached[key] = list;
            return list;
        }
        public Department[] ChildrenList(int id) {
            string key = GetCacheKey(typeof(Department),id);
            Department[] list = DataCached.GetItem<Department[]>(key);
            if (list!=null)
            {
                return list;
            }
            Executor.addParameter("@id", id);
            list = Executor.executeForListObject<DepartmentLazy>("select * from Department where parentId=@id").ToArray();
            foreach (DepartmentLazy item in list)
            {
                Lazy(item);
            }
            DataCached[key] = list;
            return list;

        }
        public Department[] RootList() {
            string key = GetCacheKey(typeof(Department));
            Department[] list = DataCached.GetItem<Department[]>(key);
            if (list != null)
            {
                return list;
            }
            
            list = Executor.executeForListObject<DepartmentLazy>("select * from Department where parentId=0 or parentId is null ").ToArray();
            foreach (DepartmentLazy item in list)
            {
                Lazy(item);
            }
            DataCached[key] = list;
            return list;
        }
        public Department[] ParentList(int id)
        {
            string key = GetCacheKey(typeof(Department), id);
            DepartmentLazy[] list = DataCached.GetItem<DepartmentLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            Department dep = Info(id);
            if (dep == null)
            {
                return null;
            }
            
            list = Executor.executeForListObject<DepartmentLazy>("select * from Department where id in ("+dep.FullParent+")").ToArray();
            foreach (DepartmentLazy item in list)
            {
                Lazy(item);
            }
            DataCached[key] = list;
            return list;

        }
        public Department[] ChildrenList(string id)
        {
            string key = GetCacheKey(typeof(Department), id);
            DepartmentLazy[] list = DataCached.GetItem<DepartmentLazy[]>(key);
            if (list != null)
            {
                return list;
            }
            Department dep = Info(id);
            if (dep==null)
            {
                return null;
            }
            Executor.addParameter("@id", dep.Id);
            list = Executor.executeForListObject<DepartmentLazy>("select * from Department where parentId=@id").ToArray();
            foreach (DepartmentLazy item in list)
            {
                Lazy(item);
            }
            DataCached[key] = list;
            return list;

        }
        public Department Info(string id)
        {
            string key = GetCacheKey(typeof(DepartmentLazy), id);
            DepartmentLazy dep = DataCached.GetItem<DepartmentLazy>(key);
            if (dep != null)
            {
                return dep;
            }
            Executor.addParameter("@code", id);
            dep = Executor.executeForSingleObject<DepartmentLazy>("select * from department where code=@code");
            Lazy(dep);
            DataCached[key] = dep;
            return dep;
        }
        public Department Info(int id) {
            string key = GetCacheKey(typeof(Department),id);
            DepartmentLazy dep = DataCached.GetItem<DepartmentLazy>(key);
            if (dep!=null)
            {
                return dep;
            }
            Executor.addParameter("@id",id);
            dep = Executor.executeForSingleObject<DepartmentLazy>("select * from department where id=@id");

            Lazy(dep);
            DataCached[key] = dep;
            
            return dep;
        }
        
        void Lazy(DepartmentLazy obj) {
            if (obj==null)
            {
                return;
            }
            obj.ChildrenDepartmentListLazy = new Func<int, Department[]>(ChildrenList);
            obj.ParentDepartmentListLazy = new Func<int, Department[]>(ParentList);
            obj.UserListLazy = new Func<int, SysUser[]>(GetInstance<SysUserData>().ListByDpartmentId);
        }
        public void add(Department obj) {
            Executor.executeNonQuery("insert into Department(Code,Name,parentId,fullParent,sort,remark)values(@Code,@Name,@parentId,@fullParent,@sort,@remark)", System.Data.CommandType.Text,new object[,]{
                {"@Code",obj.Code},{"@Name",obj.Name},{"@parentId",obj.ParentId},
                {"@fullParent",obj.FullParent},{"@sort",obj.sort},{"@remark",obj.Remark}
            });
            removeCache(typeof(Department));
        }
        public void update(Department obj)
        {
            Executor.executeNonQuery("update Department set Code=@code,Name=@Name,remark=@remark where id=@id", System.Data.CommandType.Text, new object[,]{
                {"@Code",obj.Code},{"@Name",obj.Name},{"@remark",obj.Remark},{"@id",obj.Id}
            });
            removeCache(typeof(Department));
        }
        public void Delete(int id) {
            Executor.addParameter("@id",id);
            Executor.executeNonQuery("delete from Department where id=@id");
            removeCache(typeof(Department));
        }
    }
}
