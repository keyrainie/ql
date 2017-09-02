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
using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.Portal.UI.WPMessage.Facades;
using ECCentral.Service.WPMessage.Restful.RequestMsg;
using ECCentral.WPMessage.BizEntity;
using ECCentral.Portal.UI.WPMessage;
using ECCentral.Portal.Basic.Utilities;
using ECCentral.Portal.UI.WPMessage.Models;
using ECCentral.WPMessage.QueryFilter;
using Newegg.Oversea.Silverlight.Controls.Components;

namespace ECCentral.Portal.UI.WPMessage.UserControls
{
    public partial class UCWPMessage : UserControl
    {
        public IDialog Dialog { get; set; }
        private IWindow CurrentWindow
        {
            get
            {
                return CPApplication.Current.CurrentPage.Context.Window;
            }
        }

        WPMessagFacade facade;
        WPMessageQueryFilter req = new WPMessageQueryFilter();
        List<WPMessageVM> VM;
         List<KeyValuePair<WPMessageStatus?, string>> statusList;
        public UCWPMessage()
        {
            InitializeComponent();
            Loaded += new RoutedEventHandler(UCWPMessage_Loaded);
        }

        private void UCWPMessage_Loaded(object sender, RoutedEventArgs e)
        {
            //Loaded -= new RoutedEventHandler(UCWPMessage_Loaded);
            facade = new WPMessagFacade(CPApplication.Current.CurrentPage);

            statusList = EnumConverter.GetKeyValuePairs<WPMessageStatus>(EnumConverter.EnumAppendItemType.All);
            statusList.RemoveAt(3);
            comProductStatus.ItemsSource = statusList;
            comProductStatus.SelectedIndex = 0;

            req.UserSysNo = CPApplication.Current.LoginUser.UserSysNo.Value;
            req.WPMessageStatus = (WPMessageStatus?)comProductStatus.SelectedValue;
            facade.GetCategoryByUserSysNo(req.UserSysNo,(s, args) =>
            {
                if (args.FaultsHandle())
                    return;
                List<WPMessageCategory> list = new List<WPMessageCategory>();
                list = args.Result;
                WPMessageCategory item = new WPMessageCategory();
                item.SysNo = 0;
                item.CategoryName = "-所有-";
                list.Insert(0, item);
                cbmMessageType.ItemsSource = list;
                req.CategorySysNo = Convert.ToInt32(cbmMessageType.SelectedValue);
                QueryResultGrid.Bind();
            });
        }

        private void QueryResultGrid_LoadingDataSource(object sender, Newegg.Oversea.Silverlight.Controls.Data.LoadingDataEventArgs e)
        {
            if (req.CategorySysNo == 0)
                req.CategorySysNo = null;

            req.PageIndex = e.PageIndex;
            req.PageSize = e.PageSize;
            req.SortField = e.SortField;
            facade.GetCurrentUserWPMessage(req, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                VM = DynamicConverter<WPMessageVM>.ConvertToVMList<List<WPMessageVM>>(args.Result.Rows);
                QueryResultGrid.ItemsSource = VM;
                QueryResultGrid.TotalCount = args.Result.TotalCount;
            });
        }

        private void hlbOperate_Click(object sender, RoutedEventArgs e)
        {
            WPMessageVM item = this.QueryResultGrid.SelectedItem as WPMessageVM;
            facade.UpdateMessageToProcessing(item.SysNo.Value, (obj, args) =>
            {
                if (args.FaultsHandle())
                    return;
                CPApplication.Current.CloseRequestPanel();
                CPApplication.Current.CurrentPage.Context.Window.Navigate("/"+ item.Url + item.Parameters,null,true);
            });
        }

        private void Button_Search_Click(object sender, RoutedEventArgs e)
        {
            req.CategorySysNo = Convert.ToInt32( cbmMessageType.SelectedValue);
            req.BeginCreateTime = dpkStartDate1.SelectedDateStart;
            req.EndCreateTime = dpkStartDate1.SelectedDateEnd;
            req.WPMessageStatus = (WPMessageStatus?)comProductStatus.SelectedValue;
            //if(Convert.ToInt32(cbmMessageType.SelectedValue)!=0)
            //    req.CategorySysNo= Convert.ToInt32(cbmMessageType.SelectedValue);
            QueryResultGrid.Bind();
        }

    }
}
