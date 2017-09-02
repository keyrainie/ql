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
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.Text;
using ECCentral.Portal.Basic;

namespace ECCentral.Portal.UI.MKT.UserControls.Comment
{
    public partial class UCEditProductReview : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        public int ReplyMode { get; set; }
        private ProductReviewQueryVM vm;
        private ProductReviewReplyQueryVM vmReply;
        private ProductReviewQueryFacade facade;
        private List<CommentImageVM> ImageVM;

        /// <summary>
        /// cs sysno
        /// </summary>
        public int ComplainSysNo { get; set; }

        /// <summary>
        /// 获取配置表中的配置数据（评论图片基础地址）
        /// </summary>
        public string ReviewMaintainImageUrl
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window.Configuration.GetConfigValue(ConstValue.DomainName_MKT, ConstValue.ConfigKey_External_ReviewMaintainImageUrl);
            }
        }
        /// <summary>
        /// 选中的厂商回复
        /// </summary>
        private ProductReviewReply selectVendorReplyItem;

        public UCEditProductReview()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCEditProductReview_Loaded);
        }

        private void UCEditProductReview_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCEditProductReview_Loaded);
            facade = new ProductReviewQueryFacade(CPApplication.Current.CurrentPage);
            switch (ReplyMode)
            {
                case 1:
                    AddReply.Visibility = Visibility.Visible;
                    cbNeedAdditionalText.IsChecked = false;
                    break;
                case 2:
                    CustomerMailReply.Visibility = Visibility.Visible;
                    break;
                case 3:
                    ManufacturerReply.Visibility = Visibility.Visible;
                    break;
                case 4:
                    AddReply.Visibility = Visibility.Visible;
                    CustomerMailReply.Visibility = Visibility.Visible;
                    ManufacturerReply.Visibility = Visibility.Visible;
                    break;
            }
            cbNeedAdditionalText.IsChecked = false;
            InitProductReivewDate();
            AddProductReviewReply.DataContext = vmReply;
        }

        public void InitProductReivewDate()
        {
            if (ComplainSysNo > 0)
                btnViewCSHandle.Visibility = System.Windows.Visibility.Visible;
            else
                btnCSHandle.Visibility = System.Windows.Visibility.Visible;
            
            facade.LoadProductReview(SysNo, (s, args) =>
            {
                if (args.FaultsHandle())
                    return;

                vm = args.Result.Convert<ProductReview, ProductReviewQueryVM>();
                //vm = DynamicConverter<ProductDiscussQueryVM>.ConvertToVMList<List<ProductDiscussQueryVM>>(args.Result);
                //ProductDiscussReplyList.ItemsSource = vm;

                ProductReviewReplyList.ItemsSource = vm.ProductReviewReplyList;
                VendorProductReviewReply.ItemsSource = vm.VendorReplyList;
                if (vm.ProductReviewMailLog != null)
                {
                    tbMailLog.Text = vm.ProductReviewMailLog.TopicMailContent != null ? vm.ProductReviewMailLog.TopicMailContent.Content : "";
                    tbCSNote.Text = vm.ProductReviewMailLog.CSNote != null ? vm.ProductReviewMailLog.CSNote.Content : "";
                }
                LayoutRoot.DataContext = vm;

                if (!string.IsNullOrEmpty(vm.ImageUrl))
                {
                    //string reviewMaintainImageUrl = ReviewMaintainImageUrl;
                    ImageVM = new List<CommentImageVM>();
                    string[] list = vm.ImageUrl.Split('|');
                    foreach (string url in list)
                    {
                        ImageVM.Add(new CommentImageVM() { ImageUrl = ReviewMaintainImageUrl + url, IsChecked = false });
                    }
                    ProductReivewImagesList.ItemsSource = ImageVM;

                    ProductReivewImagesList.Visibility = System.Windows.Visibility.Visible;
                    btnBatchDeleteImage.Visibility = System.Windows.Visibility.Visible;

                    ProductReivewImagesList.Bind();
                }
                else
                {
                    ProductReivewImagesList.Visibility = System.Windows.Visibility.Collapsed;
                    btnBatchDeleteImage.Visibility = System.Windows.Visibility.Collapsed;
                }
            });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbCSNote.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_BackupIsNotNull, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            else
            {
                ProductReviewMailLog log = new ProductReviewMailLog();
                log.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                log.CSNote = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, tbCSNote.Text);
                log.RefSysNo = this.SysNo;
                log.LanguageCode = ECCentral.Portal.Basic.ConstValue.BizLanguageCode;
                string mailContent = string.IsNullOrEmpty(tbTopicMailContent.Text) ? string.Empty : "--" + tbTopicMailContent.Text;
                log.TopicMailContent = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, mailContent);
                log.Type = "R";
                ProductReview productReview = vm.ConvertVM<ProductReviewQueryVM, ProductReview>();
                productReview.ProductReviewMailLog = log;
                facade.SaveProductReviewRemark(productReview, (s, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
        }

        /// <summary>
        /// 回复邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReplyMail_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(tbTopicMailContent.Text))
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_MailContentIsNotNull, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            }
            else
            {
                ProductReviewMailLog log = new ProductReviewMailLog();
                log.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                log.CSNote = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, tbCSNote.Text);
                log.RefSysNo = this.SysNo;
                log.LanguageCode = ECCentral.Portal.Basic.ConstValue.BizLanguageCode;
                string mailContent = string.IsNullOrEmpty(tbTopicMailContent.Text) ? string.Empty : "--" + tbTopicMailContent.Text;
                log.TopicMailContent = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, mailContent);
                log.Type = "R";

                ProductReview productReview = vm.ConvertVM<ProductReviewQueryVM, ProductReview>();
                productReview.ProductReviewMailLog = log;

                facade.UpdateProductReviewMailLog(productReview, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);

                });
            }
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVerify_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            invalidSysNo.Add(vm.SysNo.Value);
            facade.BatchSetProductReviewValid(invalidSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            });
        }

        /// <summary>
        /// 作废
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnVoid_Click(object sender, RoutedEventArgs e)
        {
            List<int> invalidSysNo = new List<int>();
            invalidSysNo.Add(vm.SysNo.Value);
            facade.BatchSetProductReviewInvalid(invalidSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            });
        }

        /// <summary>
        /// 提交CS处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCSHandle_Click(object sender, RoutedEventArgs e)
        {
            if (vm.SOSysno == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_NotExistsSOSysNo, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
            }
            else
            {
                ECCentral.BizEntity.SO.SOComplaintCotentInfo info = new BizEntity.SO.SOComplaintCotentInfo();
                info.ComplainTime = DateTime.Now;
                info.ComplainType = ResComment.Content_Other;
                info.ComplainSourceType = ResComment.Content_ProductReview;
                info.SOSysNo = vm.SOSysno.Value;
                info.ComplainContent = string.Format(ResComment.Content_ComplainContent, vm.Prons, vm.Cons, vm.Service); //"优点：" + vm.Prons + "\r\n缺点：" + vm.Cons + "\r\n其他：" + vm.Service;
                info.Subject = vm.Title;
                info.CustomerSysNo = int.Parse(vm.CustomerSysNo);
                info.CustomerEmail = string.Empty;//到service端去获取
                info.CustomerPhone = string.Empty;
                info.CustomerName = string.Empty;
                info.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                facade.SubmitReplyToCSProcess(info, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    if (Dialog != null)
                    {
                        Dialog.ResultArgs.Data = null;
                        Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                        Dialog.Close();
                    }
                });
            }
        }

        private void btnViewCSHandle_Click(object sender, RoutedEventArgs e)
        {
            CPApplication.Current.CurrentPage.Context.Window.Navigate(string.Format(ECCentral.Portal.Basic.ConstValue.SOComplainReplyFormat, ComplainSysNo.ToString()), null, true);
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close();
            }
        }

        /// <summary>
        /// 拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReject_Click(object sender, RoutedEventArgs e)
        {
            if (selectVendorReplyItem == null)
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_MoreThanOneRecord, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            else if (selectVendorReplyItem.StatusValue != "A")//与显示操作的条件一致
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_CannotOperateRecord, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            else
            {
                selectVendorReplyItem.Content = addReplyContent.Text;
                selectVendorReplyItem.Status = "D";
                selectVendorReplyItem.Type = "N";//泰隆优选特有
                facade.UpdateProductReviewVendorReplyStatus(selectVendorReplyItem, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    addReplyContent.Text = string.Empty; 
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                
                });
            }
        }
        
        /// <summary>
        /// grid中拒绝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void hlReject_Click(object sender, RoutedEventArgs e)
        {
            ProductReviewReply item = this.VendorProductReviewReply.SelectedItem as ProductReviewReply;

            item.Content = ResComment.Content_Reject;
            item.Status="D";
            item.ReviewSysNo=SysNo;
           //item.SysNo=0;//该条的sysno

            facade.UpdateProductReviewVendorReplyStatus(item, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                
            });
        }

        /// <summary>
        /// 批准发布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApproveRelease_Click(object sender, RoutedEventArgs e)
        {
            if (selectVendorReplyItem == null)
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_MoreThanOneRecord, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            else if (selectVendorReplyItem.StatusValue != "O")//与显示操作的条件一致
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_CannotOperateRecord, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            else
            {
                //ProductReviewReply item = this.VendorProductReviewReply.SelectedItem as ProductReviewReply;

                selectVendorReplyItem.Status = "A";
                selectVendorReplyItem.ReviewSysNo = SysNo;
                //item.SysNo = 0;//该条的sysno

                facade.UpdateProductReviewVendorReplyStatus(selectVendorReplyItem, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    selectVendorReplyItem = null; 
                    
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
        }

        /// <summary>
        ///  批量删除图片: 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBatchDeleteImage_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder updateImage = new StringBuilder();
            ImageVM.ForEach(item =>
            {
                if (!item.IsChecked)
                    updateImage.Append(item.ImageUrl).Append('|');
            });
            if (!string.IsNullOrEmpty(updateImage.ToString()))
            {
                if (updateImage.ToString().TrimEnd('|') != vm.ImageUrl)
                {
                    string param = updateImage.ToString().TrimEnd('|') + "!" + SysNo.ToString();//拼接SysNo
                    facade.DeleteProductReviewImage(param, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        InitProductReivewDate();
                    });
                }
                else
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResNewsInfo.Information_MoreThanOneRecord, MessageType.Error);
            }
        }

        private void ckbSelectRow_Click(object sender, RoutedEventArgs e)
        {
            var checkBoxAll = sender as CheckBox;
            if (ImageVM == null || checkBoxAll == null)
                return;
            ImageVM.ForEach(item =>
            {
                item.IsChecked = checkBoxAll.IsChecked ?? false;
            });
        }
       
        /// <summary>
        ///  添加回复: 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAddReply_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(addReplyContent.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_ReplyContentIsNotNull, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                
            else
            {
                ProductReviewReply item = new ProductReviewReply();
                item.Content = addReplyContent.Text;
                item.NeedAdditionalText = cbNeedAdditionalText.IsChecked.Value ? YNStatus.Yes : YNStatus.No;
                item.CustomerSysNo = int.Parse(vm.CustomerSysNo);
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                item.ReviewSysNo = SysNo;
                item.Status = "A";
                item.Type = "N";//泰隆优选特有
                facade.AddProductReviewReply(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    facade.LoadProductReview(SysNo, (s2, args2) =>
                    {
                        if (args2.FaultsHandle())
                            return;

                        vm = args2.Result.Convert<ProductReview, ProductReviewQueryVM>();
                        ProductReviewReplyList.ItemsSource = vm.ProductReviewReplyList;
                    });
                });
            }
        }

        /// <summary>
        /// 选中厂商回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VendorProductReviewReply_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectVendorReplyItem = this.VendorProductReviewReply.SelectedItem as ProductReviewReply;
            //tbVendorReply.Text = selectVendorReplyItem.Content;
        }

        private void ckb_SetTop_Checked(object sender, RoutedEventArgs e)
        {
            ckb_SetBottom.IsChecked = !ckb_SetTop.IsChecked;
        }

        private void ckb_SetBottom_Checked(object sender, RoutedEventArgs e)
        {
            ckb_SetTop.IsChecked = !ckb_SetBottom.IsChecked;
        }
    }
}
