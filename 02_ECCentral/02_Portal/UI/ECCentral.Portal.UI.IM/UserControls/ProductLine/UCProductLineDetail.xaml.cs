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
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.Portal.UI.IM.Models;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.IM.Facades;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Portal.UI.IM.UserControls
{
    public partial class UCProductLineDetail : UserControl
    {
        public IDialog Dialog { get; set; }

        public ProductLineVM VM { get; private set; }        

        public UCProductLineDetail()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCProductLineDetail_Loaded);
        }

        public UCProductLineDetail(ProductLineVM vm)
            : this()
        {
            this.VM = vm;
        }

        void UCProductLineDetail_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCProductLineDetail_Loaded);

            var queryFilter = new ProductManagerQueryFilter()
            {
                PMQueryType = ECCentral.BizEntity.Common.PMQueryType.AllValid.ToString(),
                UserName = CPApplication.Current.LoginUser.LoginName,
                CompanyCode = CPApplication.Current.CompanyCode
            };
            List<BackupPMUser> bakpmlist = new List<BackupPMUser>();
            new PMQueryFacade(CPApplication.Current.CurrentPage).QueryPMList(queryFilter, (obj, args) =>
            {
                args.Result.ForEach(p => {
                    BackupPMUser bakpm = new BackupPMUser();
                    bakpm.UserSysNo = p.SysNo;
                    bakpm.UserName = p.UserInfo.UserDisplayName;
                    bakpm.IsChecked = false;
                    if (this.VM.BackupPMList!=null&&this.VM.BackupPMList.Contains(p.SysNo))
                    {
                        bakpm.IsChecked = true;
                    }
                    
                    bakpmlist.Add(bakpm);
                });
                this.VM.BackupAllPMList = bakpmlist;
            });
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (ValidationManager.Validate(this.LayoutRoot))
            {
                var facade = new ProductLineFacade(CPApplication.Current.CurrentPage);
                this.VM.BackupPMList = this.VM.BackupAllPMList.Where(p => p.IsChecked).ToList().Select(p => p.UserSysNo).ToList();
                if (this.VM.SysNo > 0)
                {
                    this.VM.EditUser = CPApplication.Current.LoginUser.DisplayName;
                    facade.Update(this.VM, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        CloseDialog();
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("保存成功", MessageBoxType.Success);
                    });
                }
                else
                {
                    this.VM.InUser = CPApplication.Current.LoginUser.DisplayName;
                    facade.Create(this.VM, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                        {
                            return;
                        }
                        this.VM.SysNo = args.Result.SysNo;
                        CloseDialog();
                        CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show("创建成功", MessageBoxType.Success);
                    });               
                }
               
            }
        }

        private void CloseDialog()
        {
            this.VM.PMUserName = this.ucPMPicker.SelectedPMName;
            this.VM.C2Name = this.ucCategoryPicker.Category2Name;

            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.OK, Data = this.VM };
            this.Dialog.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Dialog.ResultArgs = new ResultEventArgs { DialogResult = DialogResultType.Cancel };
            this.Dialog.Close();
        }
        
        private void ucCategoryPicker_LoadCategoryCompleted(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(this.VM.PropertyFault))
            {
                CPApplication.Current.CurrentPage.Context.Window.MessageBox.Show(this.VM.PropertyFault,MessageBoxType.Information);
            }
            this.DataContext = this.VM;
        }
    }
}
