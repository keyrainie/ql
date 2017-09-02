using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.MKT.AppService;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Utility;
using System.ServiceModel.Web;
using ECCentral.Service.Utility.WCF;
using ECCentral.QueryFilter.MKT;
using ECCentral.Service.MKT.IDataAccess.NoBizQuery;

namespace ECCentral.Service.MKT.Restful
{
    public partial class MKTService
    {
        //分页查询配置单
        [WebInvoke(UriTemplate = "/ComputerConfig/Query", Method = "POST")]
        public virtual QueryResult QueryComputerConfig(ComputerConfigQueryFilter filter)
        {
            var queryDA = ObjectFactory<IComputerConfigQueryDA>.Instance;
            int totalCount;
            var data = queryDA.QueryMaster(filter, out totalCount);

            return new QueryResult
            {
                Data = data,
                TotalCount = totalCount
            };
        }

        private ComputerConfigAppService _configAppService = ObjectFactory<ComputerConfigAppService>.Instance;

        /// <summary>
        /// 获取所有组件列表
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/ComputerConfig/AllComputerParts")]
        public List<ComputerParts> GetAllComputerParts()
        {
            return _configAppService.GetAllComputerParts();
        }

        [WebGet(UriTemplate = "/ComputerConfig/{id}")]
        public ComputerConfigMaster LoadComputerConfig(string id)
        {
            int sysNo = int.Parse(id);
            return _configAppService.LoadComputerConfig(sysNo);
        }

        /// <summary>
        /// 获取配置单类型列表
        /// </summary>
        /// <returns></returns>
        [WebGet(UriTemplate = "/ComputerConfig/AllConfigType")]
        public List<ComputerConfigType> LoadAllConfigType()
        {
            return _configAppService.LoadAllConfigType();
        }

        /// <summary>
        /// 获取配置单最后更新人列表
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="channelID"></param>
        /// <returns></returns>
        [WebGet(UriTemplate = "/ComputerConfig/AllEditUser/{companyCode}/{channelID}")]
        public List<UserInfo> GetConfigEditUsers(string companyCode, string channelID)
        {
            return _configAppService.GetEditUsers(companyCode, channelID);
        }

        /// <summary>
        /// 创建配置单
        /// </summary>
        /// <param name="msg"></param>
        [WebInvoke(UriTemplate = "/ComputerConfig/Create", Method = "POST")]
        public void CreateComputerConfigMaster(ComputerConfigMaster msg)
        {
            _configAppService.CreateComputerConfigMaster(msg);
        }

        /// <summary>
        /// 更新配置单
        /// </summary>
        /// <param name="msg"></param>
        [WebInvoke(UriTemplate = "/ComputerConfig/Update", Method = "PUT")]
        public void UpdateComputerConfigMaster(ComputerConfigMaster msg)
        {
            _configAppService.UpdateComputerConfigMaster(msg);
        }

        /// <summary>
        /// Check Items是否存在于随心配与销售规则
        /// </summary>
        /// <param name="sysNos"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ComputerConfig/CheckOptionalAccessoriesItemAndCombos", Method = "POST")]
        public List<string> CheckOptionalAccessoriesItemAndCombos(List<int> sysNos)
        {
            return _configAppService.CheckOptionalAccessoriesItemAndCombos(sysNos);
        }

         /// <summary>
        /// 根据用户输入的商品信息构造一个ConfigItem
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [WebInvoke(UriTemplate = "/ComputerConfig/BuildConfigItem", Method = "POST")]
        public ComputerConfigItem BuildConfigItem(ComputerConfigItem item)
        {
            return _configAppService.BuildConfigItem(item);
        }

        /// <summary>
        /// 作废配置单
        /// </summary>
        /// <param name="msg"></param>
        [WebInvoke(UriTemplate = "/ComputerConfig/Void", Method = "PUT")]
        public void VoidComputerConfig(List<int> msg)
        {
            _configAppService.VoidComputerConfig(msg);
        }

        /// <summary>
        /// 审核通过配置单
        /// </summary>
        /// <param name="msg"></param>
        [WebInvoke(UriTemplate = "/ComputerConfig/ApprovePass", Method = "PUT")]
        public void ApprovePassComputerConfig(List<int> msg)
        {
            _configAppService.ApprovePassComputerConfig(msg);
        }

        /// <summary>
        /// 审核拒绝配置单
        /// </summary>
        /// <param name="msg"></param>
        [WebInvoke(UriTemplate = "/ComputerConfig/ApproveDecline", Method = "PUT")]
        public void ApproveDeclineComputerConfig(List<int> msg)
        {
            _configAppService.ApproveDeclineComputerConfig(msg);
        }
    }
}
