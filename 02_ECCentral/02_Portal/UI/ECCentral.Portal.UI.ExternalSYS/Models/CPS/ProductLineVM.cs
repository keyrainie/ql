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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.ExternalSYS.Models
{
    public class ProductLineVM : ModelBase
    {
        public ProductLineVM()
        {
            CategoryList = new List<ProductLineCategoryVM>();
        }

        public int SysNo { get; set; }
        private ProductLineCategoryVM category;
        public ProductLineCategoryVM Category 
        {
            get { return category; }
            set { SetValue("Category", ref category, value); }
        }

        private string productLineName;
        [Validate(ValidateType.Required)]
        public string ProductLineName 
        {
            get { return productLineName; }
            set { SetValue("ProductLineName", ref productLineName, value); }
        }
        private string priority;
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Interger)]
        public string Priority 
        {
            get { return priority; }
            set { SetValue("Priority", ref priority, value); }
        }
        private string useScopeDescription;
         [Validate(ValidateType.Required)]
        public string UseScopeDescription 
        {
            get { return useScopeDescription; }
            set { SetValue("UseScopeDescription", ref useScopeDescription, value); }
        }

        private List<ProductLineCategoryVM> categoryList;
        public List<ProductLineCategoryVM> CategoryList 
        {
            get { return categoryList; }
            set { SetValue("CategoryList", ref categoryList, value); }
        }

    }
}
