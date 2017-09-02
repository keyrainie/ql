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
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class CategoryTemplateDataVM:ModelBase
    {

        public CategoryTemplateDataVM()
        {
            CategoryPropertyList = new List<CategoryTemplatePropertyVM>();
        }
        private string lastEidtUserName;
        public string LastEidtUserName {
            get { return lastEidtUserName; }
            set { SetValue("LastEidtUserName", ref lastEidtUserName, value); }
        }

        private DateTime? lastEditDate;
        public DateTime? LastEditDate 
        {
            get { return lastEditDate; }
            set { SetValue("LastEditDate", ref lastEditDate, value); }
        }

        private CategoryTemplateVM categoryTemplateProductTitle;
        public CategoryTemplateVM CategoryTemplateProductTitle 
        {
            get { return categoryTemplateProductTitle; }
            set { SetValue("CategoryTemplateProductTitle", ref categoryTemplateProductTitle, value); }
        }
        private CategoryTemplateVM categoryTemplateProductDescription;
        public CategoryTemplateVM CategoryTemplateProductDescription
        {
            get { return categoryTemplateProductDescription; }
            set { SetValue("CategoryTemplateProductDescription", ref categoryTemplateProductDescription, value); }
        }
        private CategoryTemplateVM categoryTemplateProductName;
        public CategoryTemplateVM CategoryTemplateProductName
        {
            get { return categoryTemplateProductName; }
            set { SetValue("CategoryTemplateProductName", ref categoryTemplateProductName, value); }
        }
        private CategoryTemplateVM categoryTemplateWeb;
        public CategoryTemplateVM CategoryTemplateWeb
        {
            get { return categoryTemplateWeb; }
            set { SetValue("CategoryTemplateWeb", ref categoryTemplateWeb, value); }
        }

        private List<CategoryTemplatePropertyVM> categoryPropertyList;
        public List<CategoryTemplatePropertyVM> CategoryPropertyList 
        {
            get { return categoryPropertyList; }
            set { SetValue("CategoryPropertyList", ref categoryPropertyList, value); }
        }
        
    }

    public class CategoryTemplateVM: ModelBase
    {

        public CategoryTemplateVM()
        {
            CategoryTemplatePropertyList = new List<CategoryTemplatePropertyVM>();
        }
        public CategoryTemplateType TemplateType { get; set; }

        public List<CategoryTemplatePropertyVM> CategoryTemplatePropertyList
        {
            get;
            set;
        }
        private string categoryTemplatePropertyDisplay;
        public string CategoryTemplatePropertyDisplay 
        {
            get { return categoryTemplatePropertyDisplay; }
            set { SetValue("CategoryTemplatePropertyDisplay", ref categoryTemplatePropertyDisplay, value); }
        }
    }

    public class CategoryTemplatePropertyVM : ModelBase
    {
        public int? SysNo { get; set; }

        public string PropertyDescription { get; set; }

    }

}
