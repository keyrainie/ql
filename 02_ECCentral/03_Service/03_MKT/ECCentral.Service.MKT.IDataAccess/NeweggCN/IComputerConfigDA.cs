using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.Enum;

namespace ECCentral.Service.MKT.IDataAccess
{
    public interface IComputerConfigDA
    {
        ComputerConfigMaster LoadMaster(int sysNo);

        List<ComputerConfigType> LoadAllConfigType();

        List<UserInfo> GetEditUsers(string companyCode, string channelID);

        void CreateComputerConfigMaster(ComputerConfigMaster msg);

        void UpdateComputerConfigMaster(ComputerConfigMaster msg);

        void CreateComputerConfigInfo(ComputerConfigItem entity);

        void DeleteComputerConfigInfo(int masterSysNo);

        void AuditComputerConfigMaster(int masterSysNo, ComputerConfigStatus targetStatus);

        int CountUniqueValidation(int excludeSysNo,string uniqueValidation,string companyCode,string channelID);

        /// <summary>
        /// 获取所有组件列表
        /// </summary>
        /// <returns></returns>
        List<ComputerParts> GetAllComputerParts();

        /// <summary>
        /// 获取配置单的明细
        /// </summary>
        /// <param name="masterSysNo">配置单系统编号</param>
        /// <returns></returns>
        List<ComputerConfigItem> GetComputerConfigItems(int masterSysNo);

        List<ComputerConfigItem> GetComputerConfigProductListByProductSysNo(int productSysNo);

         /// <summary>
        /// 获取组件的可选商品分类编号
        /// </summary>
        /// <param name="partSysNo">组件系统编号</param>
        /// <returns></returns>
        List<int> GetComputerPartsCategory(int partSysNo);

        /// <summary>
        /// 获取所有组件可选分类列表
        /// </summary>
        /// <returns></returns>
        List<ComputerPartsCategory> GetAllComputerPartsCategory();

        /// <summary>
        /// 根据DIY配置单系统编号获取对应套餐的系统编号
        /// </summary>
        /// <param name="configMasterSysNo">DIY配置单系统编号</param>
        /// <returns>对应套餐的系统编号</returns>
        int GetComboSysNo(int configMasterSysNo);

        //验证ComputerConfigName是否重复存在
        int CountComputerConfigName(int excludeSysNo, string configName, string companyCode, string channelID);
        
        void SetComputerConfigMasterStatus(ComputerConfigMaster computerConfig);

        void UpdateSaleRuleStatus(ComputerConfigMaster computerConfig);
    }
}
