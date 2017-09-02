using System.Windows;
using System.Windows.Controls;

using ECCentral.BizEntity.SO;
using ECCentral.Portal.UI.SO.Facades;

using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.SO.Models;
using ECCentral.Portal.Basic.Utilities;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.SO.Resources;
using ECCentral.QueryFilter.SO;
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.SO.UserControls
{
    public partial class publicMemoClose : UserControl
    {
        public IDialog Dialog { get; set; }
        public IPage Page { get; set; }

        SOInternalMemoFacade m_facade = new SOInternalMemoFacade();

        string m_oldNote = string.Empty;

        public publicMemoClose(int sysNo)
        {
            InitializeComponent();
            BindData(sysNo);
        }

        private void BindData(int sysNo)
        {
            SOInternalMemoQueryFilter query = new SOInternalMemoQueryFilter();
            query.SysNo = sysNo;
            query.PagingInfo = new PagingInfo();
            SOQueryFacade facade = new SOQueryFacade(Page);
            facade.QuerySOInternalMemo(query, (obj, args) =>
            {
                if (args.FaultsHandle())
                {
                    return;
                }
                SOInternalMemoInfoVM vm = DynamicConverter<SOInternalMemoInfoVM>.ConvertToVM(args.Result.Rows[0], "SourceSysNo", "Importance");
                this.DataContext = vm;
                this.m_oldNote = vm.Note;
                this.btnSave.Visibility = System.Windows.Visibility.Visible;
            });
        }

        private void Button_Save_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                var vm = this.DataContext as SOInternalMemoInfoVM;
                ValidationManager.Validate(LayoutRoot);
                if (vm.ValidationErrors.Count != 0) return;

                if (!string.IsNullOrEmpty(m_oldNote))
                {
                    vm.Note = string.Format(ResSOInternalMemo.Msg_NoteFormat, txtNote.Text.Trim(), m_oldNote);
                }
                else
                {
                    vm.Note = vm.Note.Trim();
                }
                int maxNoteLength = 500;
                if (vm.Note.Length > maxNoteLength)
                {
                    vm.Note = vm.Note.Substring(0, maxNoteLength);
                    CPApplication.Current.CurrentPage.Context.Window.Alert(string.Format(ResSOInternalMemo.Msg_LongMemoCutInfo, maxNoteLength));
                }
                m_facade.Close(vm.ConvertVM<SOInternalMemoInfoVM, SOInternalMemoInfo>(), (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                    Dialog.Close();
                });
            }
        }

        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }
    }
}
