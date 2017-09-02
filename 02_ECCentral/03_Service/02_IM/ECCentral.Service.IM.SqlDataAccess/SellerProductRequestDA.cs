//************************************************************************
// 用户名				泰隆优选
// 系统名				类别延保管理
// 子系统名		        类别延保管理
// 作成者				Kevin
// 改版日				2012.6.5
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;
using System.Collections.Generic;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(ISellerProductRequestDA))]
    internal class SellerProductRequestDA : ISellerProductRequestDA
    {

        /// <summary>
        /// 根据SysNo获取商家商品信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public SellerProductRequestInfo GetSellerProductRequestInfoBySysNo(int sysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSellerProductRequestInfoBySysNo");
            command.SetParameterValue("@SysNo", sysNo);

            return command.ExecuteEntity<SellerProductRequestInfo>();
        }

                /// <summary>
        /// 根据ProductIDo获取商家商品信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public SellerProductRequestInfo GetSellerProductInfoByProductID(string productID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSellerProductInfoByProductID");
            command.SetParameterValue("@ProductID", productID);

            return command.ExecuteEntity<SellerProductRequestInfo>();
        }

        /// <summary>
        /// 根据ProductID获取商家商品属性信息
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        public List<SellerProductRequestPropertyInfo> GetSellerProductPropertyListByProductID(string productID)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSellerProductPropertyListByProductID");
            command.SetParameterValue("@ProductID", productID);

            return command.ExecuteEntityList<SellerProductRequestPropertyInfo>();
        }

        /// <summary>
        /// 根据SysNO获取商家商品属性信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public List<SellerProductRequestPropertyInfo> GetProductRequestPropertyList(int productRequestSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetProductRequestPropertyList");
            command.SetParameterValue("@ProductRequestSysNo", productRequestSysNo);

            return command.ExecuteEntityList<SellerProductRequestPropertyInfo>();
        }

        /// <summary>
        /// 根据SysNO获取商家商品图片信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public List<SellerProductRequestFileInfo> GetSenderProductRequestImageList(int productRequestSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("GetSenderProductRequestImageList");
            command.SetParameterValue("@ProductRequestSysNo", productRequestSysNo);

            return command.ExecuteEntityList<SellerProductRequestFileInfo>();
        }

        /// <summary>
        /// 更新商品需求信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo UpdateSellerProductRequest(SellerProductRequestInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSellerProductRequest");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@CommonSKUNumber", entity.CommonSKUNumber);
            cmd.SetParameterValue("@ProductID", entity.ProductID);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@EditUser", entity.EditUser.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);

            cmd.ExecuteNonQuery();
            return entity;
        }


        public void UpdateSellerProductRequestProperty(SellerProductRequestPropertyInfo entity, string editUserName)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateSellerProductRequestProperty");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@ValueSysno", entity.ValueSysno);
            cmd.SetParameterValue("@ManualInput", entity.ManualInput);
            cmd.SetParameterValue("@EditUser", editUserName);

            cmd.ExecuteNonQuery();
        }


        /// <summary>
        /// 设置Commonsku每张图片写上文件名
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void UpdateProductRequestImageName(SellerProductRequestFileInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductRequestImageName");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@ImageName", entity.ImageName);
            cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// 修改三级延保信息
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public SellerProductRequestInfo SetSellerProductRequestStatus(SellerProductRequestInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetSellerProductRequestStatus");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@Auditor", entity.Auditor.UserDisplayName);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);

            cmd.ExecuteNonQuery();
            return entity;
        }

        public int CallExternalSP(SellerProductRequestInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CallExternalSP");

            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@VP_ProductSysno", entity.RequestSysno);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@Memo", entity.Memo);

            cmd.ExecuteNonQuery();
            return (int)cmd.GetParameterValue("@IsSuccess");
        }



        /// <summary>
        /// 在审核通过和退回时修改其它状态
        /// </summary>
        /// <param name="entity"></param>
        public void SetSellerProductRequestOtherStatus(SellerProductRequestInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("SetSellerProductRequestOtherStatus");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@CompanyCode", entity.CompanyCode);
            cmd.ExecuteNonQuery();
        }
    }
}