using ECCentral.BizEntity.MKT.Promotion;

namespace ECCentral.Service.MKT.IDataAccess.Promotion
{
    public interface IProductPayTypeDA
    {
        /// <summary>
        /// 批量创建支付方式
        /// </summary>
        /// <returns></returns>
        int CreateProductPayType(ProductPayTypeInfo productPayTypeInfo);

        int AbortProductPayType(string sysNo, string editUser);
    }
}
