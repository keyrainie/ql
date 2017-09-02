using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.Service.MKT.BizProcessor;

namespace ECCentral.Service.MKT.AppService
{
    [VersionExport(typeof(ComputerConfigAppService))]
    public class ComputerConfigAppService
    {
        private ComputerConfigProcessor _configProcessor = ObjectFactory<ComputerConfigProcessor>.Instance;

        /// <summary>
        /// 获取所有组件列表
        /// </summary>
        /// <returns></returns>
        public List<ComputerParts> GetAllComputerParts()
        {
            return _configProcessor.GetAllComputerParts();
        }
        
        public ComputerConfigMaster LoadComputerConfig(int sysNo)
        {
            return _configProcessor.LoadComputerConfig(sysNo);
        }

        /// <summary>
        /// 获取配置单类型列表
        /// </summary>
        /// <returns></returns>
        public List<ComputerConfigType> LoadAllConfigType()
        {
            return _configProcessor.LoadAllConfigType();
        }

        /// <summary>
        /// 获取配置单最后更新人列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        public List<UserInfo> GetEditUsers(string companyCode, string channelID)
        {
            return _configProcessor.GetEditUsers(companyCode, channelID);
        }

        /// <summary>
        /// 创建配置单
        /// </summary>
        /// <param name="msg"></param>
        public void CreateComputerConfigMaster(ComputerConfigMaster msg)
        {
            _configProcessor.CreateComputerConfigMaster(msg);
        }

        /// <summary>
        /// 更新配置单
        /// </summary>
        /// <param name="msg"></param>
        public void UpdateComputerConfigMaster(ComputerConfigMaster msg)
        {
            _configProcessor.UpdateComputerConfigMaster(msg);
        }

        /// <summary>
        /// 根据用户输入的商品信息构造一个ConfigItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public ComputerConfigItem BuildConfigItem(ComputerConfigItem item)
        {
            return _configProcessor.BuildConfigItem(item);
        }

        public List<string> CheckOptionalAccessoriesItemAndCombos(List<int> sysNos)
        {
            return _configProcessor.CheckOptionalAccessoriesItemAndCombos(sysNos);
        }

        /// <summary>
        /// 作废配置单
        /// </summary>
        /// <param name="msg"></param>
        public void VoidComputerConfig(List<int> msg)
        {
            foreach (var sysNo in msg)
            {
                _configProcessor.VoidComputerConfigMaster(sysNo);
            }
        }

        /// <summary>
        /// 审核通过配置单
        /// </summary>
        /// <param name="msg"></param>
        public void ApprovePassComputerConfig(List<int> msg)
        {
            foreach (var sysNo in msg)
            {
                _configProcessor.AuditComputerConfigMaster(sysNo);
            }
        }

        /// <summary>
        /// 审核拒绝配置单
        /// </summary>
        /// <param name="msg"></param>
        public void ApproveDeclineComputerConfig(List<int> msg)
        {
            foreach (var sysNo in msg)
            {
                _configProcessor.DeclineComputerConfigMaster(sysNo);
            }
        }

        /// <summary>
        /// DIY装机调度
        /// </summary>
        public void CheckComputerConfigInfo()
        {
            _configProcessor.CheckComputerConfigInfo();
        }
    }
}
