using System;
using System.Windows;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.IM.Views
{
     [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductCopy
    {
        #region 属性
         private ProductCopyPropertyFacade _facade;
         private ProductCopyPropertyVM _vm;
        #endregion

        #region 初始化加载
        public ProductCopy()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            _vm = new ProductCopyPropertyVM();
            DataContext = _vm;
        }
        #endregion



        #region 页面内按钮处理事件

        #region 界面事件


        private void BtnSaveClick(object sender, RoutedEventArgs e)
        {
           if(_vm.SourceProductSysNo==null)
           {
               Window.Alert(ResProductCopy.Error_SourceInfo, MessageType.Error);
               return;
           }
           if (_vm.TargetProductSysNo == null)
           {
               Window.Alert(ResProductCopy.Error_TargetInfo, MessageType.Error);
               return;
           }
            _facade=new ProductCopyPropertyFacade();
            _facade.ProductCopyProperty(_vm,(obj,arg)=>
                    {
                         if(arg.FaultsHandle())
                         {
                             return;
                         }
                         Window.Alert(ResProductCopy.Info_SaveSuccessfully, MessageType.Information);
                         return;
                    });
        }

        private void BtnCancelClick(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }


        #endregion

        #endregion

        #region 跳转
        #endregion
    }
}
