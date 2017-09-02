using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutoClose.Model
{
    public class ReturnPointList
    {
        /////////////////////CRL16063////////////////
        public string PM { get; set; }
        public string VendorSysNo { get; set; }
        ////////////////////////////////////////////
        
        public int SysNo { get; set; }

        public string PMName { get; set; }

        public string ReturnPointName { get; set; }

   
        public decimal ReturnPoint { get; set; }

        public string ReturnPointString
        {
            get
            {
                return ReturnPoint.ToString(FormatConst.CurrencyFormat);
            }
            set { ReturnPoint = Convert.ToDecimal(value); }
        }


    
        public decimal RemnantReturnPoint { get; set; }

        public string RemnantReturnPointString
        {
            get
            {
                return RemnantReturnPoint.ToString(FormatConst.CurrencyFormat);
            }
            set { RemnantReturnPoint = Convert.ToDecimal(value); }
        }

   
        public int Status { get; set; }

        public string StatusString
        {
            get
            {
                if (Status == 0)
                {
                    return "申请";
                }

                if (Status == 1)
                {
                    return "审核通过";
                }

                if (Status == -1)
                {
                    return "废止";
                }

                return "";
            }
        }

  
        public DateTime CreateTime { get; set; }

        public string CreateTimeString
        {
            get
            {
                return CreateTime.ToString(FormatConst.LongDateFormat);
            }
        }

 
        public DateTime ApproveTime { get; set; }

        public string ApproveTimeString
        {
            get
            {
                return ApproveTime.ToString(FormatConst.LongDateFormat);
            }
        }

  
        public string CreateUserName { get; set; }

  
        public string AuditUserName { get; set; }

  
        public string VendorName { get; set; }
    }

    public static class FormatConst
    {
        public const string CurrencyFormat = "￥#####0.00";
        public const string DecimalFormat = "#########0.00";
        public const string ExactCurrencyFormat = "￥#####0.000";
        public const string LongDateFormat = "yyyy-MM-dd HH:mm:ss";
        public const string ShortDateFormat = "yyyy-MM-dd";

    }
}
