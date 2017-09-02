using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
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
using ECCentral.QueryFilter.PO;
using ECCentral.BizEntity.PO;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.Controls.Data;
using ECCentral.Portal.UI.MKT.Facades;
using ECCentral.Portal.Basic;
using System.Windows.Media.Imaging;
using System.Text;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.BizEntity.MKT;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCProductReview : UserControl
    {
        public IDialog Dialog { get; set; }

        ProductReviewVM newVM = new ProductReviewVM();

        public ProductReviewQueryFacade serviceFacade;
     
        public IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        public UCProductReview()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(_Loaded);
        }

      
        void _Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= _Loaded;
            this.DataContext = newVM;
            serviceFacade = new ProductReviewQueryFacade(CPApplication.Current.CurrentPage);          
          
        }
       
      
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
           
            Dialog.ResultArgs.Data = null;
            Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
            Dialog.Close(true);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
            {
                return;
            }
            newVM.Score = (newVM.Score1 + newVM.Score2 + newVM.Score3 + newVM.Score4) / 4.0m;
            var model = newVM.ConvertVM<ProductReviewVM, ProductReview>();          
            serviceFacade.CreateProductReview(newVM, (s, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                if (args.Result.SysNo.HasValue && args.Result.SysNo.Value > 0)
                {
                    Dialog.ResultArgs.Data = args.Result;
                }
                else
                {
                    Dialog.ResultArgs.Data = null;
                    CurrentWindow.Alert("添加评论失败");
                }
                Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                Dialog.Close(true);

            });
           
        }      
       
    }
}
   
   
