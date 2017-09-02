using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.Common;

namespace ECCentral.QueryFilter.IM
{
    public class ManufacturerQueryFilter
    {
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// 生产商ID
        /// </summary>
        public string ManufacturerID { get; set; }

        /// <summary>
        /// 生产商本地化名称
        /// </summary>
        public string ManufacturerNameLocal { get; set; }


        /// <summary>
        /// 生产商国际化名称
        /// </summary>
        public string ManufacturerNameGlobal { get; set; }


        /// <summary>
        /// 生产商状态
        /// </summary>
        public ManufacturerStatus? Status { get; set; }


    }
}
