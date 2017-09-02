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

namespace ECCentral.Portal.UI.Customer.Models
{
    public class SecurityQuestionViewModel : ModelBase
    {
        public string Question1 { get; set; }

        public string Question2 { get; set; }

        public string Question3 { get; set; }

        public string Answer1 { get; set; }

        public string Answer2 { get; set; }

        public string Answer3 { get; set; }

        #region 页面显示
        public string Question1Str
        {
            get
            {
                if (string.IsNullOrEmpty(Question1))
                    Question1 = "未设置";
                return Question1;
            }
        }

        public string Question2Str
        {
            get
            {
                if (string.IsNullOrEmpty(Question2))
                    Question2 = "未设置";
                return Question2;
            }
        }

        public string Question3Str
        {
            get
            {
                if (string.IsNullOrEmpty(Question3))
                    Question3 = "未设置";
                return Question3;
            }
        }

        public string Answer1Str 
        {
            get
            {
                if (string.IsNullOrEmpty(Answer1))
                    Answer1 = "未设置";
                return Answer1;
            }
        }

        public string Answer2Str
        {
            get
            {
                if (string.IsNullOrEmpty(Answer2))
                    Answer2 = "未设置";
                return Answer2;
            }
        }

        public string Answer3Str
        {
            get
            {
                if (string.IsNullOrEmpty(Answer3))
                    Answer3 = "未设置";
                return Answer3;
            }
        }
        #endregion
    }
}
