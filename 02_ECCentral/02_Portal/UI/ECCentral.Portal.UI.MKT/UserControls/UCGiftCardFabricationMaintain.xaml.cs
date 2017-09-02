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
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Service.IM.Restful;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.IM;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using ECCentral.BizEntity.PO;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCGiftCardFabricationMaintain : UserControl
    {
        public GiftCardFabricationVM VM { get; set; }
        public List<GiftCardFabricationItemVM> gridVM;
        public IDialog Dialog { get; set; }
        private GiftCardFacade facade;
        private string total = ResGiftCardInfo.Information_GiftCardStatistics;//"记录数：{0} 总金额：{1} 总数量：{2}           ";
        private GiftCardFabricationItemRsp rsp;

        public UCGiftCardFabricationMaintain()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCGiftCardFabricationMaintain_Loaded);
        }

        void UCGiftCardFabricationMaintain_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCGiftCardFabricationMaintain_Loaded);
            facade = new GiftCardFacade(CPApplication.Current.CurrentPage);
            BtnStack.DataContext = new GiftCardFabricationVM();

            if (VM != null)
            {
                ECCentral.Portal.Basic.Utilities.CodeNamePairHelper.GetList("IM", "GiftCardFabricationStatus", (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    comStatus.ItemsSource = args.Result;
                    VM.ChannelID = "1";
                    if (args.Result != null)
                        VM.Status = args.Result.SingleOrDefault(a => a.Name == VM.Status).Code;


                    if (string.IsNullOrEmpty(VM.POSysNo))
                        hlPOSysNo.Visibility = System.Windows.Visibility.Collapsed;


                    //if (VM.POStatus != ECCentral.BizEntity.PO.PurchaseOrderStatus.Abandoned)
                    if (VM.Status != "-1" && VM.Status != "-2")
                    {
                        if (VM.POStatus == PurchaseOrderStatus.WaitingInStock || VM.Status == "1")
                            btnExport.Visibility = Visibility.Visible;
                        else if (VM.Status == "0" && VM.POStatus == null)
                        {
                            btnSave.Visibility = Visibility.Visible;
                            btnCreatePO.Visibility = Visibility.Visible;
                            btnVoid.Visibility = Visibility.Visible;
                        };

                    }

                    // if (VM.POStatus == PurchaseOrderStatus.InStocked)//测试用
                    //    btnExport.IsEnabled = true;

                    BaseInfo.DataContext = VM;
                });
            }
            else
            {
                ECCentral.Portal.Basic.Utilities.CodeNamePairHelper.GetList("IM", "GiftCardFabricationStatus", (obj, args) =>
                {
                    if (args.FaultsHandle()) return;
                    comStatus.ItemsSource = args.Result;
                });
                hlPOSysNo.Visibility = System.Windows.Visibility.Collapsed;

                VM = new GiftCardFabricationVM();
                VM.ChannelID = "1";
                VM.Status = "0";
                VM.SysNo = 0;
                BaseInfo.DataContext = VM;
                tbTotal.Text = string.Format(total, 0, 0, 0);

                btnSave.Visibility = Visibility.Visible;
                //btnCreatePO.IsEnabled = false;
                //btnVoid.IsEnabled = false;
                //btnExport.IsEnabled = false;
            }

            facade.GetGiftCardFabricationItem(VM.SysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                rsp = args.Result;

                gridVM = new List<GiftCardFabricationItemVM>();
                if (rsp.GiftCardFabricationList != null)
                {
                    foreach (GiftCardFabrication gift in rsp.GiftCardFabricationList)
                    {
                        
                        GiftCardFabricationItemVM item = new GiftCardFabricationItemVM();
                        item.SysNo = gift.SysNo.Value;
                        item.Quantity = gift.Quantity;
                        item.ProductName = gift.Product.ProductBasicInfo.ShortDescription.Content;
                        item.CurrentPrice = gift.Product.ProductPriceInfo.CurrentPrice;
                        item.ProductID = gift.Product.ProductID;
                        item.ProductSysNo = gift.Product.SysNo;
                        item.PMUserSysNo = gift.PMUserSysNo;
                        gridVM.Add(item);
                    }
                    DataGrid.ItemsSource = gridVM;
                    if (gridVM[0].SysNo.Value == 0)
                        DataGrid.Columns[0].Visibility = System.Windows.Visibility.Collapsed;
                    if (rsp.GiftCardFabricationList != null)
                        tbTotal.Text = string.Format(total, rsp.GiftCardFabricationList.Count, rsp.TotalPrice, rsp.TotalCount);
                    else
                        tbTotal.Text = string.Format(total, 0, 0, 0);
                }
                else
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_CheckTheCardGiftProduct, MessageType.Information);

            });
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            GiftCardFabricationMaster item = new GiftCardFabricationMaster();
            List<GiftCardFabrication> list = new List<GiftCardFabrication>();

            item = VM.ConvertVM<GiftCardFabricationVM, GiftCardFabricationMaster>();
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            bool checkQuantity = false;
            foreach (GiftCardFabricationItemVM gift in gridVM)
            {
               
                GiftCardFabrication g = new GiftCardFabrication();
                g.Product = new ProductInfo();
                g.Product.ProductID = gift.ProductID;
                g.Quantity = gift.Quantity;
                g.Product.SysNo = gift.ProductSysNo.Value;
                g.MasterSysNo = VM.SysNo.Value;
                g.SysNo = gift.SysNo.Value;
                list.Add(g);
                if (gift.Quantity < 0)
                {
                    checkQuantity = false;
                    break;
                }
                if (gift.Quantity > 0)
                {
                    checkQuantity = true;
                }
            }
            if (!checkQuantity)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("每种面值的礼品卡数量都不能小于0，并且礼品卡总数量必须大于0！", MessageType.Error);
                return;
            }
            item.GiftCardFabricationList = list;
            facade.UpdateGiftCardFabrications(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_OperateSuccessful, MessageType.Information);
                if (Dialog != null)
                {
                    Dialog.ResultArgs.Data = null;
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                }
            });
        }

        /// <summary>
        /// 生成采购单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCreatePO_Click(object sender, RoutedEventArgs e)
        {
            GiftCardFabricationMaster item = new GiftCardFabricationMaster();
            List<GiftCardFabrication> list = new List<GiftCardFabrication>();

            item = VM.ConvertVM<GiftCardFabricationVM, GiftCardFabricationMaster>();
            
            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            foreach (GiftCardFabricationItemVM gift in gridVM)
            {
                if (gift.Quantity.HasValue && gift.Quantity.Value > 0)
                {
                    GiftCardFabrication g = new GiftCardFabrication();
                    g.Product = new ProductInfo();
                    g.Product.ProductID = gift.ProductID;
                    g.Quantity = gift.Quantity;
                    g.Product.SysNo = gift.ProductSysNo.Value;
                    g.MasterSysNo = VM.SysNo.Value;
                    g.PMUserSysNo = gift.PMUserSysNo;
                    list.Add(g);
                }
            }
           
            if (list.Count > 0)
            {
                item.GiftCardFabricationList = list;
                facade.CreatePOGiftCardFabrication(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    if (args.Result != 0)
                    {
                        VM.POSysNo = args.Result.ToString();
                        hlPOSysNo.Content = VM.POSysNo;
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_OperateSuccessful, MessageType.Information);
                        if (Dialog != null)
                        {
                            Dialog.ResultArgs.Data = null;
                            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            Dialog.Close();
                        }
                    }
                });
            }
            else
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_NeedGiftQuantity, MessageType.Warning);
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            facade.DeleteGiftCardFabrication(VM.SysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_OperateSuccessful, MessageType.Information);
                if (Dialog != null)
                {
                    Dialog.ResultArgs.Data = null;
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                }
            });
        }

        /// <summary>
        /// 导出卡号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExport_Click(object sender, RoutedEventArgs e)
        {
            //CreateGiftCardInfoList

            //ResetGiftCardFabrication
            //GiftCardFabricationMaster item = new GiftCardFabricationMaster();
            //item = VM.ConvertVM<GiftCardFabricationVM, GiftCardFabricationMaster>();
            //foreach (GiftCardFabricationItemVM gift in gridVM)
            //{
            //    item.GiftCardFabricationList.Add(gift.ConvertVM <GiftCardFabricationItemVM, GiftCardFabrication>());
            //}

            facade.GetAddGiftCardInfoList(VM.SysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                if (args.Result)
                {
                    ColumnSet col = new ColumnSet();
                    col.Insert(0, "TransactionNumber", ResGiftCardInfo.Column_SysNo);
                    col.Insert(1, "Code", ResGiftCardInfo.Column_CardSysNo);
                    col.Insert(2, "Password", ResGiftCardInfo.Column_Password);
                    col.Insert(3, "BarCode", ResGiftCardInfo.Column_BarCard);
                    col.Insert(4, "TotalAmount", ResGiftCardInfo.Column_Amount);
                    facade.ExportGiftCardExcelFile(VM.SysNo.Value, new ColumnSet[] { col });

                    if (Dialog != null)
                    {
                        Dialog.ResultArgs.Data = null;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    }
                }
                else
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResGiftCardInfo.Information_GiftCardIsInProcess, MessageType.Warning);
                //??????????
                //facade.ResetGiftCardFabrication(item.SysNo.Value, (obj2, args2) =>
                //{
                //    if (args2.FaultsHandle())
                //        return;

                //    CPApplication.Current.CurrentPage.Context.Window.Alert("操作成功！", MessageType.Information);
                //});
            });
        }

        /// <summary>
        /// 关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            }
        }

        private void hlPOSysNo_Click(object sender, RoutedEventArgs e)
        {
            if (hlPOSysNo.Content != null && !string.IsNullOrEmpty(hlPOSysNo.Content.ToString()))
            {
                CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ECCentral.Portal.Basic.ConstValue.PO_PurchaseOrderMaintain, hlPOSysNo.Content.ToString()), null, true);
                if (Dialog != null)
                {
                    Dialog.ResultArgs.Data = null;
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                }
            }
        }

        private void tbQuantity_LostFocus(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.DataGrid))
                return;
            var txt = sender as TextBox;
            var exp = txt.GetBindingExpression(TextBox.TextProperty);
            exp.UpdateSource();
            int totalCount = 0;
            decimal totalPrice = 0;
            foreach (GiftCardFabricationItemVM gift in gridVM)
            {
                if (gift.Quantity.HasValue && gift.Quantity.Value > 0)
                {
                    totalPrice += gift.Quantity.Value * gift.CurrentPrice.Value;
                    totalCount += gift.Quantity.Value;
                }
            }
            tbTotal.Text = string.Format(total, rsp.GiftCardFabricationList.Count, totalPrice.ToString(), totalCount);
        }
    }
}