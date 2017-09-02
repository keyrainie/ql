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
using System.Windows.Navigation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.Portal.UI.MKT.Models.Floor;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.BizEntity.MKT.Floor;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class FloorSectionMaintain : PageBase
    {
        private FloorFacade ServiceFacade;

        public FloorVM FloorVM { get; set; }

        public List<FloorSectionVM> FloorSectionListVM
        {
            get { return SectionTagResult.ItemsSource as List<FloorSectionVM>; }
            set { SectionTagResult.ItemsSource = value; }
        }

        public FloorSectionMaintain()
        {
            InitializeComponent();
            FloorSectionListVM = new List<FloorSectionVM>();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            ServiceFacade = new FloorFacade(this);
            var floorSysNo = Request.Param;

            ServiceFacade.GetFloorMaster(floorSysNo, (objMaster, argsMaster) =>
            {
                if (argsMaster.FaultsHandle()) return;
                FloorVM = argsMaster.Result.Convert<FloorMaster, FloorVM>();
                tlCon.Header = string.Format("{0}—{1}", FloorVM.FloorName, ResFloorMaintain.Expander_MaintainTitle);

                ServiceFacade.GetFloorSectionList(floorSysNo, (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    var sectionList = EntityConverter<List<FloorSection>, List<FloorSectionVM>>.Convert(args.Result);
                    FloorSectionListVM = sectionList;
                    ucFloorSection.FloorSectionMaintain = this;
                    if (sectionList.Count > 0)
                    {
                        var defaultSection = sectionList.First();
                        defaultSection.IsChecked = true;
                        TagChecedBindData(defaultSection);
                    }
                });
            });
        }

        private void RdoSectionTag_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton rdoSelect = sender as RadioButton;
            var floorSection = rdoSelect.DataContext as FloorSectionVM;
            TagChecedBindData(floorSection);
        }

        private void TagChecedBindData(FloorSectionVM floorSection)
        {
            ucFloorSection.CurrentVM = floorSection.DeepCopy();
            ServiceFacade.GetFloorSectionItemList(floorSection.SysNo.ToString(), (obj, args) =>
            {
                var sectionItemList = args.Result;
                ucFloorSectionData.CurrentVM = floorSection;
                ucFloorSectionData.ProductListVM = new List<FloorSectionProductVM>();
                ucFloorSectionData.BannerListVM = new List<FloorSectionBannerVM>();
                ucFloorSectionData.BrandListVM = new List<FloorSectionBrandVM>();
                ucFloorSectionData.TextLinkListVM = new List<FloorSectionLinkVM>();

                sectionItemList.ForEach(item =>
                {
                    switch (item.ItemType)
                    {
                        case FloorItemType.Product:
                            var productVM = EntityConverter<FloorItemProduct, FloorSectionProductVM>.Convert((FloorItemProduct)item.ItemProudct, (s, t) =>
                            {
                                t.SysNo = item.SysNo;
                                t.FloorMasterSysNo = item.FloorMasterSysNo;
                                t.FloorSectionSysNo = item.FloorSectionSysNo;
                                t.Priority = item.Priority;
                                t.ItemType = item.ItemType;
                                t.ItemPosition = item.ItemPosition;
                                t.IsSelfPage = item.IsSelfPage;
                            });
                            ucFloorSectionData.ProductListVM.Add(productVM);
                            ucFloorSectionData.productResult.ItemsSource = ucFloorSectionData.ProductListVM;
                            break;
                        case FloorItemType.Banner:
                            var bannerVM = EntityConverter<FloorItemBanner, FloorSectionBannerVM>.Convert((FloorItemBanner)item.ItemBanner, (s, t) =>
                            {
                                t.SysNo = item.SysNo;
                                t.FloorMasterSysNo = item.FloorMasterSysNo;
                                t.FloorSectionSysNo = item.FloorSectionSysNo;
                                t.Priority = item.Priority;
                                t.ItemType = item.ItemType;
                                t.ItemPosition = item.ItemPosition;
                                t.IsSelfPage = item.IsSelfPage;
                            });
                            ucFloorSectionData.BannerListVM.Add(bannerVM);
                            ucFloorSectionData.BannerResult.ItemsSource = ucFloorSectionData.BannerListVM;
                            break;
                        case FloorItemType.Brand:
                            var brandVM = EntityConverter<FloorItemBrand, FloorSectionBrandVM>.Convert((FloorItemBrand)item.ItemBrand, (s, t) =>
                            {
                                t.SysNo = item.SysNo;
                                t.FloorMasterSysNo = item.FloorMasterSysNo;
                                t.FloorSectionSysNo = item.FloorSectionSysNo;
                                t.Priority = item.Priority;
                                t.ItemType = item.ItemType;
                                t.ItemPosition = item.ItemPosition;
                                t.IsSelfPage = item.IsSelfPage;
                            });
                            ucFloorSectionData.BrandListVM.Add(brandVM);
                            ucFloorSectionData.BrandResult.ItemsSource = ucFloorSectionData.BrandListVM;
                            break;
                        case FloorItemType.TextLink:
                            var textLinkVM = EntityConverter<FloorItemTextLink, FloorSectionLinkVM>.Convert((FloorItemTextLink)item.ItemTextLink, (s, t) =>
                            {
                                t.SysNo = item.SysNo;
                                t.FloorMasterSysNo = item.FloorMasterSysNo;
                                t.FloorSectionSysNo = item.FloorSectionSysNo;
                                t.Priority = item.Priority;
                                t.ItemType = item.ItemType;
                                t.ItemPosition = item.ItemPosition;
                                t.IsSelfPage = item.IsSelfPage;
                            });
                            ucFloorSectionData.TextLinkListVM.Add(textLinkVM);
                            ucFloorSectionData.TextLinkResult.ItemsSource = ucFloorSectionData.TextLinkListVM;
                            break;
                    }
                });
            });
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btnTemp = sender as HyperlinkButton;
            var sectionVM = btnTemp.DataContext as FloorSectionVM;
            Window.Confirm(ResFloorMaintain.Info_ConfirmDelete, (d, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    ServiceFacade.DeleteFloorSection(sectionVM.SysNo.Value, (obj, cargs) =>
                    {
                        if (cargs.FaultsHandle()) return;
                        if (sectionVM.IsChecked)
                        {
                            ucFloorSection.CurrentVM = new FloorSectionVM();
                            ucFloorSectionData.ProductListVM = new List<FloorSectionProductVM>();
                            ucFloorSectionData.BannerListVM = new List<FloorSectionBannerVM>();
                            ucFloorSectionData.BrandListVM = new List<FloorSectionBrandVM>();
                            ucFloorSectionData.TextLinkListVM = new List<FloorSectionLinkVM>();
                            ucFloorSectionData.CurrentVM = null;
                        }
                        FloorSectionListVM.Remove(sectionVM);
                        SectionTagResult.ItemsSource = FloorSectionListVM;
                    });
                }
            });
        }

        //private void ButtonSave_Click(object sender, RoutedEventArgs e)
        //{
        //    //保存当前所有标签配置
        //}
    }
}
