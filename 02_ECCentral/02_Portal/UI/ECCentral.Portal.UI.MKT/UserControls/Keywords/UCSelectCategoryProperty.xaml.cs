using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Models;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCSelectCategoryProperty : UserControl
    {
        public int? Category3SysNo { private get; set; }
        private ProductKeywordsQueryFacade facade;
        private SelectCategoryPropertyVM model;

        //向外部公开属性
        public int CategoryPropertySysNo { get { return model.CategoryProperty.PropertySysNo; } }
        public int PropertyValueSysNo { get { return model.PropertyValue.PropertyValueSysNo; } }
         public string InputValue { get { return model.InputValue; }}
        public UCSelectCategoryProperty()
        {
            InitializeComponent();
            this.Loaded += (sender, e) => 
            {
                model = new SelectCategoryPropertyVM();
                facade = new ProductKeywordsQueryFacade(CPApplication.Current.CurrentPage);
               
                model.CategoryProperty = (from p in model.CategoryPropertyList where p.PropertySysNo == -1 select p).FirstOrDefault(); //默认绑定请选择
                model.PropertyValue = (from p in model.PropertyValueList where p.PropertyValueSysNo == -1 select p).FirstOrDefault();//默认绑定请选择
                this.DataContext = model;
            };
            this.cboProperty.SelectionChanged += (sender, e) =>
            {
                //获取属性值
                if (model.CategoryProperty != null)
                {
                    if (model.CategoryProperty.PropertySysNo == -1)
                    {
                        model.InputValue = null;
                        txtInput.IsEnabled = false;
                    }
                    else
                    {
                        txtInput.IsEnabled = true;   
                    }

                    facade.GetPropertyValueByPropertySysNo(model.CategoryProperty.PropertySysNo, (obj, arg) =>
                    {
                        if (arg.FaultsHandle())
                        {
                            return;
                        }
                        List<PropertyValueVM> tempData = new List<PropertyValueVM>() { new PropertyValueVM() { PropertyValueSysNo = -1, PropertyValueDescription = "--请选择--" } };
                        foreach (var item in arg.Result.Rows)
                        {
                            tempData.Add(new PropertyValueVM() { PropertyValueSysNo = item.ValueSysNo, PropertyValueDescription = item.ValueDescription });
                        }
                        model.PropertyValueList = tempData;
                        model.PropertyValue = (from p in model.PropertyValueList where p.PropertyValueSysNo == -1 select p).FirstOrDefault();//默认绑定请选择
                    });
                }
            };
         
        }
        public void BindData()
        {
            //获取属性
            facade.GetPropertyByCategory3SysNo(Category3SysNo ?? 0, (obj, arg) =>
            {
                if (arg.FaultsHandle())
                {
                    return;
                }
                List<CategoryPropertyVM> tempData = new List<CategoryPropertyVM>() { new CategoryPropertyVM() { PropertyDescription = "--请选择--", PropertySysNo = -1 } };
                foreach (var item in arg.Result.Rows)
                {
                    tempData.Add(new CategoryPropertyVM() { PropertySysNo = item.PropertySysNo, PropertyDescription = item.PropertyDescription });
                }
                model.CategoryPropertyList = tempData;
                model.CategoryProperty = (from p in model.CategoryPropertyList where p.PropertySysNo == -1 select p).FirstOrDefault(); //默认绑定请选择
                model.PropertyValue = (from p in model.PropertyValueList where p.PropertyValueSysNo == -1 select p).FirstOrDefault();//默认绑定请选择
                this.DataContext = model;
            });
        }
    }
}
