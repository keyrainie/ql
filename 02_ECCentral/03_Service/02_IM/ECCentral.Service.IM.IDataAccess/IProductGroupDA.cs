using System.Collections.Generic;
using ECCentral.BizEntity.IM;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface IProductGroupDA
    {
        /// <summary>
        /// 根据商品SysNo获取该商品所在商品组信息
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        ProductGroup GetProductGroup(int productSysNo);

        /// <summary>
        /// 根据商品SysNo获取该商品所在商品组内所有商品SysNo
        /// </summary>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        List<int> GetProductSysNoListByGroupProductSysNo(int productSysNo);

        /// <summary>
        /// 根据SysNo获取商品组信息
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        /// <returns></returns>
        ProductGroup GetProductGroupInfoBySysNo(int productGroupSysNo);

        /// <summary>
        /// 清空该商品组下商品的商品组属性（刷新）
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        void RemoveProductCommonInfoGroupSysNo(int productGroupSysNo);

        /// <summary>
        /// 组名称排重判断
        /// </summary>
        /// <param name="sysNo"> </param>
        /// <param name="groupName"> </param>
        /// <returns></returns>
        ProductGroup GetProductGroupInfoByGroupName(int sysNo, string groupName);

        /// <summary>
        /// 创建商品组信息
        /// </summary>
        /// <param name="productGroup"></param>
        void CreateProductGroupInfo(ProductGroup productGroup);

        /// <summary>
        /// 编辑商品组信息
        /// </summary>
        /// <param name="productGroup"></param>
        void UpdateProductGroupInfo(ProductGroup productGroup);

        /// <summary>
        /// 创建商品组分组属性
        /// </summary>
        /// <param name="productGroup"></param>
        void CreateGroupPropertySetting(ProductGroup productGroup);

        /// <summary>
        /// 删除商品组所有分组属性
        /// </summary>
        /// <param name="productGroupSysNo"></param>
        void DeleteGroupPropertySetting(int productGroupSysNo);

        /// <summary>
        /// 获取商品所在商品组的其它商品ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        List<string> GetProductGroupIDSFromProductID(string productID);
    }
}
