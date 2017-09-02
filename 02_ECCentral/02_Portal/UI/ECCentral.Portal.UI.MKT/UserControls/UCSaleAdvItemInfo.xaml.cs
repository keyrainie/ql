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
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Views;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCSaleAdvItemInfo : UserControl
    {
        public IDialog Dialog { get; set; }
        public SaleAdvTemplateItemMaintain Page { get; set; }

        public UCSaleAdvItemInfo()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as SaleAdvItemVM;
            
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                if (vm.SysNo > 0)
                {
                    new SaleAdvTemplateFacade(this.Page).UpdateSaleAdvItem(vm, (obj, args) =>
                    {
                        this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = args.Result };
                        this.Dialog.Close();
                    });
                }
                else
                {
                    var group = vm.Groups.FirstOrDefault(p => p.SysNo == vm.GroupSysNo);
                    if (group == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("请先添加分组信息!", MessageType.Warning);
                        return;
                    }
                    var productInfo = this.ucProcutPicker.SelectedProductInfo;
                    if (productInfo == null)
                    {
                        CPApplication.Current.CurrentPage.Context.Window.Alert("请选择商品!", MessageType.Warning);
                        return;
                    }
                    vm.ProductID = productInfo.ProductID;
                    vm.ProductName = productInfo.ProductName;
                    vm.ProductStatus = productInfo.Status;
                    vm.OnlineQty = productInfo.OnlineQty;
                    vm.MarketPrice = productInfo.BasicPrice;
                    vm.RecommendType = group.RecommendType;

                    if (group != null && group.SysNo > 0)
                    {
                        vm.GroupPriority = int.Parse(group.Priority);
                        vm.GroupName = group.GroupName;
                    }
                    new SaleAdvTemplateFacade(this.Page).CreateSaleAdvItem(vm, (obj, args) =>
                    {
                        this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = args.Result };
                        this.Dialog.Close();
                    });
                }                
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }
    }
}
