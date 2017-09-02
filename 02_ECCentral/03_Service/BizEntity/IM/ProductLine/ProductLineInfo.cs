using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.IM
{
    [Serializable]
    [DataContract]
    public class ProductLineInfo : IIdentity, ICompany
    {
        [DataMember]
        public int? SysNo { get; set; }

        /// <summary>
        /// 一级类编号
        /// </summary>
        [DataMember]
        public int? C1SysNo { get; set; }

        /// <summary>
        /// 一级类名称
        /// </summary>
        [DataMember]
        public string C1Name { get; set; }

        /// <summary>
        /// 二级类编号
        /// </summary>
        [DataMember]
        public int? C2SysNo { get; set; }


        /// <summary>
        /// 二级类名称
        /// </summary>
        [DataMember]
        public string C2Name { get; set; }

        /// <summary>
        /// 三级类名称
        /// </summary>
        [DataMember]
        public string C3Name { get; set; }

        /// <summary>
        /// 三级类编号
        /// </summary>
        [DataMember]
        public int? C3SysNo { get; set; }

        /// <summary>
        /// 品牌编号
        /// </summary>
        [DataMember]
        public int? BrandSysNo { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        [DataMember]
        public string BrandName { get; set; }

        /// <summary>
        /// PM用户编号
        /// </summary>
        [DataMember]
        public int PMUserSysNo { get; set; }

        /// <summary>
        /// PM用户名称
        /// </summary>
        [DataMember]
        public string PMUserName { get; set; }

        /// <summary>
        /// 销售专员编号
        /// </summary>
        [DataMember]
        public int MerchandiserSysNo { get; set; }

        /// <summary>
        /// 销售专员名称
        /// </summary>
        [DataMember]
        public string MerchandiserName { get; set; }


        
        private string b_backupPMSysNoList = string.Empty;

        [DataMember]
        public string BackupPMSysNoList 
        {
            get 
            {
                return b_backupPMSysNoList;
            }
            set 
            {
                b_backupPMSysNoList = value;
            }
        }

        [DataMember]
        public string BackupPMNameList { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [DataMember]
        public string InUser { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        public DateTime InDate { get; set; }

        /// <summary>
        /// 编辑人
        /// </summary>
        [DataMember]
        public string EditUser { get; set; }

        /// <summary>
        /// 编辑日期
        /// </summary>
        [DataMember]
        public DateTime EditDate { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string CompanyCode { get; set; }

        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public string StoreCompanyCode { get; set; }

        /// <summary>
        /// 编码格式
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }
    }
}
