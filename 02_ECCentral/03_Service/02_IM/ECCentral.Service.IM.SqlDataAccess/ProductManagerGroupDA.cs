//************************************************************************
// 用户名				泰隆优选
// 系统名				PM组管理
// 子系统名		        PM组管理业务接口实现
// 作成者				Tom
// 改版日				2012.4.24
// 改版内容				新建
//************************************************************************

using System;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.IM.IDataAccess.NoBizQuery;
using ECCentral.Service.Utility;
using ECCentral.Service.Utility.DataAccess;

namespace ECCentral.Service.IM.SqlDataAccess
{
    [VersionExport(typeof(IProductManagerGroupDA))]
    public class ProductManagerGroupDA : IProductManagerGroupDA
    {
        /// <summary>
        /// 根据SysNO获取PM组信息
        /// </summary>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        public ProductManagerGroupInfo GetProductManagerGroupInfoBySysNo(int productManagerGroupInfoSysNo)
        {

            DataCommand cmd = DataCommandManager.GetDataCommand("GetProductManagerGroupInfoBySysNo");
            cmd.SetParameterValue("@SysNo", productManagerGroupInfoSysNo);
            var sourceEntity = cmd.ExecuteEntity<ProductManagerGroupInfo>();
            if (sourceEntity == null)
            {
                sourceEntity = new ProductManagerGroupInfo { SysNo = productManagerGroupInfoSysNo };
            }
            else
            {
                var productmanagergroupQueryDA = ObjectFactory<IProductManagerGroupQueryDA>.Instance;
                var pms = productmanagergroupQueryDA.QueryAllProductManagerInfoByPMGroupSysNo(productManagerGroupInfoSysNo);
                sourceEntity.ProductManagerInfoList = pms;
            }
            return sourceEntity;

        }

        /// <summary>
        /// 根据用户编号获取PM组信息
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <returns></returns>
        public ProductManagerGroupInfo GetPMListByUserSysNo(int userSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetPMListByUserSysNo");
            cmd.SetParameterValue("@UserSysNo", userSysNo);
            var sourceEntity = cmd.ExecuteEntity<ProductManagerGroupInfo>();
            if (sourceEntity == null)
            {
                sourceEntity = new ProductManagerGroupInfo();
            }
            else
            {
                var productmanagergroupQueryDA = ObjectFactory<IProductManagerGroupQueryDA>.Instance;
                if (sourceEntity.SysNo != null && sourceEntity.SysNo > 0)
                {
                    var pms = productmanagergroupQueryDA.QueryAllProductManagerInfoByPMGroupSysNo(sourceEntity.SysNo.Value);
                    sourceEntity.ProductManagerInfoList = pms;
                }
            }
            return sourceEntity;
        }

        /// <summary>
        /// 创建PM组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductManagerGroupInfo CreateProductManagerGroupInfo(ProductManagerGroupInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CreateProductManagerGroupInfo");
            cmd.SetParameterValue("@PMGroupName", entity.PMGroupName.Content.Trim());
            cmd.SetParameterValue("@TLSysNo", entity.UserInfo.SysNo);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
            entity.SysNo = (int)cmd.GetParameterValue("@SysNo");
            return entity;
        }

        /// <summary>
        /// 修改PM组
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public ProductManagerGroupInfo UpdateProductManagerGroupInfo(ProductManagerGroupInfo entity)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("UpdateProductManagerGroupInfo");
            cmd.SetParameterValue("@SysNo", entity.SysNo);
            cmd.SetParameterValue("@TLSysNo", entity.UserInfo.SysNo);
            cmd.SetParameterValue("@Status", entity.Status);
            cmd.SetParameterValue("@CompanyCode", "8601");
            cmd.ExecuteNonQuery();
            return entity;
        }

        /// <summary>
        /// 除本身之外是否存在某个组名
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="productManagerGroupInfoSysNo"></param>
        /// <returns></returns>
        public bool IsExistPMGroupName(string groupName, int productManagerGroupInfoSysNo)
        {

            DataCommand cmd = DataCommandManager.GetDataCommand("IsExistPMGroupName");
            cmd.SetParameterValue("@SysNo", productManagerGroupInfoSysNo);
            cmd.SetParameterValue("@PMGroupName", groupName);
            var value = cmd.ExecuteScalar();
            if (value is DBNull || value == null)
            {
                return false;
            }
            var count = Convert.ToInt32(value);
            return count != 0;
        }

        /// <summary>
        /// 是否存在某个组名
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public bool IsExistPMGroupName(string groupName)
        {
            var result = IsExistPMGroupName(groupName, 0);
            return result;
        }

        /// <summary>
        /// 更新PM隶属于哪个PM组
        /// </summary>
        /// <param name="PMUserSysNo"></param>
        /// <param name="pmGroupSysNo"></param>
        /// <param name="companyCode"></param>
        public void UpdatePMMasterGroupSysNo(int PMUserSysNo, int pmGroupSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("UpdatePMMasterGroupSysNo");

            dc.SetParameterValue("@PMUserSysNo", PMUserSysNo);
            dc.SetParameterValue("@PMGroupSysNo", pmGroupSysNo);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.ExecuteNonQuery();
        }

        /// <summary>
        /// 清空PM对应的PM组
        /// </summary>
        /// <param name="pmGroupSysNo"></param>
        public void ClearPMMasterGroupSysNo(int pmGroupSysNo)
        {
            DataCommand dc = DataCommandManager.GetDataCommand("ClearPMMasterGroupSysNo");
            dc.SetParameterValue("@PMGroupSysNo", pmGroupSysNo);
            dc.SetParameterValue("@CompanyCode", "8601");

            dc.ExecuteNonQuery();

        }
    }
}
