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

namespace ECCentral.Portal.UI.PO.Models
{
    public class PurchaseOrderMemoInfoVM : ModelBase
    {
        /// <summary>
        /// PM确认时间
        /// </summary>
        private DateTime? pMConfirmTime;

        public DateTime? PMConfirmTime
        {
            get { return pMConfirmTime; }
            set { this.SetValue("PMConfirmTime", ref pMConfirmTime, value); }
        }

        private string pMConfirmUserName;
        /// <summary>
        /// PM确认人
        /// </summary>
        public string PMConfirmUserName
        {
            get { return pMConfirmUserName; }
            set { this.SetValue("PMConfirmUserName", ref pMConfirmUserName, value); }
        }

        public string PMConfirmTimeAndUserName
        {
            get
            {
                if (pMConfirmTime.HasValue)
                {
                    return string.Format("{0}[{1}]", pMConfirmUserName, pMConfirmTime.Value.ToString("yyyy/MM/dd HH:mm:ss"));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// 备注
        /// </summary>
        private string note;

        public string Note
        {
            get { return note; }
            set { this.SetValue("Note", ref note, value); }
        }

        /// <summary>
        /// 采购单备忘录
        /// </summary>
        private string memo;

        public string Memo
        {
            get { return memo; }
            set { this.SetValue("Memo", ref memo, value); }
        }

        /// <summary>
        /// 拒绝理由
        /// </summary>
        private string refuseMemo;

        public string RefuseMemo
        {
            get { return refuseMemo; }
            set { this.SetValue("RefuseMemo", ref refuseMemo, value); }
        }

        /// <summary>
        /// 入库备注
        /// </summary>
        private string inStockMemo;

        public string InStockMemo
        {
            get { return inStockMemo; }
            set { this.SetValue("InStockMemo", ref inStockMemo, value); }
        }

        /// <summary>
        /// PM申请理由
        /// </summary>
        private string pMRequestMemo;

        public string PMRequestMemo
        {
            get { return pMRequestMemo; }
            set { this.SetValue("PMRequestMemo", ref pMRequestMemo, value); }
        }

        /// <summary>
        /// TL申请理由
        /// </summary>
        private string tLRequestMemo;

        public string TLRequestMemo
        {
            get { return tLRequestMemo; }
            set { this.SetValue("TLRequestMemo", ref tLRequestMemo, value); }
        }
    }
}
