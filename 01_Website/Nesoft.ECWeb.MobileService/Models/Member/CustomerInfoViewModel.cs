using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Nesoft.ECWeb.Entity.Member;
using Nesoft.ECWeb.Enums;
using Nesoft.Utility;
using System.Web.Script.Serialization;

namespace Nesoft.ECWeb.MobileService.Models.Member
{
    public class CustomerInfoViewModel
    {
        public int SysNo { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public int? Gender { get; set; }
        [ScriptIgnore]
        public CustomerRankType CustomerRank { get; set; }
        public string CustomerRankStr
        {
            get
            {
                return EnumHelper.GetDescription(this.CustomerRank);
            }
        }
        public int Status { get; set; }
        public string RegisterTimeString { get; set; }
        public string CellPhone { get; set; }
        public string Phone { get; set; }
        public decimal ValidScore { get; set; }
        public int IsPhoneValided { get; set; }
        public int IsEmailConfirmed { get; set; }
        [ScriptIgnore]
        public DateTime? Birthday { get; set; }
        private string m_BirthdayStr;
        public string BirthdayStr
        {
            get
            {
                if (this.Birthday.HasValue)
                {
                    return this.Birthday.Value.ToString("yyyy-MM-dd");
                }
                return m_BirthdayStr;
            }
            set
            {
                m_BirthdayStr = value;
            }
        }
        public string DwellAddress { get; set; }
        public int DwellAreaSysNo { get; set; }
        public string DwellZip { get; set; }
        public decimal ValidPrepayAmt { get; set; }
        public string AvtarImage { get; set; }
        /// <summary>
        /// 用户头像状态(A=有效,D=无效)
        /// </summary>
        public string AvtarImageDBStatus { get; set; }
        public string AvtarImageUrl
        {
            get
            {
                return (!string.IsNullOrEmpty(AvtarImage) && (AvtarImageDBStatus == "a" || AvtarImageDBStatus == "A")) ? ConfigurationManager.AppSettings["FileBaseUrl"].ToString() + "/" + AvtarImage:"";
            }
        }
    }
}