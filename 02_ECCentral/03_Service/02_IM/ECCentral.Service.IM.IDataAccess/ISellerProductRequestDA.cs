//************************************************************************
// 用户名				泰隆优选
// 系统名				类别延保管理
// 子系统名		        类别延保管理NoBizQuery查询接口
// 作成者				Kevin
// 改版日				2012.6.5
// 改版内容				新建
//************************************************************************


using ECCentral.BizEntity.IM;
using System.Collections.Generic;

namespace ECCentral.Service.IM.IDataAccess
{
    public interface ISellerProductRequestDA
    {
        /// <summary>
        /// 根据SysNO获取商家商品需求信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        SellerProductRequestInfo GetSellerProductRequestInfoBySysNo(int sysNo);

        /// <summary>
        /// 根据ProductID获取商家商品信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        SellerProductRequestInfo GetSellerProductInfoByProductID(string productID);

        /// <summary>
        /// 设置商家商品需求信息状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SellerProductRequestInfo SetSellerProductRequestStatus(SellerProductRequestInfo entity);

        /// <summary>
        /// 在审核通过和退回时修改其它状态
        /// </summary>
        /// <param name="entity"></param>
        void SetSellerProductRequestOtherStatus(SellerProductRequestInfo entity);

        /// <summary>
        /// 修改商家商品需求信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        SellerProductRequestInfo UpdateSellerProductRequest(SellerProductRequestInfo entity);

        int CallExternalSP(SellerProductRequestInfo entity);

        /// <summary>
        /// 设置Commonsku每张图片写上文件名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        void UpdateProductRequestImageName(SellerProductRequestFileInfo entity);

        /// <summary>
        /// 更新属性信息
        /// </summary>
        /// <param name="entity"></param>
        void UpdateSellerProductRequestProperty(SellerProductRequestPropertyInfo entity, string editUserName);

        /// <summary>
        /// 根据SysNO获取商家商品属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        List<SellerProductRequestPropertyInfo> GetProductRequestPropertyList(int productRequestSysNo);

        /// <summary>
        /// 根据SysNO获取商家商品图片信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        List<SellerProductRequestFileInfo> GetSenderProductRequestImageList(int productRequestSysNo);

        /// <summary>
        /// 根据ProductID获取商家商品属性信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        List<SellerProductRequestPropertyInfo> GetSellerProductPropertyListByProductID(string productID);

    }
}
