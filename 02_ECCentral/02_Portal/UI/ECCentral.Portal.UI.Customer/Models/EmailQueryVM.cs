using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using ECCentral.BizEntity.Customer;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using System.Linq;
using ECCentral.BizEntity.Enum.Resources;
namespace ECCentral.Portal.UI.Customer.Models
{
    public class EmailQueryVM : ModelBase
    {
        public EmailQueryVM()
        {
            this.WebChannelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
            this.WebChannelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
            this.CompanyCode = CPApplication.Current.CompanyCode;
        }
        public string CompanyCode { get; set; }

        public List<UIWebChannel> WebChannelList { get; set; }

        private string _ChannelID;
        public string ChannelID
        {
            get { return _ChannelID; }
            set { base.SetValue("ChannelID", ref _ChannelID, value); }
        }
        private DateTime? _EndDateFrom;

        public DateTime? EndDateFrom
        {
            get { return _EndDateFrom; }
            set { base.SetValue("EndDateFrom", ref _EndDateFrom, value); }
        }
        private DateTime? _EndDateTo;

        public DateTime? EndDateTo
        {
            get { return _EndDateTo; }
            set { base.SetValue("EndDateTo", ref _EndDateTo, value); }
        }
        private string _Email;

        public string Email
        {
            get { return _Email; }
            set { base.SetValue("Email", ref _Email, value); }
        }
        private string _Title;

        public string Title
        {
            get { return _Title; }
            set { base.SetValue("Title", ref _Title, value); }
        }
        private EmailSendStatus? _Status;

        public EmailSendStatus? Status
        {
            get { return _Status; }
            set { base.SetValue("Status", ref _Status, value); }
        }
        private string _Source;
        public string Source
        {
            get { return _Source; }
            set { base.SetValue("Source", ref _Source, value); }
        }
    }
}
