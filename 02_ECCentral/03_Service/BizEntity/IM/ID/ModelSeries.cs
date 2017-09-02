namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品ID-型号段
    /// </summary>
    public class ModelSeries : IIdentity, ICompany, ILanguage
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
        /// 三级类别
        /// </summary>
        public int CategorySysNo { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public int BrandSysNo { get; set; }

        /// <summary>
        /// 商品组型号系列
        /// </summary>
        public LanguageContent ProductGroupModel { get; set; }

        /// <summary>
        /// 公司代码
        /// </summary>
        public string CompanyCode { get; set; }

        /// <summary>
        /// 语言代码
        /// </summary>
        public string LanguageCode { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ModelSeries)
            {
                var modelSeries = obj as ModelSeries;
                if (modelSeries.CategorySysNo == CategorySysNo
                    && modelSeries.BrandSysNo == BrandSysNo
                    && modelSeries.ProductGroupModel == ProductGroupModel)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
