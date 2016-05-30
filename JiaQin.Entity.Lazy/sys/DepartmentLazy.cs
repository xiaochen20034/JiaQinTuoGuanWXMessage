using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    public class DepartmentLazy:Department
    {
        public Func<int, Department[]> ParentDepartmentListLazy;
        public override Department[] ParentDepartmentList
        {
            get
            {
                if (base.ParentDepartmentList==null && ParentDepartmentListLazy!=null)
                {
                    return ParentDepartmentListLazy(base.Id);
                }
                return base.ParentDepartmentList;
            }
            set
            {
                base.ParentDepartmentList = value;
            }
        }
        public Func<int, Department[]> ChildrenDepartmentListLazy;
        public override Department[] ChildrenDepartmentList
        {
            get
            {
                if (base.ChildrenDepartmentList == null && ChildrenDepartmentListLazy != null)
                {
                    return ChildrenDepartmentListLazy(base.Id);
                }
                return base.ChildrenDepartmentList;
            }
            set
            {
                base.ChildrenDepartmentList = value;
            }
        }

        public Func<int, SysUser[]> UserListLazy;
        public override SysUser[] UserList
        {
            get
            {
                if (base.UserList==null && UserListLazy!=null)
                {
                    return UserListLazy(this.Id);
                }
                return base.UserList;
            }
            set
            {
                base.UserList = value;
            }
        }
    }
}
