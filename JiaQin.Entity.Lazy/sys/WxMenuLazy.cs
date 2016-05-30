using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Entity.Lazy
{
    public class WxMenuLazy : WxMenu
    {
        public Func<int, WxMenu> ParentMenuLazy;
        public override WxMenu ParentMenu
        {
            get
            {

                if (base.ParentMenu == null && ParentMenuLazy != null && base.ParentId > 0)
                {
                    return ParentMenuLazy(base.ParentId);
                }
                return base.ParentMenu;
            }
            set
            {
                base.ParentMenu = value;
            }
        }
        public Func<int, WxMenu[]> SubMenuItemsLazy;

        public override WxMenu[] SubMenuItems
        {
            get
            {
                if (base.SubMenuItems == null && SubMenuItemsLazy != null)
                {
                    return SubMenuItemsLazy(base.Id);
                }
                return base.SubMenuItems;
            }
            set
            {
                base.SubMenuItems = value;
            }
        }
    }
}
