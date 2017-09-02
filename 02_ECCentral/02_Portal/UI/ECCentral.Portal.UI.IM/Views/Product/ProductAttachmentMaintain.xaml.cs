using System;
using System.Text.RegularExpressions;
using System.Windows;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Facades.Product;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using UtilityHelper = Newegg.Oversea.Silverlight.Utilities.UtilityHelper;

namespace ECCentral.Portal.UI.IM.Views
{
    [View(IsSingleton = true, SingletonType = SingletonTypes.Url)]
    public partial class ProductAttachmentMaintain : PageBase
    {

        #region 属性
        private ProductAttachmentFacade _facade;
        private int _sysNo;
        private ProductAttachmentDetailsVM _detailsVM;
        private ProductAttachmentDetailsListVM _vm;
        private static readonly Regex m_regex = new Regex("^[0-9]*[1-9][0-9]*$");
        private const int MaxCount = 10;

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo InUser { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }
        #endregion

        #region 初始化加载
        public ProductAttachmentMaintain()
        {
            InitializeComponent();
        }

        public override void OnPageLoad(object sender, EventArgs e)
        {
            base.OnPageLoad(sender, e);
            var productSysNo = Request.Param;
            _detailsVM = new ProductAttachmentDetailsVM();
            _vm = new ProductAttachmentDetailsListVM();
            _facade = new ProductAttachmentFacade();
            if (!string.IsNullOrEmpty(productSysNo))
            {
                ucProductPicker.IsEnabled = false;
                if(Int32.TryParse(productSysNo, out _sysNo))
                {
                    _vm.ProductSysNo = _sysNo;
                    _detailsVM.ProductSysNo = _sysNo;
                    BindPage();
                }
                else
                {
                    Window.Alert("商品编号无效！", MessageType.Error);
                    return;
                }

            }
            else
            {
                ucProductPicker.IsEnabled = true;
            }
            expander1.DataContext = _detailsVM;
        }
        #endregion

        #region 查询绑定
        private void BindPage()
        {
            if (_sysNo > 0)
            {
                ucProductPicker.ProductSysNo = _sysNo.ToString();
                _facade.GetProductAttachmentList(_sysNo,
                   (obj, args) =>
                   {
                       if (args.FaultsHandle())
                       {
                           return;
                       }
                       foreach (ProductAttachmentInfo t in args.Result)
                       {
                           var entity = new ProductAttachmentDetailsVM
                           {
                               ProductAttachmentSysNo = t.AttachmentProduct.SysNo,
                               ProductSysNo = _sysNo,
                               AttachmentQuantity = Convert.ToString(t.Quantity),
                               EditUser = t.EditUser,
                               EditDate = t.EditDate,
                               InUser = t.InUser,
                               InDate=t.InDate,
                               AttachmentProductID = t.AttachmentProduct.ProductID,
                               Operator=AttachmentOperator.Update,
                               AttachmentProductName = t.AttachmentProduct.ProductBasicInfo.ProductBriefTitle.Content
                           };
                           InUser = t.InUser;
                           InDate = t.InDate;
                           _vm.ProductAttachmentList.Add(entity);
                       }
                       dgProductAttachmentList.ItemsSource = _vm.ProductAttachmentList;
                   });
            }

        }
        #endregion

        #region 页面内按钮处理事件

