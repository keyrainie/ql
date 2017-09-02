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
using ECCentral.Portal.UI.MKT.Models;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.MKT.UserControls.Comment
{
    public partial class UCAddReviewScoreItem : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        private bool isAdd = true;
        private ReviewScoreItemVM vm;
        private ReviewScoreItemFacade facade;
        public UCAddReviewScoreItem()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddReviewScoreItem_Loaded);
        }

        private void UCAddReviewScoreItem_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddReviewScoreItem_Loaded);
            //vm = new Models.ReviewScoreItemQueryVM();
            //LayoutRoot.DataContext = vm;

            facade = new ReviewScoreItemFacade(CPApplication.Current.CurrentPage);
            if (SysNo > 0)
            {
                isAdd = false;
                facade.LoadReviewScoreItem(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    vm = args.Result.Convert<ReviewScoreItem, ReviewScoreItemVM>();
                    ucCategory.LoadCategoryCompleted += InitCategory;
                });
            }
            else
            {
                vm = new ReviewScoreItemVM();
                vm.Status = ADStatus.Deactive;
                LayoutRoot.DataContext = vm;
            }
        }

        private void InitCategory(object sender, EventArgs e)
        {
            if (vm != null && vm.C3SysNo.HasValue)
            {
                ucCategory.Category3SysNo = vm.C3SysNo.Value;
                vm.C2SysNo = ucCategory.Category2SysNo.Value;
                vm.C1SysNo = ucCategory.Category1SysNo.Value;
                LayoutRoot.DataContext = vm;
            }
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            vm = LayoutRoot.DataContext as ReviewScoreItemVM;
            ReviewScoreItem item = EntityConvertorExtensions.ConvertVM<ReviewScoreItemVM, ReviewScoreItem>(vm, (v, t) =>
            {
                t.Name = new BizEntity.LanguageContent(ECCentral.Portal.Basic.ConstValue.BizLanguageCode, v.Name);
            });

            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            if (isAdd)
            {
                facade.CreateReviewScoreItem(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_CreateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
            else
            {
                item.SysNo = SysNo;
                facade.UpdateReviewScoreItem(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return; 
                    
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_UpdateSuccessful, Newegg.Oversea.Silverlight.Controls.Components.MessageType.Information);
                });
            }
        }
    }
}
