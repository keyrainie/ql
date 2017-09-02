using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.Portal.UI.IM.Models;
using ECCentral.Portal.UI.IM.Views.ProductDomain;
using ECCentral.Service.IM.Restful.RequestMsg;

using Newegg.Oversea.Silverlight.ControlPanel.Core;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCBatchUpdatePM : UserControl
    {
        public ProductDomainManagement Page { get; private set; }

        public IDialog Dialog { get; set; }       

        public List<ProductDepartmentCategoryVM> CategoryList { get; private set; }

        public UCBatchUpdatePM()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCBatchUpdatePM_Loaded);
        }       

        public UCBatchUpdatePM(ProductDomainManagement page, List<ProductDepartmentCategoryVM> categoryList)
            : this()
        {
            this.Page = page;
            this.CategoryList = categoryList;
        }

        void UCBatchUpdatePM_Loaded(object sender, RoutedEventArgs e)
        {
            if (this.Page.Filter.IsSearchEmptyCategory)
            {
                tbDomain.Visibility = System.Windows.Visibility.Visible;
                cmbDomains.Visibility = System.Windows.Visibility.Visible;

                new ProductDomainFacade(CPApplication.Current.CurrentPage).LoadDomainForListing((obj, args) =>
                {                    
                    cmbDomains.ItemsSource = args.Result;
                    cmbDomains.SelectedIndex = 0;
                });
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (this.ucPMPicker.SelectedPMSysNo == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("请选择PM!", MessageType.Warning);
                return;
            }
            //查询空二级类得时候才会显示Domain列表
            BatchUpdatePMReq req = new BatchUpdatePMReq
            {                
                ProductDomainSysNo = cmbDomains.Visibility == System.Windows.Visibility.Visible ? int.Parse(cmbDomains.SelectedValue.ToString()) : default(int?),
                PMSysNo = ucPMPicker.SelectedPMSysNo
            };
            this.CategoryList.ForEach(p =>
            {
                var category = new ProductDepartmentCategory { SysNo = p.SysNo, C2SysNo = p.C2SysNo };
                req.DepartmentCategoryList.Add(category);
            });

            new ProductDomainFacade(this.Page).BatchUpdatePM(req, (obj, args) =>
            {
                this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK };
                this.Dialog.Close();
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }

        private void ucPMPicker_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.ucPMPicker.SelectedPMSysNo > 0 && !string.IsNullOrEmpty(this.ucPMPicker.BackupUserList))
            {
                new ProductDomainFacade(CPApplication.Current.CurrentPage).GetUserListName(this.ucPMPicker.BackupUserList, (obj, args) =>
                {
                    this.txtBackupUserNameList.Text = args.Result;
                });
            }
            else
            {
                this.txtBackupUserNameList.Text = string.Empty;
            }
        }
    }
}
