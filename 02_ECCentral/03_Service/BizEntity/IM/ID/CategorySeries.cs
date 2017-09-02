namespace ECCentral.BizEntity.IM
{
    /// <summary>
    /// 商品ID-类别段
    /// </summary>
    public class CategorySeries : IIdentity
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
        /// 三级类别
        /// </summary>
        public int? CategorySysNo { get; set; }
    }
}
