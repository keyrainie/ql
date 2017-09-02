using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility.DataAccess;
using ECCentral.BizEntity.Enum;

namespace ECCentral.Service.MKT.SqlDataAccess.NeweggCN
{
    [VersionExport(typeof(IComputerConfigDA))]
    public class ComputerConfigDA : IComputerConfigDA
    {
        #region IComputerConfigDA Members

        public ComputerConfigMaster LoadMaster(int sysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetComputerConfigMasterBySysNo");
            cmd.SetParameterValue("@SysNo", sysNo);

            return cmd.ExecuteEntity<ComputerConfigMaster>();
        }

        public List<ComputerConfigType> LoadAllConfigType()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetComputerConfigMasterType");
            return cmd.ExecuteEntityList<ComputerConfigType>();
        }

        public List<UserInfo> GetEditUsers(string companyCode, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetComputerConfigMasterEditUser");
            cmd.SetParameterValue("@CompanyCode", companyCode);
            //TODO:添加ChannelID参数

            return cmd.ExecuteEntityList<UserInfo>();
        }

        public void CreateComputerConfigMaster(ComputerConfigMaster entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateComputerConfigMaster");
            command.SetParameterValue("@ComputerConfigName", entity.ComputerConfigName);
            command.SetParameterValue("@ComputerConfigType", entity.ComputerConfigTypeSysNo);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@CustomerSysno", entity.CustomerSysNo);
            command.SetParameterValue("@UniqueValidation", entity.UniqueValidation);
            command.SetParameterValueAsCurrentUserAcct("@InUser");
            command.SetParameterValue("@CompanyCode", entity.CompanyCode);

            command.ExecuteNonQuery();
            entity.SysNo = (int)command.GetParameterValue("@SysNo");
        }

        public void UpdateComputerConfigMaster(ComputerConfigMaster entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("UpdateComputerConfigMaster");
            command.SetParameterValue("@SysNo", entity.SysNo);
            command.SetParameterValue("@ComputerConfigName", entity.ComputerConfigName);
            command.SetParameterValue("@ComputerConfigType", entity.ComputerConfigTypeSysNo);
            command.SetParameterValue("@Note", entity.Note);
            command.SetParameterValue("@Priority", entity.Priority);
            command.SetParameterValue("@Status", entity.Status);
            command.SetParameterValue("@UniqueValidation", entity.UniqueValidation);
            command.SetParameterValueAsCurrentUserAcct("@EditUser");

            command.ExecuteNonQuery();
        }

        public void CreateComputerConfigInfo(ComputerConfigItem entity)
        {
            DataCommand command = DataCommandManager.GetDataCommand("CreateComputerConfigInfo");
            command.SetParameterValue("@ProductSysNo", entity.ProductSysNo);
            command.SetParameterValue("@ComputerConfigSysNo", entity.ComputerConfigSysNo);
            command.SetParameterValue("@ComputerPartSysNo", entity.ComputerPartSysNo);
            command.SetParameterValue("@Discount", entity.Discount);
            command.SetParameterValue("@ProductQty", entity.ProductQty);

            command.ExecuteNonQuery();
            entity.SysNo = (int)command.GetParameterValue("@SysNo");
        }

        public void DeleteComputerConfigInfo(int masterSysNo)
        {
            DataCommand command = DataCommandManager.GetDataCommand("DeleteComputerConfigInfo");
            command.SetParameterValue("@ComputerConfigMasterSysNo", masterSysNo);
            command.ExecuteNonQuery();
        }

        public void AuditComputerConfigMaster(int masterSysNo,ComputerConfigStatus targetStatus)
        {
            DataCommand command = DataCommandManager.GetDataCommand("AuditComputerConfigMaster");
            command.SetParameterValue("@ComputerConfigMasterSysNo", masterSysNo);
            command.SetParameterValue("@Status", targetStatus);
            command.SetParameterValueAsCurrentUserAcct("@AuditUser");
            command.ExecuteNonQuery();
        }

        public int CountUniqueValidation(int excludeSysNo, string uniqueValidation, string companyCode, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CountUniqueValidation");
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@UniqueValidation", uniqueValidation);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            //TODO:添加ChannelID参数

            return cmd.ExecuteScalar<int>();
        }


        /// <summary>
        /// 获取所有组件列表
        /// </summary>
        /// <returns></returns>
        public List<ComputerParts> GetAllComputerParts()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetComputerPartsList");

            return cmd.ExecuteEntityList<ComputerParts>();
        }

        /// <summary>
        /// 获取配置单的明细
        /// </summary>
        /// <param name="masterSysNo">配置单系统编号</param>
        /// <returns></returns>
        public List<ComputerConfigItem> GetComputerConfigItems(int masterSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetComputerConfigProductList");
            cmd.SetParameterValue("@ComputerConfigMasterSysNo", masterSysNo);

            return cmd.ExecuteEntityList<ComputerConfigItem>();
        }

        public List<ComputerConfigItem> GetComputerConfigProductListByProductSysNo(int productSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("GetComputerConfigProductListByProductSysNo");
            cmd.SetParameterValue("@ProductSysNo", productSysNo);

            return cmd.ExecuteEntityList<ComputerConfigItem>();
        }

        /// <summary>
        /// 获取组件的可选商品分类编号
        /// </summary>
        /// <param name="partSysNo">组件系统编号</param>
        /// <returns></returns>
        public List<int> GetComputerPartsCategory(int partSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryCategoryByComputerPart");
            cmd.SetParameterValue("@ComputerPartSysNo", partSysNo);

            return cmd.ExecuteFirstColumn<int>();
        }

        /// <summary>
        /// 获取所有组件可选分类列表
        /// </summary>
        /// <returns></returns>
        public List<ComputerPartsCategory> GetAllComputerPartsCategory()
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QueryAllPartsCategory");

            return cmd.ExecuteEntityList<ComputerPartsCategory>();
        }

         /// <summary>
        /// 根据DIY配置单系统编号获取对应套餐的系统编号
        /// </summary>
        /// <param name="configMasterSysNo">DIY配置单系统编号</param>
        /// <returns>对应套餐的系统编号</returns>
        public int GetComboSysNo(int configMasterSysNo)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("QuerySaleRuleSysNoByCCMSysNo");
            cmd.SetParameterValue("@SysNo", configMasterSysNo);

            return cmd.ExecuteScalar<int>();
        }

        //验证ComputerConfigName是否重复存在
        public int CountComputerConfigName(int excludeSysNo, string configName, string companyCode, string channelID)
        {
            DataCommand cmd = DataCommandManager.GetDataCommand("CountComputerConfigName");
            cmd.SetParameterValue("@ExcludeSysNo", excludeSysNo);
            cmd.SetParameterValue("@ComputerConfigName", configName);
            cmd.SetParameterValue("@CompanyCode", companyCode);

            //TODO:添加ChannelID参数

            return cmd.ExecuteScalar<int>();
        }
        #endregion


        public void SetComputerConfigMasterStatus(ComputerConfigMaster computerConfig)
        {
            throw new NotImplementedException();
        }

        public void UpdateSaleRuleStatus(ComputerConfigMaster computerConfig)
        {
            throw new NotImplementedException();
        }
    }
}
