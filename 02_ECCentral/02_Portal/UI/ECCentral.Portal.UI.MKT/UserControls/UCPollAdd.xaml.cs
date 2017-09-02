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
using System.Windows.Navigation;
using ECCentral.Portal.UI.MKT.Models;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using ECCentral.QueryFilter.MKT;
using ECCentral.Portal.UI.MKT.Facades;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.Basic;
using Newegg.Oversea.Silverlight.Controls.Components;
using ECCentral.Portal.UI.MKT.Resources;

namespace ECCentral.Portal.UI.MKT.UserControls
{
    public partial class UCPollAdd : UserControl
    {
        public IDialog Dialog { get; set; }
        private PollListVM model;
        private PollFacade facade;
        public int SysNo { get; set; }

        public UCPollAdd()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(UCPollAdd_Loaded);
        }

        void UCPollAdd_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCPollAdd_Loaded);
            facade = new PollFacade(CPApplication.Current.CurrentPage);
            if (SysNo > 0)
            {
                facade.LoadPollMaster(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;

                    model = args.Result.Convert<PollMaster, PollListVM>();
                    model.ChannelID = "1";
                    LayoutRoot.DataContext = model;

                    this.ucPageType.IsEnabled = model.PageType.HasValue && model.PageType != 4;

                    this.ucPageType.PageTypeLoadCompleted += new EventHandler(ucPageType_PageTypeLoadCompleted);
                    this.ucPageType.PageLoadCompleted += new EventHandler(ucPageType_PageLoadCompleted);
                });
            }
            else
            {
                model = new PollListVM();
                model.SysNo = "0";
                model.ChannelID = "1";
                model.Status = ADStatus.Deactive;
                LayoutRoot.DataContext = model;
            }
        }

        public void ucPageType_PageLoadCompleted(object sender, EventArgs e)
        {
            ucPageType.SetPageID(model.PageID);
        }

        public void ucPageType_PageTypeLoadCompleted(object sender, EventArgs e)
        {
            ucPageType.SetPageType(model.PageType);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this.LayoutRoot))
                return;
            if (!ucPageType.PageType.HasValue)
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_PageCategoryIsNotNull, MessageType.Warning);
            else if (ucPageType.PageType.Value>0 && ucPageType.PageType.Value <= 3 && !ucPageType.PageID.HasValue)
                CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_PageIDIsNotNull, MessageType.Warning);
            else
            {
                PollMaster item = EntityConvertorExtensions.ConvertVM<PollListVM, PollMaster>(model, (v, t) =>
                {
                    t.PollName = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.PollName);
                });
                item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
                item.PageType = ucPageType.PageType.Value;
                if (ucPageType.PageID.HasValue)
                    item.PageID = ucPageType.PageID.Value;
                else
                    item.PageID = -1;

                if (item.SysNo > 0)
                {
                    facade.UpdatePollMaster(item, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, MessageType.Information);
                        if (Dialog != null)
                        {
                            Dialog.ResultArgs.Data = item.SysNo;
                            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            Dialog.Close();
                        }
                    });
                
                }
                else
                {

                    facade.CreatePollMaster(item, (obj, args) =>
                    {
                        if (args.FaultsHandle())
                            return;
                        CPApplication.Current.CurrentPage.Context.Window.Alert(ResComment.Information_OperateSuccessful, MessageType.Information);
                        if (Dialog != null)
                        {
                            Dialog.ResultArgs.Data = int.Parse(args.Result.ToString()); ;
                            Dialog.ResultArgs.DialogResult = DialogResultType.OK;
                            Dialog.Close();
                        }
                    });
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.Data = null;
                Dialog.ResultArgs.DialogResult = DialogResultType.Cancel;
                Dialog.Close();
            }
        }
    }
}
