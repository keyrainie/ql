using System.Linq;
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
using Newegg.Oversea.Silverlight.Controls;
using ECCentral.Portal.UI.IM.Views.ProductLine;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCBatchUpdateProductLine : UserControl
    {
        public ProductLineManagement Page 
        {
            get 
            {
                return CPApplication.Current.CurrentPage as ProductLineManagement;
            }
        }

        private BatchUpdatePMVM VM;

        public IDialog Dialog { get; set; }

        public List<ProductLineVM> Category2List { get; private set; }

        public UCBatchUpdateProductLine()
        {
            InitializeComponent();

            Loaded += new RoutedEventHandler(UCBatchUpdateProductLine_Loaded);

            this.VM = new BatchUpdatePMVM();
            this.VM.InUser = CPApplication.Current.LoginUser.DisplayName;
            this.VM.CompanyCode = CPApplication.Current.CompanyCode;
            this.VM.LanguageCode = CPApplication.Current.LanguageCode;
        }

        public UCBatchUpdateProductLine(List<ProductLineVM> category2List)
            : this()
        {
            this.VM.ProductLineList = category2List;
        }

        void UCBatchUpdateProductLine_Loaded(object sender, RoutedEventArgs e)
        {
            this.VM.IsEmptyC2Create = this.Page.Filter.IsSearchEmptyCategory;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            Page.Window.MessageBox.Clear();
            if (this.VM.ProductLineList == null || this.VM.ProductLineList.Count==0)
            {
                string infostring = "请先选择批量更新的数据，之后再进行更新操作!";
                if (this.VM.IsEmptyC2Create)
                {
                    infostring = "请先选择批量创建的数据，之后再进行更新操作!";
                }
                Page.Window.MessageBox.Show(infostring, MessageBoxType.Warning);
                return;
            }
            if (this.VM.IsEmptyC2Create)
            {
                if (this.ucPMPicker.SelectedPMSysNo == null)
                {
                    Page.Window.MessageBox.Show("请选择产品经理!", MessageBoxType.Warning);
                    return;
                }
                if (this.ucMerchandiserPicker.SelectedPMSysNo == null)
                {
                    Page.Window.MessageBox.Show("请选择跟单员!", MessageBoxType.Warning);
                    return;
                }
            }
            else
            {
                this.VM.BakPMUpdateType = "Append";
                if(OperatorType2.IsChecked.Value)
                {
                    this.VM.BakPMUpdateType = "Remove";
                }
            }
            this.VM.BackupPMList = this.VM.BackupAllPMList.Where(p => p.IsChecked).ToList().Select(p => p.UserSysNo).ToList();
            ProductLineFacade facade = new ProductLineFacade();
            facade.BatchUpdate(this.VM,(obj,args)=>{
                MessageBoxType messagetype = MessageBoxType.Success;
                string errorstring = string.Empty;
                if (args.Error != null)
                {
                    bool isBizException = true;
                    messagetype = MessageBoxType.Warning;
                    errorstring = GetError(args.Error, ref isBizException);
                    if (!isBizException)
                    {
                        messagetype = MessageBoxType.Error;
                    }
                }
                else
                {
                    errorstring = "更新成功";
                    if (this.VM.IsEmptyC2Create)
                    {
                        errorstring = "创建成功";
                    }
                    this.Dialog.Close();
                }
                Page.Window.MessageBox.Show(errorstring, messagetype);
                Page.ListControl.BindData(Page.Filter);
            });
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }

        private void ucPMPicker_PMLoaded(object sender, System.EventArgs e)
        {
            this.VM.BackupAllPMList = this.ucPMPicker.itemList.Select<ProductManagerInfo, BackupPMUser>(p =>
            {
                BackupPMUser pmuser = new BackupPMUser();
                pmuser.UserSysNo = p.SysNo;
                pmuser.UserName = p.UserInfo.UserDisplayName;
                return pmuser;
            }).ToList<BackupPMUser>();
            if (this.VM.BackupAllPMList.Count > 0)
            {
                this.VM.BackupAllPMList.RemoveAt(0);
            }
            this.DataContext = this.VM;
        }

        private string GetError(RestServiceError error, ref bool isBizException)
        {
            StringBuilder build = new StringBuilder();
            foreach (Error item in error.Faults)
            {
                if (isBizException && !item.IsBusinessException)
                {
                    isBizException = false;
                }
                build.Append(string.Format("{0}", item.ErrorDescription));
            }
            return build.ToString();
        }
    }
}
