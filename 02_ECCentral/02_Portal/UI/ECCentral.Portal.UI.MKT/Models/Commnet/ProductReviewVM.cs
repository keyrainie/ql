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
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class ProductReviewVM : ModelBase
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        
        private string productSysNo;
        [Validate(ValidateType.Required)]
        public string ProductSysNo
        {
            get { return productSysNo; }
            set { base.SetValue("ProductSysNo", ref productSysNo, value); }
        }        

        /// <summary>
        /// 顾客编号
        /// </summary>
        public string customerSysNo;
        [Validate(ValidateType.Required)]
        public string CustomerSysNo
        {
            get { return customerSysNo; }
            set
            {
                base.SetValue("CustomerSysNo", ref customerSysNo, value);
            }
        }
        /// <summary>
        /// 评论标题
        /// </summary>
       
        public string title;
        [Validate(ValidateType.Required)]
        public string Title
        {
            get { return title; }
            set
            {
                base.SetValue("Title", ref title, value);
            }
        }

        /// <summary>
        /// 优点
        /// </summary>
        public string prons;
        public string Prons
        {
            get { return prons; }
            set
            {
                base.SetValue("Prons", ref prons, value);
            }
        }

        /// <summary>
        /// 缺点
        /// </summary>
        public string cons;
        public string Cons
        {
            get { return cons; }
            set
            {
                base.SetValue("Cons", ref cons, value);
            }
        }

        /// <summary>
        /// 服务质量
        /// </summary>
        public string service;
        public string Service
        {
            get { return service; }
            set
            {
                base.SetValue("Service", ref service, value);
            }
        }

        

        public decimal Score { get; set; }

        public int score1;
        public int Score1
        {
            get { return score1; }
            set
            {
                base.SetValue("Score1", ref score1, value);
            }
        }

        public int score2;
        public int Score2
        {
            get { return score2; }
            set
            {
                base.SetValue("Score2", ref score2, value);
            }
        }

        public int score3;
        public int Score3
        {
            get { return score3; }
            set
            {
                base.SetValue("Score3", ref score3, value);
            }
        }
        public int score4;
        public int Score4
        {
            get { return score4; }
            set
            {
                base.SetValue("Score4", ref score4, value);
            }
        }      
    }
}
