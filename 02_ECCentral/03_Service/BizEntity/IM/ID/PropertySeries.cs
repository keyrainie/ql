namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品ID-分组属性段
    /// </summary>
    public class PropertySeries : IIdentity, ICompany, ILanguage
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 增量数字
        /// </summary>
        public int SeriesNo { get; set; }

        /// <summary>
        /// 对应代码
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 商品组
        /// </summary>
        public int ProductGroupSysNo { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        public string ProductModel { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言代码
        /// </summary>
        public string LanguageCode { get; set; }

    }
}
