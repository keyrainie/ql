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
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.PO.Models;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.UI.PO.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Inventory;
using System.Windows.Data;
using ECCentral.QueryFilter.Inventory;
using ECCentral.Portal.Basic.Components.UserControls.StockPicker;
using ECCentral.Portal.UI.PO.Facades;
using ECCentral.Portal.Basic;
using System.ComponentModel;
using ECCentral.Portal.Basic.Components.UserControls;
using ECCentral.BizEntity.PO.Vendor;

namespace ECCentral.Portal.UI.PO.UserControls
{
    public partial class UCCommissionTemplateMaintain : UserControl
    {
        public IDialog Dialog { get; set; }
        public bool EditFlag;
        public bool IsDeliveryControlHide;
        private VendorCommissionInfoVM vm;
        private CommissionRuleTemplateInfo EditInfo;
        private dynamic AllBrand;
        public int SysNo { get; set; }

        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }
        public VendorFacade vendorFacade;

        public UCCommissionTemplateMaintain()
        {
            InitializeComponent();
            vm = new VendorCommissionInfoVM();
            this.DataContext = vm;
            vendorFacade = new VendorFacade(CPApplication.Current.CurrentPage);
            this.Loaded += UCCommissionTemplateMaintain_Loaded;
        }

        void UCCommissionTemplateMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            if (SysNo == 0)
            {
                //加载所有类别
                new OtherDomainDataFacade().QueryAllCategory((obj, result) =>
                {
                    if (!result.FaultsHandle() && result.Result != null)
                    {
                        this.tvCategory.IsSpeardToParent = true;
                        this.tvCategory.Nodes = TransferCategoryToNodes(result.Result); ;
                        this.tvCategory.BuildTreeByData();
                    }
                });
                //加载所有品牌
                new OtherDomainDataFacade().QueryAllBrand((obj, result) =>
                {
                    AllBrand = result.Result;
                    BindBrandTree();
                });
            }
            else
            {
                this.tvBrand.Visibility = System.Windows.Visibility.Collapsed;
                this.tvCategory.Visibility = System.Windows.Visibility.Collapsed;
                this.labBrand.Visibility = System.Windows.Visibility.Collapsed;
                this.labCategory.Visibility = System.Windows.Visibility.Collapsed;
                vendorFacade.LoadCommissionRuleTemplate(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    EditInfo = args.Result;
                    vm = EntityConverter<CommissionRuleTemplateInfo, VendorCommissionInfoVM>.Convert(EditInfo);
                    VendorStagedSaleRuleEntity vssrEntity = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.XmlDeserialize<VendorStagedSaleRuleEntity>(EditInfo.SalesRule);
                    vm.SaleRuleEntity = EntityConverter<VendorStagedSaleRuleEntity, VendorStagedSaleRuleEntityVM>.Convert(vssrEntity);
                    if (vm.SaleRuleEntity != null && vm.SaleRuleEntity.StagedSaleRuleItems!=null && vm.SaleRuleEntity.StagedSaleRuleItems.Count>0)
                    {
                        vm.SaleRuleEntity.StagedSaleRuleItems[vm.SaleRuleEntity.StagedSaleRuleItems.Count - 1].EndAmt = null;
                    }
                    vm.GuaranteedAmt = vssrEntity.MinCommissionAmt.ToString();
                    if (vm.SaleRuleEntity != null)
                    {
                        this.ucSaleStageSettings.VendorStageSaleSettingsList = vm.SaleRuleEntity.StagedSaleRuleItems;
                        this.ucSaleStageSettings.BindVendorSaleStageList();
                    }
                    this.DataContext = vm;
                });
            }
        }

        private void BindBrandTree()
        {
            var brand = Newegg.Oversea.Silverlight.Utilities.UtilityHelper.DeepClone(AllBrand);
            this.tvBrand.IsSpeardToParent = true;
            this.tvBrand.Nodes = TransferBrandToNodes(AllBrand); ;
            this.tvBrand.BuildTreeByData();

        }
        private List<Node> TransferBrandToNodes(dynamic result)
        {
            List<Node> data = new List<Node>();
            foreach (var row in result.Rows)
            {
                data.Add(new Node
                {
                    Name = row.BrandName_Ch,
                    Value = row.SysNo,
                    ParentValue = 0
                });
            }
            return data;
        }

        //转换数据,sysno 个表不唯一
        private List<Node> TransferCategoryToNodes(dynamic result)
        {
            List<Node> c1 = new List<Node>();
            List<Node> c2 = new List<Node>();
            List<Node> c3 = new List<Node>();

            foreach (var row in result.Rows)
            {
                if (c1.Where(p => p.Value - 3000000 == row.Category1SysNo).Count() == 0)
                {
                    c1.Add(new Node
                    {
                        Name = row.Category1Name,
                        Value = row.Category1SysNo + 3000000,
                        ParentValue = 0

                    });
                }

                if (c2.Where(p => p.Value - 4000000 == row.Category2SysNo).Count() == 0)
                {
                    c2.Add(new Node
                    {
                        Name = row.Category2Name,
                        Value = row.Category2SysNo + 4000000,
                        ParentValue = row.Category1SysNo + 3000000,

                    });
                }

                if (c3.Where(p => p.Value == row.Category3SysNo).Count() == 0)
                {
                    c3.Add(new Node
                    {
                        Name = row.Category3Name,
                        Value = row.Category3SysNo,
                        ParentValue = row.Category2SysNo + 4000000,

                    });
                }
            }

            return c1.Union(c2).Union(c3).ToList();
        }


        private void btnAddTemplate_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            CommissionRuleTemplateInfo info = new CommissionRuleTemplateInfo();
            RefreshVendorSaleRuleList();
            vm.SaleRuleEntity.StagedSaleRuleItems = this.ucSaleStageSettings.VendorStageSaleSettingsList;
            vm.SaleRuleEntity.MinCommissionAmt = string.IsNullOrEmpty(vm.GuaranteedAmt) ? (decimal?)null : decimal.Parse(vm.GuaranteedAmt);

            if (SysNo == 0)
            {
                info.C3SysNos = new List<CommissionRuleTemplateInfo.CategoryReq>();
                foreach (var c1 in this.tvCategory.TreeNode.SonNodes)
                {
                    foreach (var c2 in c1.SonNodes)
                    {
                        foreach (var c3 in c2.SonNodes)
                        {
                            if (c3.IsSelected)
                            {
                                info.C3SysNos.Add(new CommissionRuleTemplateInfo.CategoryReq() { C1 = c1.Value - 3000000, C2 = c2.Value - 4000000, C3 = c3.Value });
                            }
                        }
                    }
                }
                info.BrandSysNos = new List<int>();
                foreach (var b in this.tvBrand.TreeNode.SonNodes)
                {
                    if (b.IsSelected)
                    {
                        info.BrandSysNos.Add(b.Value);
                    }
                }

                if (info.C3SysNos.Count == 0)
                {
                    MessageBox.Show("请选择一个类别");
                    return;
                }

                if (info.BrandSysNos.Count == 0)
                {
                    MessageBox.Show("请选择一个类别");
                    return;
                }
                info.Status = CommissionRuleStatus.Active;
            }
            else
            {
                info.C3SysNos = new List<CommissionRuleTemplateInfo.CategoryReq>();
                info.C3SysNos.Add(new CommissionRuleTemplateInfo.CategoryReq() { C1 = EditInfo.C1SysNo.Value, C2 = EditInfo.C2SysNo.Value, C3 = EditInfo.C3SysNo.Value });
                info.BrandSysNos = new List<int>();
                info.BrandSysNos.Add(EditInfo.BrandSysNo.Value);
                info.Status = EditInfo.Status;
            }
            info.SaleRuleEntity = EntityConverter<VendorStagedSaleRuleEntityVM, VendorStagedSaleRuleEntity>.Convert(vm.SaleRuleEntity);
            info.RentFee = string.IsNullOrEmpty(vm.RentFee) ? (decimal?)null : decimal.Parse(vm.RentFee);
            info.OrderCommissionAmt = string.IsNullOrEmpty(vm.OrderCommissionAmt) ? (decimal?)null : decimal.Parse(vm.OrderCommissionAmt);
            info.DeliveryFee = string.IsNullOrEmpty(vm.DeliveryFee) ? (decimal?)null : decimal.Parse(vm.DeliveryFee);
            if ((info.OrderCommissionAmt == 0 || info.OrderCommissionAmt == null) && (info.SaleRuleEntity == null || info.SaleRuleEntity.StagedSaleRuleItems == null || info.SaleRuleEntity.StagedSaleRuleItems.Count == 0))
            {
                MessageBox.Show("请至少设置一个销售提成或订单提成规则！");
                return;
            }

            if (info.SaleRuleEntity != null && info.SaleRuleEntity.StagedSaleRuleItems != null && info.SaleRuleEntity.StagedSaleRuleItems.Count > 0)
            {
                info.SaleRuleEntity.StagedSaleRuleItems[info.SaleRuleEntity.StagedSaleRuleItems.Count - 1].EndAmt = 99999999;
            }
            vendorFacade.UpdateCommissionRuleTemplate(info, (o, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                MessageBox.Show("操作成功!");
                this.Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                this.Dialog.Close(true);
            });
        }

        private void btnFilterBrand_Click(object sender, RoutedEventArgs e)
        {
            this.tvBrand.Filter(tbBrandFilter.Text.Trim());
        }

        private void btnExpandAllCategory_Click(object sender, RoutedEventArgs e)
        {
            this.tvCategory.ExpandAll();
        }

        private void btnCollapseAllCategory_Click(object sender, RoutedEventArgs e)
        {
            this.tvCategory.CollapseAll();
        }


        /// <summary>
        /// 重新构建SaleRuleItems
        /// </summary>
        private void RefreshVendorSaleRuleList()
        {
            if (null != ucSaleStageSettings.VendorStageSaleSettingsList)
            {
                this.ucSaleStageSettings.VendorStageSaleSettingsList.Clear();
            }
            else
            {
                this.ucSaleStageSettings.VendorStageSaleSettingsList = new List<VendorStagedSaleRuleInfoVM>();
            }
            int index = 1;
            foreach (var ucItem in this.ucSaleStageSettings.spSaleStageSettings.Children)
            {
                if (ucItem is VendorSaleStageSettingsItem)
                {
                    VendorSaleStageSettingsItem uc = (VendorSaleStageSettingsItem)ucItem;
                    this.ucSaleStageSettings.VendorStageSaleSettingsList.Add(new VendorStagedSaleRuleInfoVM()
                    {
                        Order = index,
                        StartAmt = uc.StageAmtBeginVal,
                        EndAmt = uc.StageAmtEndVal,
                        Percentage = uc.StagePercentage
                    });
                }
                index++;
            }

            if (!string.IsNullOrEmpty(this.ucSaleStageSettings.txtStagedPercentage_Last.Text))
            {
                this.ucSaleStageSettings.VendorStageSaleSettingsList.Add(new VendorStagedSaleRuleInfoVM()
                {
                    Order = index,
                    StartAmt = string.IsNullOrEmpty(this.ucSaleStageSettings.lblStagedAmtBegin_Last.Text) ? 0 : decimal.Parse(this.ucSaleStageSettings.lblStagedAmtBegin_Last.Text),
                    EndAmt = 0,
                    Percentage = string.IsNullOrEmpty(this.ucSaleStageSettings.txtStagedPercentage_Last.Text) ? 0 : decimal.Parse(this.ucSaleStageSettings.txtStagedPercentage_Last.Text),
                });
            }
        }
    }
}
