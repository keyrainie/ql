using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Common;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.ExternalSYS
{
    /// <summary>
    /// 广告
    /// </summary>
    [Serializable]
    [DataContract]
    public class AdvertisingInfo : IIdentity, ICompany 
    {
        [DataMember]
        public int? SysNo { get; set; }
        [DataMember]
        public string Url { get; set; }
        [DataMember]
        public string ImageUrl { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string SharedText { get; set; }
        [DataMember]
        public string EventCode { get; set; }
        [DataMember]
        public string AdCode { get; set; }
        [DataMember]
        public int? ImageWidth { get; set; }
        [DataMember]
        public int? ImageHeight { get; set; }
        [DataMember]
        public AdvertisingType Type { get; set; }
        [DataMember]
        public ValidStatus Status { get; set; }
        [DataMember]
        public int? ProductLineSysNo { get; set; }
        [DataMember]
        public string ImageSize
        {
            get
            {
                return this.ImageWidth + "*" + this.ImageHeight;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Type == AdvertisingType.IMG)
                    {
                        string[] dimission = value.Split('*');
                        this.ImageWidth = int.Parse(dimission[0]);
                        this.ImageHeight = int.Parse(dimission[1]);
                    }
                }
            }
        }

        #region ICompany Members

        public string CompanyCode
        {
            get;
            set;
        }

        #endregion
    }
}
