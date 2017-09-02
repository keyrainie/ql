using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.Inventory;

namespace ECCentral.Service.Inventory.IDataAccess
{
    public interface IConvertRequestDA
    {

        #region 转换单维护

        /// <summary>
        /// 创建转换单主信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ConvertRequestInfo CreateConvertRequest(ConvertRequestInfo entity);

        /// <summary>
        /// 根据商品编号得到其所属产品线
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        ConvertRequestInfo GetProductLineInfo(int sysNo);

        /// <summary>
        /// 保存（更新）转换单
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ConvertRequestInfo UpdateConvertRequest(ConvertRequestInfo entity);

        /// <summary>
        /// 更新转换单状态
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        ConvertRequestInfo UpdateConvertRequestStatus(ConvertRequestInfo entity);

        /// <summary>
        /// <summary>
        /// 根据SysNo获取转换单信息
        /// </summary>
        /// <param name="requestSysNo"></param>
        /// <returns></returns>
        ConvertRequestInfo GetConvertRequestInfoBySysNo(int requestSysNo);

        /// <summary>
        /// 获取转换单新增的SysNo
        /// </summary>
        /// <returns></returns>
        int GetConvertRequestSequence();

        #endregion 转换单维护

        #region 转换商品维护


        /// <summary>
        /// 加载转换单商品
        /// </summary>
        /// <param name="convertRequestSysNo"></param>
        /// <returns></returns>
        List<ConvertRequestItemInfo> GetConvertItemListByRequestSysNo(int requestSysNo);   

        /// <summary>
        /// 创建转换单Item
        /// </summary>
        /// <param name="convertRequestItem"></param>
        /// /// <param name="requestID"></param>
        /// <param name="requestID"></param>
        /// <returns></returns>
        ConvertRequestItemInfo CreateConvertItem(ConvertRequestItemInfo convertRequestItem, int requestID);

        /// <summary>
        /// 删除转换单Item
        /// </summary>
        /// <param name="convertRequestItem"></param>
        /// <returns></returns>
        void DeleteConvertItemByRequestSysNo(int requestSysNo);

        #endregion 转换商品维护        
    }
}
