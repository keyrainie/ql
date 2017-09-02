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
using ECCentral.Portal.Basic.Utilities;
using ECCentral.BizEntity.MKT;
using ECCentral.Portal.Basic;
using ECCentral.Portal.UI.MKT.Resources;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.UserControls.Keywords
{
    public partial class UCAddDefaultKeywords : UserControl
    {
        public IDialog Dialog { get; set; }
        public int SysNo { get; set; }
        private bool isAdd = true;
        public DefaultKeywordsVM VM { get; set; }
        private DefaultKeywordsQueryFacade facade;

        //PageType控件有时在数据还有加载回来的时候已经执行了Completed事件 导致vm为null 加这两个属性可以从页面上传过来
        public int? PageType { private get; set; }
        public int? PageID { private get; set; }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseDialog(DialogResultType.Cancel);
        }

        private void CloseDialog(DialogResultType dialogResult)
        {
            if (Dialog != null)
            {
                Dialog.ResultArgs.DialogResult = dialogResult;
                Dialog.Close();
            }
        }

        public UCAddDefaultKeywords()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCAddDefaultKeywords_Loaded);
            
        }

        private void UCAddDefaultKeywords_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= new RoutedEventHandler(UCAddDefaultKeywords_Loaded);
            facade = new DefaultKeywordsQueryFacade(CPApplication.Current.CurrentPage);
            comStatus.ItemsSource = EnumConverter.GetKeyValuePairs<ADStatus>();
            if (SysNo > 0)
            {
                //编辑模式
                isAdd = false;
                this.ucPageType.PageTypeLoadCompleted += new EventHandler(ucPageType_PageTypeLoadCompleted);
                this.ucPageType.PageLoadCompleted += new EventHandler(ucPageType_PageLoadCompleted);
                facade.LoadDefaultKeywordsInfo(SysNo, (obj, args) =>
                {
                    if (args.FaultsHandle())
                    {
                        return;
                    }
                    VM = args.Result.Convert<DefaultKeywordsInfo, DefaultKeywordsVM>((info, vm) =>
                    {
                        if (info.WebChannel != null)
                        {
                            vm.ChannelID = info.WebChannel.ChannelID;
                        }
                    });
                    LayoutRoot.DataContext = VM;
                   
                });
            }
            else
            {
                //新建模式
                VM = new DefaultKeywordsVM();
                VM.Status = ADStatus.Active;
                LayoutRoot.DataContext = VM;
                //创建时默认选中第一个渠道
                this.lstChannel.SelectedIndex = 0;
            }
        }


        public void ucPageType_PageLoadCompleted(object sender, EventArgs e)
        {
               ucPageType.SetPageID(PageID);
        }

        public void ucPageType_PageTypeLoadCompleted(object sender, EventArgs e)
        {
                ucPageType.SetPageType(PageType);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidationManager.Validate(this))
                return;
            DefaultKeywordsInfo item = EntityConvertorExtensions.ConvertVM<DefaultKeywordsVM, DefaultKeywordsInfo>(VM, (v, t) =>
            {
                t.Keywords = new BizEntity.LanguageContent(ConstValue.BizLanguageCode, v.Keywords);
            });

            item.CompanyCode = Newegg.Oversea.Silverlight.ControlPanel.Core.CPApplication.Current.CompanyCode;
            //从控件取页面类型相关的值
            item.PageType = this.ucPageType.PageType;
            var pType =PageTypeUtil.ResolvePresentationType(ModuleType.DefaultKeywords, this.ucPageType.PageType.ToString());
            if (item.PageType==0)
            {
                item.PageID = this.ucPageType.PageID ?? 0;
            }
            else
            {
                item.PageID = this.ucPageType.PageID ?? -1;
            }
            item.Extend = this.ucPageType.IsExtendValid;
            if (item.PageType == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("页面类型不能为空!",MessageType.Error);
                return;
            }
            if (item.BeginDate == null || item.EndDate == null)
            {
                CPApplication.Current.CurrentPage.Context.Window.Alert("开始时间和结束时间不能为空!", MessageType.Error);
                return;
            }
            if (item.BeginDate != null && item.EndDate != null)
            {
                if (item.EndDate.Value.CompareTo(item.BeginDate) <= 0)
                {
                    CPApplication.Current.CurrentPage.Context.Window.Alert("结束时间不能小于开始时间!", MessageType.Error);
                    return;
                }
            }
     

            if (isAdd)
            {
                facade.AddDefaultKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CloseDialog(DialogResultType.OK);
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_CreateSuccessful, MessageType.Information);
                });
            }
            else
            {
                item.SysNo = SysNo;
                facade.EditDefaultKeywords(item, (obj, args) =>
                {
                    if (args.FaultsHandle())
                        return;
                    CloseDialog(DialogResultType.OK);
                    CPApplication.Current.CurrentPage.Context.Window.Alert(ResKeywords.Information_UpdateSuccessful, MessageType.Information);
                });

            }
        }
    }
}
