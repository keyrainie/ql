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
using ECCentral.QueryFilter.Common;
using Newegg.Oversea.Silverlight.Utilities.Validation;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Inventory.Models
{
   public class PMMonitoringPerformanceIndicatorsQueryFilterVM : ModelBase
   {
       public List<KeyValuePair<int?, string>> StockList = new List<KeyValuePair<int?, string>>();
       public List<KeyValuePair<string, string>> AvailableSaledDaysConditionList = new List<KeyValuePair<string, string>>();
       public List<KeyValuePair<string, string>> AvgSaledQtyConditionList = new List<KeyValuePair<string, string>>();

       public PMMonitoringPerformanceIndicatorsQueryFilterVM()
        {
            this.PagingInfo = new PagingInfo();
            StockList.Add(new KeyValuePair<int?,string>(null,"--����--"));
            StockList.Add(new KeyValuePair<int?, string>(51, "�Ϻ���"));
            StockList.Add(new KeyValuePair<int?, string>(54, "�ɶ���"));
            StockList.Add(new KeyValuePair<int?, string>(55, "�人��"));
            StockList.Add(new KeyValuePair<int?, string>(53, "���ݲ�"));
            StockList.Add(new KeyValuePair<int?, string>(52, "������"));

            //AvailableSaledDaysConditionList.Add(new KeyValuePair<string, string>(">=", ">="));
            //AvailableSaledDaysConditionList.Add(new KeyValuePair<string, string>("=", "="));
            AvailableSaledDaysConditionList.Add(new KeyValuePair<string, string>("<=", "<="));

            AvgSaledQtyConditionList.Add(new KeyValuePair<string, string>(">=", ">="));
            AvgSaledQtyConditionList.Add(new KeyValuePair<string, string>("=", "="));
            AvgSaledQtyConditionList.Add(new KeyValuePair<string, string>("<=", "<="));
        }

       public PagingInfo PagingInfo { get; set; }

       private Boolean m_SearchByCategory;
       /// <summary>
       /// �Ƿ��������ѯ
       /// </summary>
       public Boolean SearchByCategory
       {
           get { return this.m_SearchByCategory; }
           set { this.SetValue("SearchByCategory", ref m_SearchByCategory, value);  }
       }

       private String m_SelectedPMSysNo;
       /// <summary>
       /// ��ѡ��� PM ���
       /// </summary>
       public String SelectedPMSysNo
       {
           get { return this.m_SelectedPMSysNo; }
           set { this.SetValue("SelectedPMSysNo", ref m_SelectedPMSysNo, value);  }
       }

       private String m_SelectedCategory1;
       /// <summary>
       /// ��ѡ���  C1 �� ���
       /// </summary>
       public String SelectedCategory1
       {
           get { return this.m_SelectedCategory1; }
           set { this.SetValue("SelectedCategory1", ref m_SelectedCategory1, value);  }
       }

       private String m_SelectedCategory2;
       /// <summary>
       /// ��ѡ���  C2 �� ���     
       /// </summary>
       public String SelectedCategory2
       {
           get { return this.m_SelectedCategory2; }
           set { this.SetValue("SelectedCategory2", ref m_SelectedCategory2, value);  }
       }

       private Int32? m_StockSysNo;
       /// <summary>
       /// ��ѡ���  �ֿ�
       /// </summary>
       public Int32? StockSysNo
       {
           get { return this.m_StockSysNo; }
           set { this.SetValue("StockSysNo", ref m_StockSysNo, value);  }
       }

       private String m_AvailableSalesDaysCondition;
       /// <summary>
       /// �����������Ƚ�����
       /// </summary>
       public String AvailableSalesDaysCondition
       {
           get { return this.m_AvailableSalesDaysCondition; }
           set { this.SetValue("AvailableSalesDaysCondition", ref m_AvailableSalesDaysCondition, value);  }
       }

       private String m_AVGSaledQtyCondition;
       /// <summary>
       /// �վ������Ƚ�����
       /// </summary>
       public String AVGSaledQtyCondition
       {
           get { return this.m_AVGSaledQtyCondition; }
           set { this.SetValue("AVGSaledQtyCondition", ref m_AVGSaledQtyCondition, value);  }
       }

       private String m_AvailableSaledDays;
       /// <summary>
       /// ����������
       /// </summary>
       [Validate(ValidateType.Required)]
       [Validate(ValidateType.Regex, @"^([1-9]?[0-9]|[1-9]?[0-9][0-8])$", ErrorMessage = "������0��998֮���������")]
       public String AvailableSaledDays
       {
           get { return this.m_AvailableSaledDays; }
           set { this.SetValue("AvailableSaledDays", ref m_AvailableSaledDays, value);  }
       }

       private String m_AVGSaledQty;
       /// <summary>
       /// �վ�����
       /// </summary>
       [Validate(ValidateType.Interger)]
       public String AVGSaledQty
       {
           get { return this.m_AVGSaledQty; }
           set { this.SetValue("AVGSaledQty", ref m_AVGSaledQty, value);  }
       }

   }
}
