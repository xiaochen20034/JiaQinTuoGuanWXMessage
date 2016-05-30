using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using JiaQin.Entity;
using JiaQin.Data;
using Zhyj.Common;
namespace JiaQin.Services
{
    public class DepartmentServ:AppBase
    {
        
        
        public void DepartmentFigure() {
            DepartmentData departData = dataExecutorImp.GetInstance<DepartmentData>();;
            Department [] depList= departData.RootList();
            TemplateData[listDataStr] = depList;
            
        }
        public void AddEvent() {
            Department dep= Request.PackageRequest<Department>();
            DepartmentData depData = dataExecutorImp.GetInstance<DepartmentData>();
            if (depData.Info(dep.Code)!=null)
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.exists;
                TemplateData[JsonKeyValue.tip] = "编码已存在";
                TemplateData[JsonKeyValue.field] = "Code";
                return;
            }
            if (dep.ParentId!=0)
            {
                Department depTemp= depData.Info(dep.ParentId);
                if (depTemp==null)
                {
                    TemplateData[JsonKeyValue.res] = JsonKeyValue.fail;
                    TemplateData[JsonKeyValue.tip] = "父级不存在";
                    return;
                }
                dep.FullParent = depTemp.FullParent + "," + depTemp.Id;
            }
            depData.add(dep);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "添加完成";

        }
        public void UpdatePage() {
            DepartmentData depData = dataExecutorImp.GetInstance<DepartmentData>();
            TemplateData["dep"] = depData.Info(this.ID);
        }
        public void UpdateEvent() {
            Department dep = Request.PackageRequest<Department>();
            DepartmentData depData = dataExecutorImp.GetInstance<DepartmentData>();
            
            depData.update(dep);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertOKRefresh;
            TemplateData[JsonKeyValue.tip] = "编辑完成";

        }
        public void DeleteEvent()
        {
            
            DepartmentData depData = dataExecutorImp.GetInstance<DepartmentData>();

            Department[] depChilden = depData.Info(this.ID).ChildrenDepartmentList;
            if(depChilden!=null && depChilden.Length>0)
            {
                TemplateData[JsonKeyValue.res] = JsonKeyValue.alert;
                TemplateData[JsonKeyValue.tip] = "有子级，不能删除";
            }
            depData.Delete(this.ID);
            TemplateData[JsonKeyValue.res] = JsonKeyValue.alertRefresh;
            TemplateData[JsonKeyValue.tip] = "删除完成";

        }
        public void DepartmentChildrenLi()
        {
            DepartmentData depData = dataExecutorImp.GetInstance<DepartmentData>();
            Department dept = depData.Info(this.ID);
            Department[] deptList = dept.ChildrenDepartmentList;
            TemplateData["dep"] = dept;
            TemplateData[dataStr] = deptList;


        }
    }
}
