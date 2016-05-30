using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity
{
    public class Department
    {
        public int Id { set; get; }
        public string Code { set; get; }
        public string Name { set; get; }
        public int ParentId { set; get; }
        public string FullParent { set; get; }
        public int sort { set; get; }
        public string Remark { set; get; }

        public int IsJob { set; get; }
        public bool IsJob2 {
            get {
                return IsJob == 1;
            }
        }

        public virtual Department[] ParentDepartmentList { set; get; }
        public virtual Department[] ChildrenDepartmentList { set; get; }
        public virtual SysUser[] UserList { set; get; }
    }
}
