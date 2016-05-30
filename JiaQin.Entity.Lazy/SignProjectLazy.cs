using System;
using System.Collections.Generic;
using System.Text;
using JiaQin.Entity;
namespace JiaQin.Entity.Lazy
{
    /// <summary>
    /// 
    /// </summary>
    public class SignProjectLazy:SignProject
    {
        public Func<int, SignRecord[]> SignRecordListLazy = null;   
		public override SignRecord[] SignRecordList{
               get{
                     if (base.SignRecordList == null && this.SignRecordListLazy != null)
                    {
                          return  this.SignRecordListLazy(base.Id);
                    }
                    return base.SignRecordList ;
              }
              set
              {
                   base.SignRecordList = value;
              }
    }
    }
}