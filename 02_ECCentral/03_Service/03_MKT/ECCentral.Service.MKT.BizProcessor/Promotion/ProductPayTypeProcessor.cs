using System;
using System.Collections.Generic;
using ECCentral.BizEntity;
using ECCentral.BizEntity.MKT.Promotion;
using ECCentral.Service.MKT.IDataAccess.Promotion;
using ECCentral.Service.Utility;

namespace ECCentral.Service.MKT.BizProcessor.Promotion
{
    [VersionExport(typeof(ProductPayTypeProcessor))]
    public class ProductPayTypeProcessor
    {
        private readonly IProductPayTypeDA _productPayTypeDA = ObjectFactory<IProductPayTypeDA>.Instance;

        public void BatchCreateProductPayType(ProductPayTypeInfo productPayTypeInfo)
        {
            if (string.IsNullOrEmpty(productPayTypeInfo.ProductIds))
            {
                //throw new BizException("商品ID不能为空");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductPayType", "ProductPayType_PIDNotNull"));
            }
            if (productPayTypeInfo.PayTypeList == null || productPayTypeInfo.PayTypeList.Count == 0)
            {
                //throw new BizException("支付方式ID不能为空");
                throw new BizException(ResouceManager.GetMessageString("MKT.ProductPayType", "ProductPayType_PayTypeIDNotNull"));
            }
            var productIds = productPayTypeInfo.ProductIds.Split('\r');
            var successCount = 0;
            var message = string.Empty;
            var idList = new List<string>();
            foreach (var productID in productIds)
            {
                if (idList.Contains(productID)) continue;
                idList.Add(productID);
                productPayTypeInfo.ProductID = productID.Trim();
                foreach (var payType in productPayTypeInfo.PayTypeList)
                {
                    productPayTypeInfo.PayTypeSysNo = payType.PayTypeSysNo;
                    try
                    {
                        var result = CreateProductPayType(productPayTypeInfo);
                        if (result == -2)
                        {
                           // message += string.Format("商品ID：{0}不支持支付方式创建失败，失败原因：商品信息不存在\r", productID);
                            message += string.Format(ResouceManager.GetMessageString("MKT.ProductPayType", "ProductPayType_NotExsistProductInfo"), productID);
                            continue;
                        }
                    }
                    catch (Exception ex)
                    {
                        //message += string.Format("商品ID：{0}不支持支付方式创建失败，失败原因：{1}\r", productID, ex.Message);
                        message += string.Format(ResouceManager.GetMessageString("MKT.ProductPayType", "ProductPayType_PayTypeNotSupport"), productID);
                        continue;
                    }
                    successCount++;
                }
            }
            //message = string.Format("成功添加{0}条\r", successCount) + message;
            message = string.Format(ResouceManager.GetMessageString("MKT.ProductPayType", "ProductPayType_SuccessCount"), successCount) + message;
            throw new BizException(message);
        }

        public void BathAbortProductPayType(string productPayTypeIds, string editUser)
        {
            var ids = productPayTypeIds.Split(',');
            var successCount = 0;
            var message = string.Empty;
            foreach (var id in ids)
            {
                try
                {
                    var result = AbortProductPayType(id, editUser);
                    if (result == -2)
                    {
                        //message += string.Format("商品支付方式{0}中止失败，失败原因：商品支付方式不存在\r", id);
                        message += string.Format(ResouceManager.GetMessageString("MKT.ProductPayType", "ProductPayType_PayTypeNotExsist"), id);
                        continue;
                    }
                }
                catch (Exception ex)
                {
                    //message += string.Format("商品支付方式{0}中止失败，失败原因：{1}\r", id, ex.Message);
                    message += string.Format(ResouceManager.GetMessageString("MKT.ProductPayType", "ProductPayType_StopFailed"), id, ex.Message);
                    continue;
                }
                successCount++;
            }
            //message = string.Format("成功中止支付方式{0}条\r", successCount) + message;
            message = string.Format(ResouceManager.GetMessageString("MKT.ProductPayType", "ProductPayType_StopSuccess"), successCount) + message;
            throw new BizException(message);
        }

        public int AbortProductPayType(string sysNo, string editUser)
        {
            return _productPayTypeDA.AbortProductPayType(sysNo, editUser);
        }

        public int CreateProductPayType(ProductPayTypeInfo productPayTypeInfo)
        {
            return _productPayTypeDA.CreateProductPayType(productPayTypeInfo);
        }
    }
}
