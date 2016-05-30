using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zhyj.Common;
using JiaQin.Entity;
using JiaQin.Data;
using System.IO;
using System.Web;
namespace JiaQin.Service
{
    class VipUserServ : AppBase
    {
        
        public void List() {
            VipUserData vipUserData=dataExecutorImp.GetInstance<VipUserData>();
            TemplateData["vipUserList"]= vipUserData.List(Request["name"],PageSize,PageNum,out rowCount);
            Page();
        }

    }
}
