using System;
using System.Runtime.Serialization;

namespace ECCentral.BizEntity.Common
{
    [Serializable]
    [DataContract]
    public class TariffInfo : IIdentity, ILanguage
    {
        [DataMember]
        public int? SysNo { get; set; }
        /// <summary>
        /// �����
        /// </summary>
        [DataMember]
        public int? ParentSysNo { get; set; }
        /// <summary>
        /// ˰��
        /// </summary>
        [DataMember]
        public string Tariffcode { get; set; }
        /// <summary>
        /// ˰��ɾ�������0���Ա��ѯ
        /// </summary>
        [DataMember]
        public string Tcode { get; set; }
        /// <summary>
        /// Ʒ�������
        /// </summary>
        [DataMember]
        public string ItemCategoryName { get; set; }
        /// <summary>
        /// 0:���ã�1��������
        /// </summary>
        [DataMember]
        public TariffStatus? Status { get; set; }
        /// <summary>
        /// ��λ����ǧ�ˣ�����
        /// </summary>
        [DataMember]
        public string Unit { get; set; }
        /// <summary>
        /// ��˰�۸�RMB:Ԫ��
        /// </summary>
        [DataMember]
        public decimal? TariffPrice { get; set; }
        /// <summary>
        /// ˰��(����ٷֱ����֣����磬�����10%��ֱ�Ӵ���10)
        /// </summary>
        [DataMember]
        public int? TariffRate { get; set; }
        /// <summary>
        /// ������Id
        /// </summary>
        [DataMember]
        public int? InUserSysNo { get; set; }
        /// <summary>
        /// ������
        /// </summary>
        [DataMember]
        public string InUserName { get; set; }
        /// <summary>
        /// ����ʱ��
        /// </summary>
        [DataMember]
        public DateTime? InDate { get; set; }
        /// <summary>
        /// �༭��Id
        /// </summary>
        [DataMember]
        public int? EditUserSysNo { get; set; }
        /// <summary>
        /// �༭��
        /// </summary>
        [DataMember]
        public string EditUserName { get; set; }
        /// <summary>
        /// �༭ʱ��
        /// </summary>
        [DataMember]
        public DateTime? EditDate { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [DataMember]
        public string LanguageCode { get; set; }
    }
}
