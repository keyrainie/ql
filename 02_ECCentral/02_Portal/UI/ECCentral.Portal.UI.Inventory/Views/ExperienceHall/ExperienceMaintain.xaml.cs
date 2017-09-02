using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Text.RegularExpressions;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;

using ECCentral.BizEntity.Inventory;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.Inventory.Resources;
using ECCentral.Portal.UI.Inventory.Models;
using ECCentral.Portal.UI.Inventory.Facades;
using ECCentral.Portal.UI.Inventory.UserControls;
using ECCentral.Portal.Basic.Components.UserControls.ProductPicker;
using ECCentral.QueryFilter.Inventory;
using ECCentral.BizEntity.Common;
using ECCentral.Portal.UI.Inventory.UserControls.Inventory;
using ECCentral.Portal.UI.Inventory.Models.Inventory;

namespace ECCentral.Portal.UI.Inventory.Views
{
    [View(IsSingleton = true, NeedAccess = false, SingletonType = SingletonTypes.Url)]
    public partial class ExperienceMaintain : PageBase
    {
        ExperienceVM experienceVM;
        ExperienceVM ExperienceVM
        {
            get
            {
                return experienceVM;
            }
            set
            {
                experienceVM = value;
                experienceVM = experienceVM ?? new ExperienceVM();
            }
        }

        private List<ExperienceItemVM> ExperienceItemInfoList
        {
            get
            {
                return (ExperienceVM.ExperienceItemInfoList);
            }
        }

        ExperienceMaintainFacade MaintainFacade;
        private int? _sysNo;
        private int? SysNo
        {
            get
            {
                if (!_sysNo.HasValue)
                {
                    int tSysNo = 0;
                    if (!string.IsNullOrEmpty(Request.Param) && int.TryParse(Request.Param, out tSysNo))
                    {
                        _sysNo = tSysNo;
                    }
                }
                return _sysNo;
            }
            set
            {
                _sysNo = value;
            }
        }

        public ExperienceMaintain()
        {
            InitializeComponent();
        }
        public override void OnPageLoad(object sender, EventArgs e)
        {
            MaintainFacade = new ExperienceMaintainFacade(this);
            base.OnPageLoad(sender, e);
            if (SysNo.HasValue)
            {
                MaintainFacade.GetLendRequestInfoBySysNo(SysNo.Value, (vm) =>
                {
                    if (vm == null)
                    {
                        vm = null;
                        Window.Alert("单据不存在，此单据可能已经被删除或请传入其它的单据编号重试。");
                    }
                    ExperienceVM = vm;
                    SetDataContext();
                });
            }
            else
            {
                ExperienceVM = new ExperienceVM();

                experienceVM.ExperienceItemInfoList = new List<ExperienceItemVM>();

                tbstatus.Visibility = System.Windows.Visibility.Collapsed;
                tbStatusText.Visibility = System.Windows.Visibility.Collapsed;
                SetDataContext();
            }
        }

        private void SetDataContext()
        {
            this.DataContext = ExperienceVM;

            experienceVM.ExperienceItemInfoList.ForEach(x => {
                x.IsEditMode = experienceVM.Status == ExperienceHallStatus.Created;
            });

            ucExperienceItemList.dgProductList.ItemsSource = new ObservableCollection<ExperienceItemVM>(ExperienceItemInfoList);
            ucExperienceItemList.dgProductList.Bind();
            SetOpertionButton();
        }

