using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.ControlPanel.Core;
using ECCentral.BizEntity.Enum.Resources;
using System.Linq;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class HomePageSectionQueryVM:ModelBase
    {
        private string _companyCode;
        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode
        {
            get { return _companyCode; }
            set
            {
                base.SetValue("CompanyCode", ref _companyCode, value);
            }
        }
        private string _channelID;
        /// <summary>
        /// 渠道编号
        /// </summary>
        public string ChannelID
        {
            get { return _channelID; }
            set
            {
                base.SetValue("ChannelID", ref _channelID, value);
            }
        }

        /// <summary>
        /// 渠道列表
        /// </summary>
        public List<UIWebChannel> ChannelList
        {
            get
            {
                var channelList = CPApplication.Current.CurrentWebChannelList.ToList<UIWebChannel>();
                channelList.Insert(0, new UIWebChannel { ChannelName = ResCommonEnum.Enum_All });
                if (channelList.Count > 1)
                {
                    ChannelID = channelList[1].ChannelID;
                }
                return channelList;
            }
        }
    }
}
