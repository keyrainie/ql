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

namespace ECCentral.Portal.UI.MKT.Models
{

    public class SelectCategoryPropertyVM:ModelBase
    {

        public SelectCategoryPropertyVM() 
        {
            CategoryPropertyList = new List<CategoryPropertyVM>() { new CategoryPropertyVM() { PropertyDescription = "--请选择--", PropertySysNo = -1 } };
            PropertyValueList = new List<PropertyValueVM>() { new PropertyValueVM() { PropertyValueSysNo = -1, PropertyValueDescription = "--请选择--" } };
        }
         private CategoryPropertyVM categoryProperty;
         public CategoryPropertyVM CategoryProperty 
         {
             get{return categoryProperty;}
             set{SetValue("CategoryProperty",ref categoryProperty,value);}
         }
         private PropertyValueVM propertyValue;
         public PropertyValueVM PropertyValue
         {
             get { return propertyValue; }
             set { SetValue("PropertyValue", ref propertyValue, value); }
         }
         private List<CategoryPropertyVM> categoryPropertyList;
         public List<CategoryPropertyVM> CategoryPropertyList 
         {
             get { return categoryPropertyList; }
             set { SetValue("CategoryPropertyList", ref categoryPropertyList, value); }
         }
         private List<PropertyValueVM> propertyValueList;
         public List<PropertyValueVM> PropertyValueList
         {
             get { return propertyValueList; }
             set { SetValue("PropertyValueList", ref propertyValueList, value); }
         }

         private string inputValue;
         public string InputValue 
         {
             get { return inputValue; }
             set { SetValue("InputValue", ref inputValue, value); }
         }

    }


    public class CategoryPropertyVM : ModelBase
    {
        /// <summary>
        /// 属性SysNo
        /// </summary>
        public int PropertySysNo { get; set; }

        /// <summary>
        /// 属性描述
        /// </summary>
        private string propertyDescription;
        public string PropertyDescription 
        {
            get { return propertyDescription; }
            set { SetValue("PropertyDescription", ref propertyDescription, value); }
        }
    }
    public class PropertyValueVM:ModelBase
    {
        /// <summary>
        /// 属性值SysNo
        /// </summary>
        public int PropertyValueSysNo { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        private string propertyValueDescription;
        public string PropertyValueDescription 
        {
            get { return propertyValueDescription; }
            set { SetValue("PropertyValueDescription", ref propertyValueDescription, value); }
        }

    }
}