        private void SetOpertionButton()
        {
            if (SysNo.HasValue)
            {
                btntytjs.IsEnabled = experienceVM.AllocateType == AllocateType.ExperienceIn;
                btnckjs.IsEnabled = experienceVM.AllocateType == AllocateType.ExperienceOut;
            }
            else
            {
                btntytjs.IsEnabled = false;
                btnckjs.IsEnabled = false;
            }

            btnAbandon.IsEnabled = btnAudit.IsEnabled = btnCancelAudit.IsEnabled = false;
            if (ExperienceVM.SysNo.HasValue)
            {
                switch (ExperienceVM.Status)
                {
                    case ExperienceHallStatus.Created:
                        btnAbandon.IsEnabled = btnAudit.IsEnabled = true;
                        btnAdd.IsEnabled = true;
                        btnSave.IsEnabled = true;
                        btntytjs.IsEnabled = false;
                        btnckjs.IsEnabled = false;
                        break;
                    case ExperienceHallStatus.Abandon:
                    case ExperienceHallStatus.Experienced:
                        btnAbandon.IsEnabled = false;
                        btnSave.IsEnabled = false;
                        btnCancelAudit.IsEnabled = false;
                        btnAdd.IsEnabled = false;
                        btntytjs.IsEnabled = false;
                        btnckjs.IsEnabled = false;
                        break;
                    case ExperienceHallStatus.Audit:
                        btnAdd.IsEnabled = false;
                        btnAudit.IsEnabled = false;
                        btnAbandon.IsEnabled = true;
                        btnCancelAudit.IsEnabled = true;
                        break;
                    //case ExperienceHallStatus.Experienced:
                    //    btnAbandon.IsEnabled = btnAudit.IsEnabled = btnSave.IsEnabled = false;
                    //    btnCancelAudit.IsEnabled = false;
                    //    btnAdd.IsEnabled = false;
                    //    btntytjs.IsEnabled = false;
                    //    btnckjs.IsEnabled = false;
                    //    break;
                }
            }
            
            #region 权限设置

            SetLendRight();

            #endregion
        }

