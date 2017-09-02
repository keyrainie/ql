using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.IBizInteract;
using ECCentral.BizEntity.Common;
using ECCentral.Service.Inventory.BizProcessor;

namespace ECCentral.Service.Inventory.AppService
{
    [VersionExport(typeof(InentoryAppService))]
    public class InentoryAppService
    {
        public List<int> QueryPMListByRight(PMQueryType queryType, string LoginName, string companyCode)
        {
            List<int> result = new List<int>();
            var info = ObjectFactory<IIMBizInteract>.Instance.GetPMListByType(queryType, LoginName, companyCode);
            if (info != null && info.Count > 0)
            {
                foreach (var item in info)
                {
                    result.Add(item.SysNo.Value);
                }
            }
            return result;
        }

        /// <summary>
        /// 判断当前用户是否有操作此商品的权限
        /// </summary>
        /// <param name="userSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public bool CheckOperateRightForCurrentUser(int userSysNo,int productSysNo)
        {
            return ObjectFactory<ICommonBizInteract>.Instance.CheckOperateRightForCurrentUser(productSysNo,userSysNo);       
        }

        /// <summary>
        /// 根据商品sysNo获取每个商品的产品线和所属PM
        /// </summary>
        /// <param name="productLineSysNo"></param>
        /// <param name="productSysNo"></param>
        /// <returns></returns>
        public List<ProductPMLine> GetProductLineSysNoByProductList(int[] productSysNo)
        {

            List<ProductPMLine> result = ObjectFactory<ICommonBizInteract>.Instance.GetProductLineSysNoByProductList(productSysNo);
            return result;
        }

        /// <summary>
        /// 发送月底库存邮件
        /// </summary>
        /// <param name="emailList">发送地址列表</param>
        /// <param name="title">标题可以不填</param>
        /// <param name="content">发送内容</param>
        /// <returns>返回成功发送列表</returns>
        public void SendInventoryEmailEndOfMonth(string address, string language, string downloadPath, string savePath)
        {
            ObjectFactory<ProductInventoryProcessor>.Instance.SendInventoryEmailEndOfMonth(address, language, downloadPath, savePath);
        }
    }
}
