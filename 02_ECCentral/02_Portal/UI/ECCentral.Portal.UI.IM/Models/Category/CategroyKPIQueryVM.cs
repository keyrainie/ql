using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Collections.Generic;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Resources;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategroyKPIQueryVM : ModelBase
    {

        private int? c1SysNo;

        public int? C1SysNo 
        {
            get { return c1SysNo; }
            set { base.SetValue("C1SysNo", ref c1SysNo, value); }
        }

        private int? c2SysNo;
        public int? C2SysNo 
        {
            get { return c2SysNo; }
            set { base.SetValue("C2SysNo", ref c2SysNo, value); }
        }

        private int? c3SysNo;
        public int? C3SysNo
        {
            get { return c3SysNo; }
            set { base.SetValue("C3SysNo", ref c3SysNo, value); }
        }

        //public int PMUserSysNo { get; set; }
        private int status;

        public int Status 
        {
            get { return status; }
            set { base.SetValue("Status", ref status, value); }
        }

        private CategoryType categoryType = CategoryType.CategoryType2;
        public CategoryType CategoryType {
            get { return categoryType; }
            set { categoryType = value; }
        }

        public List<KeyValuePair<CategoryType?, string>> CategoryTypeList { get; set; }
        public List<KeyValuePair<int, string>> StatusList { get; set; }

        public CategroyKPIQueryVM()
        {
            List<KeyValuePair<int, string>> statusList = new List<KeyValuePair<int, string>>();

            statusList.Add(new KeyValuePair<int, string>(-999, ResCategoryKPIMaintain.SelectTextAll));
            statusList.Add(new KeyValuePair<int, string>(0, ResCategoryKPIMaintain.SelectTextValid));
            statusList.Add(new KeyValuePair<int, string>(-1, ResCategoryKPIMaintain.SelectTextInvalid));

            this.StatusList = statusList;

         List<KeyValuePair<CategoryType?, string>> categoryTypeList = new List<KeyValuePair<CategoryType?, string>>() 
            { new KeyValuePair<CategoryType?, string>(CategoryType.CategoryType2,ResCategoryKPIMaintain.SelectTextCategory2),
            new KeyValuePair<CategoryType?, string>(CategoryType.CategoryType3,ResCategoryKPIMaintain.SelectTextCategory3)};

            CategoryTypeList = categoryTypeList;
        }
    }
}
