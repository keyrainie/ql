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
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class BrandRecommendedMaintain : UserControl
    {
        public BrandRecommendedVM Brand { get; set; }
        public IDialog Dialog { get; set; }
        //public string DisPlayBrank
        //{
        //    get { return txtBrank.Text; }
        //    set { txtBrank.Text = value; }
        //}
        
        private BrandRecommendedFacade facade;
        public BrandRecommendedMaintain()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(BrandRecommendedMaintain_Loaded);
        }

        void BrandRecommendedMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            facade = new BrandRecommendedFacade();
            this.DataContext = Brand;

            
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            BrandRecommendedVM model = this.DataContext as BrandRecommendedVM;
            //if (model.BrandRank.Split(',').Count() > 20)
            //{
            //    CPApplication.Current.CurrentPage.Context.Window.Alert("编辑失败,至多输入20个推荐品牌", MessageType.Warning);
            //    return;
            //}

            if (model.Sysno == 0)   //新增
            {
                model.Level_No = (int)this.myCategoryList.cbTopCatrory.SelectedValue;
                if (model.Level_No == 0)
                {
                    model.Level_Code = 0;
                    //model.Level_Name = string.Empty;
                }
                else if (model.Level_No == 1)
                {
                    model.Level_Code = (int)this.myCategoryList.cbShow1Catrory.SelectedValue;
                    //model.Level_Name = string.Empty;
                }
                else if (model.Level_No == 2)
                {
                    model.Level_Code = (int)this.myCategoryList.cbShow2Catrory.SelectedValue;
                    //model.Level_Name = string.Empty;
                }
                if (model.Level_No > 0 && model.Level_No < 3 && model.Level_Code == 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("请选择一个类别！", MessageType.Warning);
                    return;
                }
                if (!model.BrandSysNo.HasValue || model.BrandSysNo.Value == 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("请选择一个品牌！", MessageType.Warning);
                    return;
                }
                facade.CreateBrandRecommended(model, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_SettingSuccessful);
                    CloseDialog(DialogResultType.OK);
                });

            }
            else
            {
                if (!model.BrandSysNo.HasValue || model.BrandSysNo.Value == 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("请选择一个品牌！", MessageType.Warning);
                    return;
                }
                facade.UpdateBrandRecommended(model, (obj, arg) =>
                {
                    if (arg.FaultsHandle())
                    {
                        return;
                    }
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful);
                    CloseDialog(DialogResultType.OK);

                });
            }
        }
    }
}
