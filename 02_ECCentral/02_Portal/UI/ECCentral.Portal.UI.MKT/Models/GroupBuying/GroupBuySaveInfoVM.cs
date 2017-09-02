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

namespace ECCentral.Portal.UI.MKT.Models.GroupBuying
{
    public class GroupBuySaveInfoVM : ModelBase
    {
        private string m_ProductID;
        public string ProductID
        {
            get
            {
                return m_ProductID;
            }
            set
            {
                SetValue("ProductID", ref m_ProductID, value);
            }
        }

        public string ProductSysNo { get; set; }

        private decimal? m_Price1;
        public decimal? Price1
        {
            get
            {
                return m_Price1;
            }
            set
            {
                SetValue("Price1", ref m_Price1, value);
            }
        }
        private decimal? m_Price2;
        public decimal? Price2
        {
            get
            {
                return m_Price2;
            }
            set
            {
                SetValue("Price2", ref m_Price2, value);
            }
        }
        private decimal? m_Price3;
        public decimal? Price3
        {
            get
            {
                return m_Price3;
            }
            set
            {
                SetValue("Price3", ref m_Price3, value);
            }
        }

        public Visibility IsShowPrice1
        {
            get
            {
                return m_Price1.HasValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility IsShowPrice2
        {
            get
            {
                return m_Price2.HasValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }
        public Visibility IsShowPrice3
        {
            get
            {
                return m_Price3.HasValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private decimal? m_MarginRate1;
        public decimal? MarginRate1
        {
            get
            {
                return m_MarginRate1;
            }
            set
            {
                SetValue("MarginRate1", ref m_MarginRate1, value);
            }
        }
        private decimal? m_MarginRate2;
        public decimal? MarginRate2
        {
            get
            {
                return m_MarginRate2;
            }
            set
            {
                SetValue("MarginRate2", ref m_MarginRate2, value);
            }
        }
        private decimal? m_MarginRate3;
        public decimal? MarginRate3
        {
            get
            {
                return m_MarginRate3;
            }
            set
            {
                SetValue("MarginRate3", ref m_MarginRate3, value);
            }
        }

        private decimal? m_MarginDollar1;
        public decimal? MarginDollar1
        {
            get
            {
                return m_MarginDollar1;
            }
            set
            {
                SetValue("MarginDollar1", ref m_MarginDollar1, value);
            }
        }
        private decimal? m_MarginDollar2;
        public decimal? MarginDollar2
        {
            get
            {
                return m_MarginDollar2;
            }
            set
            {
                SetValue("MarginDollar2", ref m_MarginDollar2, value);
            }
        }
        private decimal? m_MarginDollar3;
        public decimal? MarginDollar3
        {
            get
            {
                return m_MarginDollar3;
            }
            set
            {
                SetValue("MarginDollar3", ref m_MarginDollar3, value);
            }
        }

        private decimal? m_Discount1;
        public decimal? Discount1
        {
            get
            {
                return m_Discount1;
            }
            set
            {
                SetValue("Discount1", ref m_Discount1, value);
            }
        }
        private decimal? m_Discount2;
        public decimal? Discount2
        {
            get
            {
                return m_Discount2;
            }
            set
            {
                SetValue("Discount2", ref m_Discount2, value);
            }
        }
        private decimal? m_Discount3;
        public decimal? Discount3
        {
            get
            {
                return m_Discount3;
            }
            set
            {
                SetValue("Discount3", ref m_Discount3, value);
            }
        }

        private decimal? m_SpareMoney1;
        public decimal? SpareMoney1
        {
            get
            {
                return m_SpareMoney1;
            }
            set
            {
                SetValue("SpareMoney1", ref m_SpareMoney1, value);
            }
        }
        private decimal? m_SpareMoney2;
        public decimal? SpareMoney2
        {
            get
            {
                return m_SpareMoney2;
            }
            set
            {
                SetValue("SpareMoney2", ref m_SpareMoney2, value);
            }
        }
        private decimal? m_SpareMoney3;
        public decimal? SpareMoney3
        {
            get
            {
                return m_SpareMoney3;
            }
            set
            {
                SetValue("SpareMoney3", ref m_SpareMoney3, value);
            }
        }

        private Visibility isShowConfirmInfo =  Visibility.Collapsed;
        public Visibility IsShowConfirmInfo 
		{ 
			get
			{
				return isShowConfirmInfo;
			}			
			set
			{
				SetValue("IsShowConfirmInfo", ref isShowConfirmInfo, value);
			} 
		}		
    }
}
