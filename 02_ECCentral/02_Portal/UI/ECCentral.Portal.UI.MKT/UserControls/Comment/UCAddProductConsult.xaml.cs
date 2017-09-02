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
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Comment
{
    public partial class UCAddProductConsult : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        private ProductConsultQueryVM vm;
        private ProductConsultQueryFacade facade;
        /// <summary>
        /// 咨询对象
        /// </summary>
        private ProductConsult productConsult;

        /// <summary>
        /// 邮件日志对象
        /// </summary>
        private ProductReviewMailLog mailLog;
        /// <summary>
        /// 邮件日志vm
        /// </summary>
        private ProductReviewMailLogVM vmEmailLog;

        /// <summary>
        /// 厂商回复列表
        /// </summary>
        private List<ProductConsultReply> vendorReplyList;

        /// <summary>
        /// 选中的厂商回复
        /// </summary>
        private ProductConsultReply selectVendorReplyItem;

        public UCAddProductConsult()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddProductConsult_Loaded);
        }

        private void UCAddProductConsult_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddProductConsult_Loaded);
            //咨询状态,就是相对应的回复状态
            CodeNamePairHelper.GetList("MKT", "ConsultCategory", (obj, args) =>
            {
                if (args.FaultsHandle()) return;
                comConsultStatus.ItemsSource = args.Result;
            });
            facade = new ProductConsultQueryFacade(CPApplication.Current.CurrentPage);

            cbNeedAdditionalText.IsChecked = false;
            //咨询基本信息
            facade.LoadProductConsult(SysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                productConsult =  args.Result;
                vm = productConsult.Convert<ProductConsult, ProductConsultQueryVM>();
                BaseInfoLayout.DataContext = vm;
                btnReplyStackPanel.DataContext = new ProductConsultReplyQueryVM();
                if(vm.ProductReviewMailLog!=null)
                    tbTopicMailContent.Text = vm.ProductReviewMailLog.TopicMailContent.Content;//邮件回复
                vendorReplyList=  vm.VendorReplyList;
                FactoryReplyList.ItemsSource = vendorReplyList;
                ProductConsultReplyList.ItemsSource = vm.ProductConsultReplyList;
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
            if (string.IsNullOrEmpty(tbReplyContent.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_ConsultContentIsNotNull,Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
            else
            {
                vm.ReplyContent = tbReplyContent.Text;
                ProductConsultReply item = vm.ConvertVM<ProductConsultQueryVM, ProductConsultReply>();
                item.NeedAdditionalText = cbNeedAdditionalText.IsChecked.Value ? YNStatus.Yes : YNStatus.No;
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                item.ConsultSysNo = SysNo;
                facade.UpdateProductConsultDetailReply(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful,Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                    facade.LoadProductConsult(SysNo, (obj2, args2) =>
                    {
                        if (args2.FaultsHandle())
                            return;
                        productConsult = args2.Result;
                        vm = productConsult.Convert<ProductConsult, ProductConsultQueryVM>();
                        ProductConsultReplyList.ItemsSource = vm.ProductConsultReplyList;
                    });
                    tbReplyContent.Text = "";
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
        
            facade.BatchSetProductConsultValid(invalidSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_SettingSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                
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

            facade.BatchSetProductConsultInvalid(invalidSysNo, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_SettingSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                
            });
        }

        /// <summary>
        /// 回复邮件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReplyEmail_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(newTopicMailContent.Text))
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_MailContentIsNotNull, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);

            ProductReview prodoctReview=new ProductReview();

            ProductReviewMailLog item = new ProductReviewMailLog();
            item.Type = "C";
            item.CompanyCode = productConsult.CompanyCode;
            item.RefSysNo = SysNo;
            item.TopicMailContent = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, newTopicMailContent.Text);
            item.CSNote = new BizEntity.LanguageContent(ConstValue.BizLanguageCode,newTopicMailContent.Text);

            prodoctReview.ProductReviewMailLog = item;
            prodoctReview.ProductSysNo = int.Parse(vm.ProductSysNo);
            prodoctReview.CustomerSysNo = int.Parse(vm.CustomerSysNo);
            facade.UpdateProductConsultMailLog(prodoctReview, (obj, args) =>
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
        private void btnRelease_Click(object sender, RoutedEventArgs e)
        {
            //ProductConsultReply selectVendorReplyItem = FactoryReplyList.SelectedItem as ProductConsultReply;
            if (selectVendorReplyItem == null)
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_MoreThanOneRecord, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Warning);
                
            else
            {
                //ProductConsultReply item = FactoryReplyList.SelectedItem as ProductConsultReply;
                //ProductConsultReply item = vendorReplyList.SingleOrDefault(a => a.SysNo.Value == selectVendorReplyItem.SysNo.Value);
                selectVendorReplyItem.ReplyContent = tbRejectContent.Text;//暂时用于传递拒绝理由
                facade.ApproveProductConsultRelease(selectVendorReplyItem, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
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
            //ProductConsultReply item = FactoryReplyList.SelectedItem as ProductConsultReply;
            //ProductConsultReply item = vendorReplyList.SingleOrDefault(a => a.SysNo.Value == selectVendorReplyItem.SysNo.Value);
            if(selectVendorReplyItem!=null)
                selectVendorReplyItem.ReplyContent = ResComment.Content_FactoryReject;// string.Empty;//暂时用于传递拒绝理由


            facade.RejectProductConsultRelease(selectVendorReplyItem, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;

                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                
            });
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
                
            else
            {
                //ProductConsultReply item = FactoryReplyList.SelectedItem as ProductConsultReply;
                //ProductConsultReply item = vendorReplyList.SingleOrDefault(a => a.SysNo.Value == selectVendorReplyItem.SysNo.Value);
                selectVendorReplyItem.ReplyContent = tbRejectContent.Text;//暂时用于传递拒绝理由
                facade.RejectProductConsultRelease(selectVendorReplyItem, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
        }

        /// <summary>
        /// 选中厂商回复
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FactoryReplyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectVendorReplyItem = this.FactoryReplyList.SelectedItem as ProductConsultReply;
            if(selectVendorReplyItem.StatusValue=="O")
                tbVendorReply.Text = selectVendorReplyItem.ReplyContent;
        }
    }
}
