using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;

namespace ECCentral.BizEntity.IM
{
   public class RmaPolicyInfo:ICompany,ILanguage
    {

       public int? SysNo {get;set; }
        public string RMAPolicyName
        {
            get;
            set;
        }
     
        public string ECDisplayName
        {
            get;
            set;
        }
       
        public string Priority
        {
            get;
            set;
        }
        public int? ReturnDate
        {
            get;
            set;
        }
       
        public int? ChangeDate
        {
            get;
            set;
        }
       
        public string ECDisplayDesc
        {
            get;
            set;
        }
       
        public string ECDisplayMoreDesc
        {
            get;
            set;
        }
        public IsOnlineRequst? IsOnlineRequest
        {
            get;
            set;
        }
        public RmaPolicyType? RmaType
        {
            get;
            set;
        }
        public RmaPolicyStatus? Status
        {
            get;
            set;
        }
        public UserInfo User { get; set; }

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        //public string DisplayRMAPolicyName
        //{
        //    get
        //    {
        //        return String.Format("{0}[{1}]{2}"
        //            ,Status==RmaPolicyStatus.DeActive ? "*" : String.Empty
        //            ,RmaType)
        //    }
        //}

        #endregion

        #region ILanguage Members

        public string LanguageCode
        {
            get;
            set;
        }

        #endregion
    }
}
