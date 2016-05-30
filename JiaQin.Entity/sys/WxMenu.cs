using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity
{
    public class WxMenu
    {
        public int Id { set; get; }
        public int ParentId { set; get; }

        public string Name { set; get; }
        public string Code { set; get; }
        public string Type { set; get; }
        public string Key { set; get; }
        public string Url { set; get; }
        public int Sort { set; get; }
             
        public virtual WxMenu ParentMenu { set; get; }
        public virtual WxMenu[] SubMenuItems { set; get; }

    }
}
