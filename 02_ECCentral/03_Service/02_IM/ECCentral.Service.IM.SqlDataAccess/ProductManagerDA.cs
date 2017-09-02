//************************************************************************
// 用户名				泰隆优选
// 系统名				PM管理
// 子系统名		        PM管理业务接口实现
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using System;
using System.Collections.Generic;
using System.Data;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductManagerDA))]
    public class ProductManagerDA : IProductManagerDA
    {
        /// <summary>
        /// 根据SysNO获取PM信息
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public ProductManagerInfo GetProductManagerInfoBySysNo(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductManagerInfoBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);
            var sourceEntity = cmd.ExecuteEntity<ProductManagerInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 根据SysNO获取PM信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public ProductManagerInfo GetProductManagerInfoByUserSysNo(int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductManagerInfoByUserSysNo");
            cmd.SetParameterValue("@PMUserSysNo", userSysNo);
            var sourceEntity = cmd.ExecuteEntity<ProductManagerInfo>();
            return sourceEntity;
        }

        /// <summary>
        /// 创建PM
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductManagerInfo CreateProductManagerInfo(ProductManagerInfo entity)
        {
            var dc = DataCommandManager.GetDataCommand("CreateProductManagerInfo");
            dc.SetParameterValue("@ITMaxWeightforPerDay", entity.ITMaxWeightforPerDay);
            dc.SetParameterValue("@ITMaxWeightforPerOrder", entity.ITMaxWeightforPerOrder);
            dc.SetParameterValue("@MaxAmtPerDay", entity.MaxAmtPerDay);
            dc.SetParameterValue("@MaxAmtPerOrder", entity.MaxAmtPerOrder);
            dc.SetParameterValue("@PMDMaxAmtPerDay", entity.PMDMaxAmtPerDay);
            dc.SetParameterValue("@PMDMaxAmtPerOrder", entity.PMDMaxAmtPerOrder);
            dc.SetParameterValue("@SaleRatePerMonth", entity.SaleRatePerMonth);
            dc.SetParameterValue("@SaleTargetPerMonth", entity.SaleTargetPerMonth);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@TLSaleRatePerMonth", 0);
            dc.SetParameterValue("@WarehouseNumber", entity.WarehouseNumber);
            dc.SetParameterValue("@PMUserSysNo", entity.UserInfo.SysNo);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.ExecuteNonQuery();
            if (entity.SysNo == null || entity.SysNo == 0)
            {
                entity.SysNo = (int)dc.GetParameterValue("@SysNo");
                var tempEntity = GetProductManagerInfoBySysNo(entity.SysNo.Value);
                if (tempEntity != null && tempEntity.UserInfo != null)
                    entity.UserInfo.UserName = tempEntity.UserInfo.UserName;
            }
            return entity;
        }

        /// <summary>
        /// 修改PM
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductManagerInfo UpdateProductManagerInfo(ProductManagerInfo entity)
        {
            var dc = DataCommandManager.GetDataCommand("UpdateProductManagerInfo");
            dc.SetParameterValue("@SysNo", entity.SysNo);
            dc.SetParameterValue("@ITMaxWeightforPerDay", entity.ITMaxWeightforPerDay);
            dc.SetParameterValue("@ITMaxWeightforPerOrder", entity.ITMaxWeightforPerOrder);
            dc.SetParameterValue("@MaxAmtPerDay", entity.MaxAmtPerDay);
            dc.SetParameterValue("@MaxAmtPerOrder", entity.MaxAmtPerOrder);
            dc.SetParameterValue("@PMDMaxAmtPerDay", entity.PMDMaxAmtPerDay);
            dc.SetParameterValue("@PMDMaxAmtPerOrder", entity.PMDMaxAmtPerOrder);
            dc.SetParameterValue("@SaleRatePerMonth", entity.SaleRatePerMonth);
            dc.SetParameterValue("@SaleTargetPerMonth", entity.SaleTargetPerMonth);
            dc.SetParameterValue("@Status", entity.Status);
            dc.SetParameterValue("@TLSaleRatePerMonth", 0);
            dc.SetParameterValue("@WarehouseNumber", entity.WarehouseNumber);
            dc.SetParameterValue("@PMUserSysNo", entity.UserInfo.SysNo);
            dc.SetParameterValue("@CompanyCode", "8601");
            dc.ExecuteNonQuery();
            if (entity.SysNo > 0)
            {
                var tempEntity = GetProductManagerInfoBySysNo(entity.SysNo.Value);
                if (tempEntity != null && tempEntity.UserInfo != null)
                    entity.UserInfo.UserName = tempEntity.UserInfo.UserName;
            }
            return entity;
        }

        /// <summary>
        /// 除本身之外PM表中是否存在某个PMUserSysNo
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <param name="productManagerInfoSysNo"></param>
        /// <returns></returns>
        public bool IsExistPMUserSysNo(int userSysNo, int productManagerInfoSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistPMUserSysNo");
            cmd.SetParameterValue("@SysNo", productManagerInfoSysNo);
            cmd.SetParameterValue("@UserSysNo", userSysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        ///  PM表中是否存在某个PMUserSysNo
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public bool IsExistPMUserSysNo(int userSysNo)
        {
            var result = IsExistPMUserSysNo(userSysNo, 0);
            return result;
        }

        /// <summary>
        /// 是否存在PMUserSysNo
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public bool IsExistUserSysNo(int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistUserSysNo");
            cmd.SetParameterValue("@UserSysNo", userSysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public bool IsExistUserID(string userID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistUserID");
            cmd.SetParameterValue("@UserID", userID);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 是否存在PM被ProductDomain中使用
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public bool IsPMInUsingByProductDomain(int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsPMInUsingByProductDomain");
            cmd.SetParameterValue("@PMSysNo", userSysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 是否存在PM被Product中使用
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public bool IsPMInUsingByProduct(int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("IsPMInUsingByProduct");
            cmd.SetParameterValue("@PMSysNo", userSysNo);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        public List<ProductManagerInfo> GetAll()
        {
            throw new System.NotImplementedException();
        }


        public IEnumerable<ProductManagerInfo> GetProductManager(CategoryInfo category, BrandInfo brand)
        {
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 根据UserID获取UserInfo对象
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public UserInfo GetUserInfoByUserID(string userID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetUserInfoByUserID");
            cmd.SetParameterValue("@UserID", userID);
            UserInfo entity = cmd.ExecuteEntity<UserInfo>();

            return entity;
        }

        /// <summary>
        /// 根据条件和权限，查询PM列表信息
        /// </summary>
        /// <param name="queryType"></param>
        /// <param name="loginName"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        public List<ProductManagerInfo> GetPMListByType(PMQueryType queryType, string loginName, string companyCode)
        {
            var command = DataCommandManager.CreateCustomDataCommandFromConfig(string.Format("QueryPMList_{0}", queryType.ToString()));
            command.AddInputParameter("@LoginName", DbType.String, loginName);
            List<ProductManagerInfo> returnList = command.ExecuteEntityList<ProductManagerInfo>();
            return returnList;
        }

        public List<ProductManagerInfo> GetPMLeaderList(string companyCode)
        {
            var command = DataCommandManager.CreateCustomDataCommandFromConfig("GetPMLeaderList");
            
            command.SetParameterValue("@CompanyCode", companyCode);

            return command.ExecuteEntityList<ProductManagerInfo>();            
        }
    }
}