        #region 界面事件


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_vm.ProductAttachmentList == null || _vm.ProductAttachmentList.Count <= 0)
            {
                Window.Alert("附件个数不能为零！", MessageType.Error);
                return;
            }
            if (_vm.ProductSysNo == null || _vm.ProductSysNo.Value <= 0)
            {
                Window.Alert("请选择主商品！", MessageType.Error);
                return;
            }
            if (_sysNo > 0)
            {
                _facade.UpdateAttachment(_vm, (obj, args) =>
                                                 {
                                                     if (args.FaultsHandle())
                                                     {
                                                         return;
                                                     }
                                                     Window.Alert(ResProductAttachmentMaintain.Info_SucessMessage, MessageType.Information);
                                                 });
            }
            else
            {
                _facade.CreateAttachment(_vm, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Window.Alert(ResProductAttachmentMaintain.Info_SucessMessage, MessageType.Information);
                });
            }
        }

        private void btnAddAttachment_Click(object sender, RoutedEventArgs e)
        {
            var result = IsValid();
            if (!result) return;
            ucProductPicker.IsEnabled = false;
            if (_vm.ProductAttachmentList.Contains(_detailsVM))
            {
                Window.Alert(ResProductAttachmentMaintain.Msg_OnAddProductAttachment, MessageType.Error);
                return;
            }
            _vm.ProductSysNo = _detailsVM.ProductSysNo;
            GetProductInfo(_detailsVM.ProductAttachmentSysNo, SetProductAttachmentDetailsVM);
            GetProductInfo(_detailsVM.ProductSysNo);
        }

        private bool IsValid()
        {
            if (ucProductPicker.ProductSysNo == null || !m_regex.IsMatch(ucProductPicker.ProductSysNo))
            {
                Window.Alert(ResProductAttachmentMaintain.ProductSysNoInvalid, MessageType.Error);
                return false;
            }
            if (ucAttachmentPicker.ProductSysNo == null || !m_regex.IsMatch(ucAttachmentPicker.ProductSysNo))
            {
                Window.Alert(ResProductAttachmentMaintain.ProductAttachmentSysNoInvalid, MessageType.Error);
                return false;
            }
            if (ucProductPicker.ProductSysNo == ucAttachmentPicker.ProductSysNo)
            {
                Window.Alert("主商品与附件不能是同一商品", MessageType.Error);
                return false;
            }
            if (_vm.ProductAttachmentList.Count >= MaxCount)
            {
                Window.Alert("附件数量不能超过十个", MessageType.Error);
                return false;
            }
            var result = ValidationManager.Validate(expander1);
            return result;

        }


        private void hyperlinkOperationEdit_Click(object sender, RoutedEventArgs e)
        {
            Window.Confirm(ResProductAttachmentQuery.Confirm_Delete
                , (obj, args) =>
                            {
                                if (args.DialogResult == DialogResultType.OK)
                                {
                                    var manufacturer = dgProductAttachmentList.SelectedItem as ProductAttachmentDetailsVM;
                                    _vm.ProductAttachmentList.Remove(manufacturer);
                                    dgProductAttachmentList.ItemsSource = _vm.ProductAttachmentList;
                                    if (_vm.ProductAttachmentList == null || _vm.ProductAttachmentList.Count == 0)
                                    {
                                        ucProductPicker.IsEnabled = true;
                                    }
                                }
                            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Window.Close();
        }

        private void GetProductInfo(int? productSysNo, Action<ProductInfo> callback = null)
        {
            if (productSysNo == null) return;
            var productFacade = new ProductFacade();
            productFacade.GetProductInfo(productSysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result == null || args.Result.ProductBasicInfo == null || args.Result.ProductCommonInfoSysNo == null || args.Result.ProductCommonInfoSysNo.Value <= 0)
                {
                    Window.MessageBox.Show(callback == null ? "商品编号无效." : "附件编号无效.", MessageBoxType.Warning);
                    return;
                }
                if (callback != null)
                {
                    callback(args.Result);
                }
            });

        }

        private void SetProductAttachmentDetailsVM(ProductInfo entity)
        {
            if (entity == null) return;
            _detailsVM.AttachmentProductID = entity.ProductID;
            _detailsVM.AttachmentProductName = entity.ProductName;
            var tempEntity = UtilityHelper.DeepClone(_detailsVM);
            if(_sysNo<=0)
            {
                tempEntity.InDate = DateTime.Now;
                tempEntity.InUser = new UserInfo { UserName = CPApplication.Current.LoginUser.LoginName };
            }
            else
            {
                tempEntity.InDate = InDate;
                tempEntity.InUser = InUser; 
            }
            
            tempEntity.Operator = _sysNo > 0 ? AttachmentOperator.Update : AttachmentOperator.Add;
            _vm.ProductAttachmentList.Add(tempEntity);
            dgProductAttachmentList.ItemsSource = _vm.ProductAttachmentList;
        }
        #endregion

        #endregion




    }
}
