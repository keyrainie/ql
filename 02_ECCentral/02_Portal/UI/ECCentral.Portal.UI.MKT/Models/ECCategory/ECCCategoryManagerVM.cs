using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace ECCentral.Portal.UI.MKT.Models.ECCategory
{
    public class ECCCategoryManagerVM : ModelBase
    {
        public List<KeyValuePair<ECCCategoryManagerStatus?, string>> CategoryStatusList { get; set; }
        public List<KeyValuePair<ECCCategoryManagerType?, string>> CategoryTypeList { get; set; }

        public ECCCategoryManagerVM()
        {
            this.CategoryStatusList = EnumConverter.GetKeyValuePairs<ECCCategoryManagerStatus>(EnumConverter.EnumAppendItemType.All);
            this.CategoryTypeList = EnumConverter.GetKeyValuePairs<ECCCategoryManagerType>(EnumConverter.EnumAppendItemType.None);
        }
        private int? category1SysNo;
        public int? Category1SysNo
        {
            get { return category1SysNo; }
            set { SetValue("Category1SysNo", ref category1SysNo, value); }
        }
        private int? category2SysNo;
        public int? Category2SysNo
        {
            get { return category2SysNo; }
            set { SetValue("Category2SysNo", ref category2SysNo, value); }
        }
        private ECCCategoryManagerType type = ECCCategoryManagerType.ECCCategoryType1;
        public ECCCategoryManagerType Type
        {
            get { return type; }
            set { SetValue("Type", ref type, value); }
        }

        /// <summary>
        /// 类别ID
        /// </summary>
        private string _categoryID;
        public string CategoryID
        {
            get { return _categoryID; }
            set { SetValue("CategoryID", ref _categoryID, value); }
        }
        /// <summary>
        /// 类别名称
        /// </summary>
        public string CategoryName { get; set; }

        /// <summary>
        /// 类别状态
        /// </summary>
        public ECCCategoryManagerStatus? Status { get; set; }

        private int? sysNo;
        public int? SysNo
        {
            get { return sysNo; }
            set { SetValue("SysNo", ref sysNo, value); }
        }
    }
}
