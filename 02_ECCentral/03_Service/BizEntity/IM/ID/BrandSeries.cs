namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品ID-品牌段
    /// </summary>
    public class BrandSeries : IIdentity
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 对应代码
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public int? BrandSysNo { get; set; }
    }
}
