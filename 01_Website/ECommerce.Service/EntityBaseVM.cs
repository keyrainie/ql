using System;


namespace ECommerce.Facade
{
    public class EntityBaseVM
    {
        /// <summary>
        /// 创建者系统编号
        /// </summary>
        public int? InUserSysNo { get; set; }

        /// <summary>
        /// 创建者的显示名
        /// </summary>
        public string InUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime InDate { get; set; }

        /// <summary>
        /// 最后更新者系统编号
        /// </summary>
        public int? EditUserSysNo { get; set; }

        /// <summary>
        /// 最后更新者显示名
        /// </summary>
        public string EditUserName { get; set; }

        /// <summary>
        /// 最后更新时间
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 商家系统编号
        /// </summary>
        public string CompanyCode { get; set; }

        public string LanguageCode { get; set; }
    }
}
