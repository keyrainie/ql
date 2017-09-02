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
using ECCentral.BizEntity.MKT;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace ECCentral.Portal.UI.MKT.Models
{
    public class BannerInfoVM : ModelBase
    {
        private int _sysNo;
        /// <summary>
        /// 系统编号
        /// </summary>
        public int SysNo
        {
            get { return _sysNo; }
            set
            {
                base.SetValue("SysNo", ref _sysNo, value);
            }
        }
        private BannerType _bannerType;
        /// <summary>
        /// 广告类型
        /// </summary>
        [Validate(ValidateType.Required)]
        [Validate(ValidateType.MaxLength, 100)]
        public BannerType BannerType
        {
            get { return _bannerType; }
            set
            {
                base.SetValue("BannerType", ref _bannerType, value);               
            }
        }
        private string _bannerTitle;
        /// <summary>
        /// 广告标题
        /// </summary>
        [Validate(ValidateType.Required)]
        public string BannerTitle
        {
            get { return _bannerTitle; }
            set
            {
                base.SetValue("BannerTitle", ref _bannerTitle, value);
            }
        }
        private string _bannerText;
        /// <summary>
        /// 广告内容
        /// </summary>
        public string BannerText
        {
            get { return _bannerText; }
            set
            {
                base.SetValue("BannerText", ref _bannerText, value);
            }
        }
        private string _bannerResourceUrl;
        /// <summary>
        /// 资源地址
        /// </summary>
        [Validate(ValidateType.URL)]
        public string BannerResourceUrl
        {
            get { return _bannerResourceUrl; }
            set
            {
                base.SetValue("BannerResourceUrl", ref _bannerResourceUrl, value);
            }
        }

        private string _bannerResourceUrl2;
        /// <summary>
        /// 资源地址2
        /// </summary>
        [Validate(ValidateType.URL)]
        public string BannerResourceUrl2
        {
            get { return _bannerResourceUrl2; }
            set
            {
                base.SetValue("BannerResourceUrl2", ref _bannerResourceUrl2, value);
            }
        }


        /// <summary>
        /// Html模板
        /// </summary>
        public int? BannerFrameSysNo
        {
            get;
            set;
        }

        private string _bannerLink;
        /// <summary>
        /// 链接地址
        /// </summary>
        [Validate(ValidateType.URL)]
        public string BannerLink
        {
            get { return _bannerLink; }
            set
            {
                base.SetValue("BannerLink", ref _bannerLink, value);
            }
        }
        private string _bannerOnClick;
        /// <summary>
        /// 广告脚本
        /// </summary>
        public string BannerOnClick
        {
            get { return _bannerOnClick; }
            set
            {
                base.SetValue("BannerOnClick", ref _bannerOnClick, value);
            }
        }
        private string _description;
        /// <summary>
        /// 描述
        /// </summary>
        public string Description
        {
            get { return _description; }
            set
            {
                base.SetValue("Description", ref _description, value);
            }
        }



        #region UI扩展属性


        //public int? PageType { get; set; }
        //public string PositionID { get; set; }

        //public void SetVisibility()
        //{
        //    BannerLinkVisibility = System.Windows.Visibility.Visible;

        //    BannerResourceUrlVisibility = System.Windows.Visibility.Visible;

        //    BannerResourceUrl2Visibility = System.Windows.Visibility.Visible;

        //    BannerTextVisibility = System.Windows.Visibility.Visible;


        //    BannerResourceUrlTitle = "资源地址：";
        //    BannerResourceUrl2Title = "";
        //    BannerResourceUrlMemo = "";
        //    BannerResourceUrl2Memo = "";

        //    if (BannerType == BannerType.Image)
        //    {
        //        if (PageType == 0 && (PositionID == "227" || PositionID == "221"))
        //        {
        //            BannerResourceUrlVisibility = System.Windows.Visibility.Collapsed;

        //            BannerResourceUrl2Visibility = System.Windows.Visibility.Collapsed;


        //            BannerResourceUrlTitle = "资源地址A：";
        //            BannerResourceUrl2Title = "资源地址B：";

        //            if (PositionID == "227")
        //            {
        //                BannerResourceUrlMemo = "(默认广告图)";
        //                BannerResourceUrl2Memo = "(鼠标移入时广告图)";
        //            }
        //            else
        //            {
        //                BannerResourceUrlMemo = "(宽屏图片800*147)";
        //                BannerResourceUrl2Memo = "(窄屏图片560*147)";
        //            }
        //        }
        //        else
        //        {
        //            BannerResourceUrlVisibility = System.Windows.Visibility.Collapsed;
        //        }

        //        BannerLinkVisibility = System.Windows.Visibility.Collapsed;
        //    }
        //    else if (BannerType == BannerType.Flash || BannerType == BannerType.Video)
        //    {
        //        BannerLinkVisibility = System.Windows.Visibility.Collapsed;

        //        BannerResourceUrlVisibility = System.Windows.Visibility.Collapsed;

        //    }
        //    else if (BannerType == BannerType.HTML)
        //    {
        //        BannerLinkVisibility = System.Windows.Visibility.Collapsed;
        //        BannerTextVisibility = System.Windows.Visibility.Collapsed;
        //    }
        //    else if (BannerType == BannerType.Text)
        //    {
        //        BannerTextVisibility = System.Windows.Visibility.Collapsed;
        //    }
        //}

        //private string _bannerResourceUrlTitle = "资源地址：";
        ///// <summary>
        ///// 资源地址标题
        ///// </summary>
        //public string BannerResourceUrlTitle
        //{
        //    get { return _bannerResourceUrlTitle; }
        //    set
        //    {
        //        base.SetValue("BannerResourceUrlTitle", ref _bannerResourceUrlTitle, value);
        //    }
        //}

        //private string _bannerResourceUrl2Title = "资源地址B：";
        ///// <summary>
        ///// 资源地址2标题
        ///// </summary>
        //public string BannerResourceUrl2Title
        //{
        //    get { return _bannerResourceUrl2Title; }
        //    set
        //    {
        //        base.SetValue("BannerResourceUrl2Title", ref _bannerResourceUrl2Title, value);
        //    }
        //}

        //private string _bannerResourceUrlMemo;
        ///// <summary>
        ///// 资源地址说明
        ///// </summary>
        //public string BannerResourceUrlMemo
        //{
        //    get { return _bannerResourceUrlMemo; }
        //    set
        //    {
        //        base.SetValue("BannerResourceUrlMemo", ref _bannerResourceUrlMemo, value);
        //    }
        //}

        //private string _bannerResourceUrl2Memo;
        ///// <summary>
        ///// 资源地址2说明
        ///// </summary>
        //public string BannerResourceUrl2Memo
        //{
        //    get { return _bannerResourceUrl2Memo; }
        //    set
        //    {
        //        base.SetValue("BannerResourceUrl2Memo", ref _bannerResourceUrl2Memo, value);
        //    }
        //}
     

        //private Visibility _bannerLinkVisibility = Visibility.Visible;
        ///// <summary>
        ///// 链接地址是否可见
        ///// </summary>
        //public Visibility BannerLinkVisibility
        //{
        //    get { return _bannerLinkVisibility; }
        //    set
        //    {
        //        base.SetValue("BannerLinkVisibility", ref _bannerLinkVisibility, value);
        //    }
        //}
        //private Visibility _bannerResourceUrlVisibility = Visibility.Visible;
        ///// <summary>
        ///// 资源地址是否可见
        ///// </summary>
        //public Visibility BannerResourceUrlVisibility
        //{
        //    get { return _bannerResourceUrlVisibility; }
        //    set
        //    {
        //        base.SetValue("BannerResourceUrlVisibility", ref _bannerResourceUrlVisibility, value);
        //    }
        //}

        //private Visibility _bannerResourceUrl2Visibility = Visibility.Visible;
        ///// <summary>
        ///// 资源地址2是否可见
        ///// </summary>
        //public Visibility BannerResourceUrl2Visibility
        //{
        //    get { return _bannerResourceUrl2Visibility; }
        //    set
        //    {
        //        base.SetValue("BannerResourceUrl2Visibility", ref _bannerResourceUrlVisibility, value);
        //    }
        //}

        //private Visibility _bannerTextVisibility = Visibility.Collapsed;
        ///// <summary>
        ///// 广告内容是否可见
        ///// </summary>
        //public Visibility BannerTextVisibility
        //{
        //    get { return _bannerTextVisibility; }
        //    set
        //    {
        //        base.SetValue("BannerTextVisibility", ref _bannerTextVisibility, value);
        //    }
        //}
        #endregion
    }
}
