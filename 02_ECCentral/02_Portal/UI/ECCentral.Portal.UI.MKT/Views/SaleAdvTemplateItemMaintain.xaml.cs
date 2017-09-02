using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.UserControls;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.UI.MKT.Resources;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.Enum.Resources;

namespace ECCentral.Portal.UI.MKT.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class SaleAdvTemplateItemMaintain : PageBase
    {
        private int sysNo;

        public SaleAdvertisementVM VM
        {
            get
            {
                return this.DataContext as SaleAdvertisementVM;
            }
            private set
            {
                this.DataContext = value;
            }
        }       

        public SaleAdvTemplateItemMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);

            string no = Request.Param;
            if (!string.IsNullOrEmpty(no))
            {
                if (int.TryParse(no, out sysNo))
                {
                    new SaleAdvTemplateFacade(this).Load(sysNo, (obj, args) =>
                    {
                        this.VM = args.Result;

                        this.VM.Groups.Insert(0, new SaleAdvGroupVM { GroupName = ResCommonEnum.Enum_Select });

                        this.dataSaveAdvGroups.ItemsSource = this.VM.Groups.Where(p => p.SysNo > 0).ToList();
                        this.dataSaleAdvItems.ItemsSource = this.VM.Items;
                        if (this.VM.IsHold != null && this.VM.IsHold.Trim().ToUpper() == "Y")
                        {
                            this.ContainerControl.IsEnabled = false;
                        }
                        else
                        {
                            this.VM.IsHold = "N";
                            this.ContainerControl.IsEnabled = true;
                        }
                    });
                }
            }
            else
            {
                this.VM = new SaleAdvertisementVM();
            }
        }

        private void btnAddGroup_Click(object sender, RoutedEventArgs e)
        {
            UCSaleAdvGroupInfo uc = new UCSaleAdvGroupInfo();
            uc.Page = this;
            uc.DataContext = new SaleAdvGroupVM { SaleAdvSysNo = this.VM.SysNo };
            IDialog dialog = Window.ShowDialog(ResSaleAdvTemplateItemMaintain.PopTitle_GroupInfo, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var vm = args.Data as SaleAdvGroupVM;

                    //如果勾选了所有分组适用，则更新所有Group的EditUser和EditDate以及ShowStartDate和ShowEndDate
                    if ((uc.DataContext as SaleAdvGroupVM).AllGroup)
                    {
                        foreach (var g in this.VM.Groups)
                        {
                            g.ShowStartDate = vm.ShowStartDate;
                            g.ShowEndDate = vm.ShowEndDate;
                            g.EditDate = vm.EditDate;
                            g.EditUser = vm.EditUser;
                        }
                    }
                    
                    this.VM.Groups.Add(vm);

                    //this.dataSaveAdvGroups.ItemsSource = this.VM.Groups;
                    this.dataSaveAdvGroups.ItemsSource = this.VM.Groups.Where(p => p.SysNo > 0).ToList();

                    Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
                }
            });
            uc.Dialog = dialog;
        }

        private void btnUpdateGroup_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;

            //深Copy导致RecommendTypeList多出两条空记录，故重新在取一次RecommendTypeList
            var vm = UtilityHelper.DeepClone<SaleAdvGroupVM>(btn.DataContext as SaleAdvGroupVM);
            vm.RecommendTypeList = ECCentral.Portal.Basic.Utilities.EnumConverter.GetKeyValuePairs<RecommendType>();

            UCSaleAdvGroupInfo uc = new UCSaleAdvGroupInfo();
            uc.Page = this;
            uc.DataContext = vm;
            IDialog dialog = Window.ShowDialog(ResSaleAdvTemplateItemMaintain.PopTitle_GroupInfo, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var item = args.Data as SaleAdvGroupVM;
                    var i = this.VM.Groups.FirstOrDefault(p => p.SysNo == vm.SysNo);
                    int index = this.VM.Groups.IndexOf(i);
                 
                    this.VM.Groups.RemoveAt(index);
                    this.VM.Groups.Insert(index, item);
                    this.dataSaveAdvGroups.ItemsSource = this.VM.Groups.Where(p => p.SysNo > 0).ToList();
                    this.dataSaleAdvItems.ItemsSource = this.VM.Items;
                }
            });
            uc.Dialog = dialog;
        }

        private void btnDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResSaleAdvTemplateItemMaintain.Confirm_DeleteGroup, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    HyperlinkButton btn = sender as HyperlinkButton;
                    var vm = btn.DataContext as SaleAdvGroupVM;
                    new SaleAdvTemplateFacade(this).DeleteSaleAdvGroup(vm.SysNo.Value, (o, a) =>
                    {                       
                        //删除该分组下的所有商品
                        var list = this.VM.Items.Where(p => p.GroupSysNo == vm.SysNo).ToList();
                        list.ForEach(p =>
                        {
                            this.VM.Items.Remove(p);
                        });

                        this.VM.Groups.Remove(vm);

                        this.dataSaveAdvGroups.ItemsSource = this.VM.Groups.Where(p => p.SysNo > 0).ToList();

                        Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
                    });
                }
            });
        }

        private void btnAddProduct_Click(object sender, RoutedEventArgs e)
        {
            UCSaleAdvItemInfo uc = new UCSaleAdvItemInfo();
            uc.Page = this;
            var itemVM = new SaleAdvItemVM { SaleAdvSysNo = this.VM.SysNo };

            //if (this.VM.Groups.Count == 0)
            //{
            //    itemVM.Groups.Insert(0, new SaleAdvGroupVM { GroupName = ResCommonEnum.Enum_Select });
            //}
            //else
            //{
                itemVM.Groups = this.VM.Groups;
            //}
            if (this.VM.Groups != null && this.VM.Groups.Count > 0)
            {
                itemVM.GroupSysNo = this.VM.Groups.FirstOrDefault().SysNo;
            }
            uc.DataContext = itemVM;
            IDialog dialog = Window.ShowDialog(ResSaleAdvTemplateItemMaintain.PopTitle_ProductInfo, uc, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var vm = args.Data as SaleAdvItemVM;
                    int index = this.VM.Items.IndexOf(vm);
                    if (index > 0)
                    {
                        this.VM.Items.RemoveAt(index);
                        this.VM.Items.Insert(index, vm);
                    }
                    else
                    {
                        this.VM.Items.Add(vm);
                    }

                    this.dataSaleAdvItems.ItemsSource = this.VM.Items;
                    this.VM.CalculateItemsCount();

                    Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
                }
            });
            uc.Dialog = dialog;
        }

        private void btnBatchAddItem_Click(object sender, RoutedEventArgs e)
        {
            UCSaleAdvItemBatchAdd uc = new UCSaleAdvItemBatchAdd();
            uc.Page = this;
            var itemVM = new SaleAdvItemVM { SaleAdvSysNo = this.VM.SysNo };

            itemVM.Groups = this.VM.Groups;
            if (this.VM.Groups != null && this.VM.Groups.Count > 0)
            {
                itemVM.GroupSysNo = this.VM.Groups.FirstOrDefault().SysNo;
            }
            uc.DataContext = itemVM;
            IDialog dialog = Window.ShowDialog(ResSaleAdvTemplateItemMaintain.PopTitle_ProductInfo, uc);
            uc.Dialog = dialog;
        }

        private void ckbSelectAllRow_Click(object sender, RoutedEventArgs e)
        {
            CheckBox chk = (CheckBox)sender;
            var rows = this.dataSaleAdvItems.ItemsSource;
            foreach (SaleAdvItemVM row in rows)
            {
                row.IsChecked = chk.IsChecked.Value;
            }
        }       

        private void btnBatchUpdateItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.VM.Items.Count(p => p.IsChecked) == 0)
            {
                Window.Alert(ResSaleAdvTemplateItemMaintain.Warning_NoItemsSelected, MessageType.Warning);
                return;
            }
            //只能验证可视化范围内的数据，如果在可视化范围之外，因为控件都还没有创建，无法验证，只能通过验证实体的方式处理
            if (ValidationManager.Validate(this.dataSaleAdvItems))
            {
                var list = this.VM.Items.Where(p => p.IsChecked).ToList();
                list.ForEach(p => p.RecommendType = this.VM.Groups.Where(g=> g.SysNo==p.GroupSysNo).First().RecommendType);
                new SaleAdvTemplateFacade(this).BatchUpdateSaleAdvItem(list, (obj, args) =>
                {
                    this.chkHidden.IsChecked = false;

                    UnCheckItems();

                    this.VM.CalculateItemsCount();

                    Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
                });
            }
        }

        private void btnBatchActive_Click(object sender, RoutedEventArgs e)
        {
            if (this.VM.Items.Count(p => p.IsChecked) == 0)
            {
                Window.Alert(ResSaleAdvTemplateItemMaintain.Warning_NoItemsSelected, MessageType.Warning);
                return;
            }
            var list = this.VM.Items.Where(p => p.IsChecked).ToList();
            list.ForEach(p => p.Status = ADStatus.Active);
            new SaleAdvTemplateFacade(this).BatchUpdateSaleAdvItemStatus(list, (obj, args) =>
            {
                this.chkHidden.IsChecked = false;

                UnCheckItems();

                Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
            });
        }

        private void btnBatchDeactive_Click(object sender, RoutedEventArgs e)
        {
            if (this.VM.Items.Count(p => p.IsChecked) == 0)
            {
                Window.Alert(ResSaleAdvTemplateItemMaintain.Warning_NoItemsSelected, MessageType.Warning);
                return;
            }
            var list = this.VM.Items.Where(p => p.IsChecked).ToList();
            list.ForEach(p => p.Status = ADStatus.Deactive);
            new SaleAdvTemplateFacade(this).BatchUpdateSaleAdvItemStatus(list, (obj, args) =>
            {
                this.chkHidden.IsChecked = false;

                UnCheckItems();

                Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
            });
        }

        private void btnBatchDeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (this.VM.Items.Count(p => p.IsChecked) == 0)
            {
                Window.Alert(ResSaleAdvTemplateItemMaintain.Warning_NoItemsSelected, MessageType.Warning);
                return;
            }
            Window.Confirm(ResSaleAdvTemplateItemMaintain.Confirm_DeleteProduct, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    var list = this.VM.Items.Where(p => p.IsChecked).ToList();

                    new SaleAdvTemplateFacade(this).BatchDeleteSaleAdvItem(list, (o, a) =>
                    {
                        list.ForEach(p =>
                        {
                            this.VM.Items.Remove(p);
                        });

                        this.chkHidden.IsChecked = false;

                        UnCheckItems();

                        this.VM.CalculateItemsCount();

                        Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
                    });
                }
            });
        }

        private void btnDeactive_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            var orgVM = btn.DataContext as SaleAdvItemVM;
            var vm = UtilityHelper.DeepClone<SaleAdvItemVM>(orgVM);
            vm.Status = ADStatus.Deactive;
            new SaleAdvTemplateFacade(this).UpdateSaleAdvItemStatus(vm, (obj, args) =>
            {
                orgVM.Status = ADStatus.Deactive;
                Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
            });
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResSaleAdvTemplateItemMaintain.Confirm_DeleteProduct, (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    HyperlinkButton btn = sender as HyperlinkButton;
                    var vm = btn.DataContext as SaleAdvItemVM;
                    new SaleAdvTemplateFacade(this).DeleteSaleAdvItem(vm.SysNo.Value, (o, a) =>
                    {
                        this.VM.Items.Remove(vm);

                        this.VM.CalculateItemsCount();

                        Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
                    });
                }
            });
        }

        private void btnActive_Click(object sender, RoutedEventArgs e)
        {
            HyperlinkButton btn = sender as HyperlinkButton;
            var orgVM = btn.DataContext as SaleAdvItemVM;
            var vm = UtilityHelper.DeepClone<SaleAdvItemVM>(orgVM);
            vm.Status = ADStatus.Active;
            new SaleAdvTemplateFacade(this).UpdateSaleAdvItemStatus(vm, (obj, args) =>
            {
                orgVM.Status = ADStatus.Active;
                Window.Alert(ResSaleAdvTemplateItemMaintain.Info_Successfully);
            });
        }

        private void UnCheckItems()
        {
            foreach(var item in this.VM.Items)
            {
                item.IsChecked = false;
            }
        }
    }
}
