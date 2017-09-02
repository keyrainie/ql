using System;
using System.Windows;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.MKT.UserControls;
using System.Windows.Controls;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Globalization;
using System.Threading;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class BrandRecommended : PageBase
    {
        BrandRecommendedQueryVM model;
        public BrandRecommended()
        {
            InitializeComponent();
            this.BrandRecommendedResult.LoadingDataSource += new EventHandler<LoadingDataEventArgs>(BrandRecommendedResult_LoadingDataSource);
        }
        private CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }
        void cbTopCatrory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            model = new BrandRecommendedQueryVM();
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            model.BrandType = (int)myCategoryList.cbTopCatrory.SelectedValue;

            if (model.BrandType == 0)
            {
                model.LevelCodeParent = -1;
                model.LevelCode = -1;
                //BrandRecommendedResult.Columns[2].Visibility = Visibility.Collapsed;
                //BrandRecommendedResult.Columns[3].Visibility = Visibility.Collapsed;
            }
            else if (model.BrandType == 1)
            {
                BrandRecommendedResult.Columns[2].Visibility = Visibility.Visible;
                BrandRecommendedResult.Columns[3].Visibility = Visibility.Collapsed;
                model.LevelCode = (int)myCategoryList.cbShow1Catrory.SelectedValue;
                model.LevelCodeParent = -1;
            }
            else if (model.BrandType == 2)
            {
                BrandRecommendedResult.Columns[2].Visibility = Visibility.Visible;
                BrandRecommendedResult.Columns[3].Visibility = Visibility.Visible;
                model.LevelCodeParent = myCategoryList.cbShow1Catrory.SelectedValue == null ? -1 : (int)myCategoryList.cbShow1Catrory.SelectedValue;
                model.LevelCode = myCategoryList.cbShow2Catrory.SelectedValue == null ? -1 : (int)myCategoryList.cbShow2Catrory.SelectedValue;            
            }

            //model.LevelCodeParent = myCategoryList.cbShow1Catrory.SelectedValue==null? -1: (int)myCategoryList.cbShow1Catrory.SelectedValue;
            //model.LevelCode =myCategoryList.cbShow2Catrory.SelectedValue==null?-1: (int)myCategoryList.cbShow2Catrory.SelectedValue;
            ////列根据条件的不同变化 
            //if ((int)myCategoryList.cbTopCatrory.SelectedValue == 2)
            //{
            //    BrandRecommendedResult.Columns[3].Visibility = Visibility.Visible;
            //}
            //else
            //{
            //    model.LevelCode = (int)myCategoryList.cbShow1Catrory.SelectedValue;
            //    BrandRecommendedResult.Columns[3].Visibility = Visibility.Collapsed;
            //}



            this.BrandRecommendedResult.Bind();
        }

        void BrandRecommendedResult_LoadingDataSource(object sender, LoadingDataEventArgs e)
        {
            BrandRecommendedFacade facade = new BrandRecommendedFacade();
            facade.GetCategoryRelatedByQuery(model, e.PageSize, e.PageIndex, e.SortField, (obj, arg) =>
            {
                this.BrandRecommendedResult.ItemsSource = arg.Result.Rows;
                this.BrandRecommendedResult.TotalCount = arg.Result.TotalCount;
            });
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            dynamic d = this.BrandRecommendedResult.SelectedItem as dynamic;
            BrandRecommendedVM model = new BrandRecommendedVM() { Sysno = d.SysNo, BrandSysNo = d.BrandSysNo, Level_Name = d.Level_Name, Level_No = d.Level_No, Level_Code = d.Level_Code };
            BrandRecommendedMaintain maintain = new BrandRecommendedMaintain();
            maintain.Brand = model;
            maintain.myCategoryList.Level_No = model.Level_No;
            maintain.myCategoryList.Level_Code = model.Level_Code;
            maintain.myCategoryList.IsEnabled = false;
            maintain.ucBrandPicker.SelectedBrandName = d.BrandName;
            maintain.Dialog = Window.ShowDialog(ResBrandRecommended.ResourceManager.GetString("UpdateReBrand", CurrentCulture), maintain, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    BrandRecommendedResult.Bind();
                }
            }, new Size(560, 400));
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            BrandRecommendedVM model = new BrandRecommendedVM() { Sysno = 0, Level_No = 0, Level_Code = 0 };
            BrandRecommendedMaintain maintain = new BrandRecommendedMaintain();
            maintain.Brand = model;
            maintain.myCategoryList.IsEnabled = true;
            maintain.Dialog = Window.ShowDialog(ResBrandRecommended.ResourceManager.GetString("UpdateReBrand", CurrentCulture), maintain, (s, args) =>
            {
                if (args.DialogResult == Newegg.Oversea.Silverlight.Controls.Components.DialogResultType.OK)
                {
                    BrandRecommendedResult.Bind();
                }
            }, new Size(500, 300));
        }


    }
}
