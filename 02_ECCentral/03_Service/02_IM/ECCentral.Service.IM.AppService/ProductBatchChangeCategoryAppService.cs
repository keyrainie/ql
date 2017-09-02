//************************************************************************
// 用户名				泰隆优选
// 系统名				商品批量移类
// 子系统名		        商品批量移类业务实现
// 作成者				Kevin
// 改版日				2012.6.7
// 改版内容				新建
//************************************************************************

using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.BizProcessor;
using ECCentral.Service.Utility;
using System.Collections.Generic;
using ECCentral.BizEntity;
using System.Text;

namespace ECCentral.Service.IM.AppService
{

    [VersionExport(typeof(ProductBatchChangeCategoryAppService))]
    public class ProductBatchChangeCategoryAppService
    {

        /// <summary>
        /// 批量更改商品类别
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public void BatchChangeCategory(ProductBatchChangeCategoryInfo entity)
        {
            StringBuilder result = new StringBuilder();

            int successCount = 0;
            int errorCount = 0;

            foreach (string productID in entity.ProductIDs)
            {
                try
                {
                    if (productID.Trim().Equals(string.Empty)) continue;
                    ObjectFactory<ProductBatchChangeCategoryProcessor>.Instance.ProductChangeCategory(productID,entity.CategoryInfo,entity.EditUser);
                    successCount++;
                }
                catch (BizException ex)
                {
                    string message = ResouceManager.GetMessageString("IM.Category", "ProductID");
                    message += "：{0}，";
                    message += ResouceManager.GetMessageString("IM.Category", "FailReason");
                    message += "：{1}";
                    result.AppendLine(string.Format(message, productID, ex.Message));
                    errorCount++;
                }
            }

            string resMessage = ResouceManager.GetMessageString("IM.Category", "MoveCategorySuccess");
            resMessage += "：{0}，";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += ResouceManager.GetMessageString("IM.Category", "MoveCategoryFail");
            resMessage += "：{1}";
            resMessage += ResouceManager.GetMessageString("IM.Category", "NumberUnit");
            resMessage += "。\r\n";
            result.Insert(0, string.Format(resMessage, successCount, errorCount));

            throw new BizException(result.ToString());

        }

    }
}
