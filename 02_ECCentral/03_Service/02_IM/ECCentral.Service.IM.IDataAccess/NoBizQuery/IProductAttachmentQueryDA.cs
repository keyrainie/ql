using System.Data;

using ECCentral.BizEntity.IM;
using ECCentral.QueryFilter.IM;

namespace ECCentral.Service.IM.IDataAccess.NoBizQuery
{
    public interface IProductAttachmentQueryDA
    {
        /// <summary>
        /// 查询商品附件
        /// </summary>
        /// <returns></returns>
        DataTable QueryProductAttachment(ProductAttachmentQueryFilter queryCriteria, out int totalCount);
    }
}