        private void SetLendRight()
        {
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ExperienceHall_Create))
            {
                btnSave.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ExperienceHall_Abandon))
            {
                btnAbandon.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ExperienceHall_Audit))
            {
                btnAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ExperienceHall_CancelAudit))
            {
                btnCancelAudit.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ExperienceHall_ExperienceIn))
            {
                btntytjs.IsEnabled = false;
            }
            if (!AuthMgr.HasFunctionPoint(AuthKeyConst.Inventory_ExperienceHall_ExperienceOut))
            {
                btnckjs.IsEnabled = false;
            }
        }

        #region 商品操作

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            UCProductSearch picker = new UCProductSearch();
            picker.SelectionMode = SelectionMode.Multiple;

            IDialog dialog = Window.ShowDialog("选择商品", picker, (o, a) =>
            {
                if (a.DialogResult == DialogResultType.OK)
                {
                    var list = a.Data as List<ProductVM>;
                    if (list != null && list.Count > 0)
                    {
                        list.ForEach(p =>
                        {
                            if (!ExperienceItemInfoList.Any(x => x.ProductSysNo == p.SysNo))
                                ExperienceItemInfoList.Add(new ExperienceItemVM()
                                {
                                    ProductSysNo = p.SysNo,
                                    ProductName = p.ProductName,
                                    AllocateQty = 0,
                                    ProductID = p.ProductID
                                });
                        });

                        ucExperienceItemList.dgProductList.ItemsSource = new ObservableCollection<ExperienceItemVM>(ExperienceItemInfoList);
                        ucExperienceItemList.dgProductList.Bind();
                    }
                }
            });

            picker.DialogHandler = dialog;
        }

        #endregion 借货商品操作


        #region 借货单操作

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (experienceVM.ExperienceItemInfoList.Count <= 0)
            {
                Window.Alert("请至少添加一个商品");
                return;
            }

            if (SysNo.HasValue)
            {
                InnerUpdateExperience();
            }
            else
            {
                InnerCreateExperience();
            }
        }

        private void InnerCreateExperience()
        {
            MaintainFacade.CreateRequest(ExperienceVM, sysno =>
            {
                if (sysno.HasValue)
                {
                    SysNo = sysno;

                    Window.Alert("体验厅调拨单创建成功");

                    MaintainFacade.GetLendRequestInfoBySysNo(SysNo.Value, (mm) =>
                    {
                        if (mm == null)
                        {
                            mm = null;
                            Window.Alert("获取单据失败");
                        }
                        ExperienceVM = mm;

                        SetDataContext();
                    });
                }
                else
                {
                    Window.Alert("体验厅调拨单创建失败");
                }
            });
        }

        private void InnerUpdateExperience()
        {
            MaintainFacade.UpdateRequest(ExperienceVM, sysno =>
            {
                if (sysno.HasValue)
                {
                    SysNo = sysno;
                    Window.Alert("体验厅调拨单修改成功");

                    MaintainFacade.GetLendRequestInfoBySysNo(SysNo.Value, (mm) =>
                    {
                        if (mm == null)
                        {
                            mm = null;
                            Window.Alert("获取单据失败");
                        }
                        ExperienceVM = mm;

                        SetDataContext();
                    });
                }
            });
        }

        private void btnAbandon_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确认要进行作废操作？", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    MaintainFacade.AbandonExperience(ExperienceVM, vm =>
                    {
                        if (vm != null)
                        {
                            Window.Alert("作废成功");

                            MaintainFacade.GetLendRequestInfoBySysNo(SysNo.Value, (mm) =>
                            {
                                if (mm == null)
                                {
                                    mm = null;
                                    Window.Alert("获取单据失败");
                                }
                                ExperienceVM = mm;

                                SetDataContext();
                            });
                        }
                    });
                }
            });
        }

        private void btnAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确认要进行审核操作？", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    MaintainFacade.AuditExperience(ExperienceVM, vm =>
                    {
                        if (vm != null)
                        {
                            Window.Alert("审核成功");

                            MaintainFacade.GetLendRequestInfoBySysNo(SysNo.Value, (mm) =>
                            {
                                if (mm == null)
                                {
                                    mm = null;
                                    Window.Alert("获取单据失败");
                                }
                                ExperienceVM = mm;

                                SetDataContext();
                            });
                        }
                    });
                }
            });
        }

        private void btnCancelAudit_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确认要进行取消审核操作？", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    MaintainFacade.CancelAuditExperience(ExperienceVM, vm =>
                    {
                        if (vm != null)
                        {
                            Window.Alert("取消审核成功");

                            MaintainFacade.GetLendRequestInfoBySysNo(SysNo.Value, (mm) =>
                            {
                                if (mm == null)
                                {
                                    mm = null;
                                    Window.Alert("获取单据失败");
                                }
                                ExperienceVM = mm;

                                SetDataContext();
                            });
                        }
                    });
                }
            });
        }

        /// <summary>
        /// 体验厅接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btntytjs_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确认要进行体验厅接收操作？", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    MaintainFacade.ExperienceInOrOut(ExperienceVM, vm =>
                    {
                        if (vm != null)
                        {
                            Window.Alert("体验厅接收成功");

                            MaintainFacade.GetLendRequestInfoBySysNo(SysNo.Value, (mm) =>
                            {
                                if (mm == null)
                                {
                                    mm = null;
                                    Window.Alert("获取单据失败");
                                }
                                ExperienceVM = mm;

                                SetDataContext();
                            });
                        }
                    });
                }
            });
        }

        /// <summary>
        /// 仓库接收
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnckjs_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm("确认要进行仓库接收操作？", (obj, args) =>
            {
                if (args.DialogResult == DialogResultType.OK)
                {
                    MaintainFacade.ExperienceInOrOut(ExperienceVM, vm =>
                    {
                        if (vm != null)
                        {
                            Window.Alert("仓库接收成功");

                            MaintainFacade.GetLendRequestInfoBySysNo(SysNo.Value, (mm) =>
                            {
                                if (mm == null)
                                {
                                    mm = null;
                                    Window.Alert("获取单据失败");
                                }
                                ExperienceVM = mm;

                                SetDataContext();
                            });
                        }
                    });
                }
            });
        }

        #endregion 页面内按钮处理事件
    }
}
