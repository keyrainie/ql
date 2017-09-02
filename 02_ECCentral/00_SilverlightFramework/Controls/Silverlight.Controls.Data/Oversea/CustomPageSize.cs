namespace Newegg.Oversea.Silverlight.Controls.Primitives
{
    /// <summary>
    /// 自定义每页显示数的实体
    /// </summary>
    public class CustomPageSize
    {
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 每页显示数
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否为默认
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
