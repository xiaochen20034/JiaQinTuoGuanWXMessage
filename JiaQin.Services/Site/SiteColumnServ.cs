using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JiaQin.Entity;
using JiaQin.Data;
using Zhyj.Common;
namespace JiaQin.Service
{
    public class SiteColumnServ:AppBase
    {

        /// <summary>
        /// 栏目页面
        /// </summary>
        public void Figure()
        {
            ListByColumnId();
        }
        public void ListByColumnId()
        {
            string ColumnId = Request["ColumnId"];

            int ColumnIdInt = 0;//
            SiteColumnData columnData = dataExecutorImp.GetInstance<SiteColumnData>();
            SiteColumn columnInfo=null;
            if (int.TryParse(ColumnId, out ColumnIdInt) && ColumnIdInt > 0)
            {
                columnInfo = columnData.getSiteColumnInfoById(ColumnIdInt);
            }
            else {
                
                columnInfo = new SiteColumn()
                {
                    ID = 0,
                     Code="00",
                      ParentColumn=0,
                       Name="网站栏目"
                };
            }
            SiteColumn[] columnList = columnData.List(columnInfo.ID,true);

            TemplateData["columnInfo"] = columnInfo;
            TemplateData["columnList"] = columnList;
        }

        public void AddPage()
        {

            string ColumnId = Request["ColumnId"];

            int ColumnIdInt = 0;//
            SiteColumnData columnData = dataExecutorImp.GetInstance<SiteColumnData>();
            SiteColumn columnInfo = null;
            if (int.TryParse(ColumnId, out ColumnIdInt) && ColumnIdInt>0)
            {
                columnInfo = columnData.getSiteColumnInfoById(ColumnIdInt);
            }
            else
            {

                columnInfo = new SiteColumn()
                {
                    ID = 0,
                    Code = "00",
                    ParentColumn = 0,
                    Name = "网站栏目"
                };
            }


            TemplateData["columnInfo"] = columnInfo;
            
        }
        public void AddEvent()
        {
            SiteColumn cls = Request.PackageRequest<SiteColumn>();
            SiteColumnData clsData = dataExecutorImp.GetInstance<SiteColumnData>();
            clsData.Add(cls);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "栏目添加完成";
        }
        public void UpdatePage()
        {
            string ColumnId = Request["ColumnId"];

            int ColumnIdInt = Convert.ToInt32(ColumnId);
            SiteColumnData columnData = dataExecutorImp.GetInstance<SiteColumnData>();
            SiteColumn columnInfo = columnData.getSiteColumnInfoById(ColumnIdInt);
            if (columnInfo.ParentColumnInfo == null)
            {
                columnInfo.ParentColumnInfo= new SiteColumn()
                {
                    ID = 0,
                    Code = "00",
                    ParentColumn = 0,
                    Name = "网站栏目"
                };
            }
            TemplateData["columnInfo"] = columnInfo;
        }
        public void UpdateEvent()
        {
            SiteColumn cls = Request.PackageRequest<SiteColumn>();
            SiteColumnData clsData = dataExecutorImp.GetInstance<SiteColumnData>();
            clsData.Update(cls);

            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "栏目 更新完成";
        }
        public void DeleteEvent()
        {
            this.ID = Convert.ToInt32(Request["ColumnId"]);
            SiteColumnData clsData = dataExecutorImp.GetInstance<SiteColumnData>();
            clsData.Delete(this.ID);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "栏目 删除完成";
        }
    }
}
