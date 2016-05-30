using JiaQin.Entity;
using JiaQin.Entity.Lazy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JiaQin.Data
{
    public class WxMenuData : IDataExecutorImp
    {
        /// <summary>
        /// 获取父级 parentId 为id的数据 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public WxMenu[] List(int id)
        {
            string key = GetCacheKey(new Type[]{
                            typeof(WxMenu)
                     }, new object[] { id });


            WxMenuLazy[] list = DataCached.GetItem<WxMenuLazy[]>(key);

            if (list != null)
            {
                return list;
            }
            Executor.addParameter("@parentId", id);
            list = Executor.executeForListObject<WxMenuLazy>(" select * from  wxMenu where parentId=@parentId   order by sort,id").ToArray();

            for (int i = 0; i < list.Length; i++)
            {
                this.Lazy(list[i]);
            }

            DataCached[key] = list;

            return list;
        }
        public WxMenu getWxMenuInfo(int Id)
        {
            string key = GetCacheKey(typeof(WxMenu), Id);

            WxMenuLazy obj = DataCached.GetItem<WxMenuLazy>(key);

            if (obj != null)
            {
                return obj;
            }

            Executor.addParameter("@id", Id);
            obj = Executor.executeForSingleObject<WxMenuLazy>("select * from wxMenu where id=@id");
            if (obj == null)
            {
                return null;
            }
            this.Lazy(obj);
            DataCached[key] = obj;
            return obj;

        }

        void Lazy(WxMenuLazy obj)
        {
            if (obj == null)
            {
                return;
            }
            obj.ParentMenuLazy = new Func<int, WxMenu>(getWxMenuInfo);
            obj.SubMenuItemsLazy = new Func<int, WxMenu[]>(List);
        }

        public void Add(WxMenu menu) {
            
            Executor.executeNonQuery("insert into  wxMenu(code,[name],[type],[key],[url],parentId,sort)values(@code,@name,@type,@key,@url,@parentId,@sort)",System.Data.CommandType.Text,new object[,]{
                {"@code",menu.Code},
                {"@name",menu.Name},
                {"@type",menu.Type},
                {"@key",menu.Key},
                {"@url",menu.Url},
                {"@parentId",menu.ParentId},{"@sort",menu.Sort}
            });
            removeCache(typeof(WxMenu));
        }

        public void Update(WxMenu menu)
        {
            Executor.executeNonQuery("update  wxMenu set [name]=@name,[type]=@type,[key]=@key,[url]=@url,parentId=@parentId,[sort]=@sort where id=@id", System.Data.CommandType.Text, new object[,]{                
                {"@name",menu.Name},
                {"@type",menu.Type},
                {"@key",menu.Key},
                {"@url",menu.Url},
                {"@parentId",menu.ParentId},
                {"@sort",menu.Sort},
                {"@id",menu.Id}
            });
            removeCache(typeof(WxMenu));
        }
        /// <summary>
        /// 删除菜单，会连同子菜单一起删除
        /// </summary>
        /// <param name="id"></param>
        public void Delete(int id)
        {
            Executor.executeNonQuery("delete wxMenu   where id=@id or parentId=@id", System.Data.CommandType.Text, new object[,]{                                
                {"@id",id}
            });
            removeCache(typeof(WxMenu));
        }


    }
}
