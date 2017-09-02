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

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.Models
{
    public class ControlPanelUser
    {
        public string LoginName { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SMSInfo
    {
        /// <summary>
        /// 手机号
        /// </summary>
        public string CellPhoneNum { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 发送优先级:1(L)～5(H)
        /// </summary>
        public int Priority { get; set; }
    }

    public class LoginCountRequest
    {
        public int Action { set; get; } // 0,insert; 1 read
        public string SystemNo { set; get; }
        public string InUser { set; get; }
    }
}
