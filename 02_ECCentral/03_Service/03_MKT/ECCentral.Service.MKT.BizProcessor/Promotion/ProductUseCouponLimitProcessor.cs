using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.MKT.IDataAccess;
using ECCentral.BizEntity.MKT;
using ECCentral.BizEntity;

namespace ECCentral.Service.MKT.BizProcessor
{
    [VersionExport(typeof(ProductUseCouponLimitProcessor))]
    public class ProductUseCouponLimitProcessor
    {
        private IProductUseCouponLimitDA _ProductUseCouponLimitDA = ObjectFactory<IProductUseCouponLimitDA>.Instance;
        /// <summary>
        /// 创建
        /// </summary>
        /// <param name="info"></param>
        public void CreateProductUseCouponLimit(List<ProductUseCouponLimitInfo> list)
        {
            Dictionary<int, List<ProductUseCouponLimitInfo>> errorList = new Dictionary<int, List<ProductUseCouponLimitInfo>>();
            errorList.Add(-1, new List<ProductUseCouponLimitInfo>());
            errorList.Add(1, new List<ProductUseCouponLimitInfo>());
            errorList.Add(2, new List<ProductUseCouponLimitInfo>());
            errorList.Add(3, new List<ProductUseCouponLimitInfo>());
            foreach (var item in list)
            {
                int result = _ProductUseCouponLimitDA.CreateProductUseCouponLimit(item);
                errorList[result].Add(item);
            }
            string errorstr = string.Empty;
            //errorstr = string.Format("新增成功{0}个;更新状态{1}个;失败{2}个\r\n", errorList[1].Count(), errorList[2].Count, errorList[-1].Count + errorList[3].Count);
            errorstr = string.Format(ResouceManager.GetMessageString("MKT.BuyLimit", "BuyLimit_AddSuccessUpStateFiale"), errorList[1].Count(), errorList[2].Count, errorList[-1].Count + errorList[3].Count);
            foreach (var item in errorList.Keys)
            {
                if (item == -1)
                {
                    foreach (var error in errorList[item])
                    {
                        //errorstr = errorstr + string.Format("商品[{0}]不存在\r\n", error.ProductId);
                        errorstr = errorstr + string.Format(ResouceManager.GetMessageString("MKT.BuyLimit", "BuyLimit_TheProductNotExsist"), error.ProductId);
                    }
                }
                if (item == 3)
                {
                    foreach (var error in errorList[item])
                    {
                        //errorstr = errorstr + string.Format("商品[{0}]已存在并有效\r\n", error.ProductId);
                        errorstr = errorstr + string.Format(ResouceManager.GetMessageString("MKT.BuyLimit", "BuyLimit_ExsistActiveProduct"), error.ProductId);
                    }
                }
            }
            if (!string.IsNullOrEmpty(errorstr))
            {
                throw new BizException(errorstr);
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="list"></param>
        public void ModifyProductUseCouponLimit(List<ProductUseCouponLimitInfo> list)
        {
            foreach (var item in list)
            {
                if (item.CouponLimitType == CouponLimitType.Manually) //手动类型的数据做物理删除
                {
                    _ProductUseCouponLimitDA.DeleteProductUseCouponLimit(item.SysNo);
                    ExternalDomainBroker.CreateOperationLog(
                    String.Format("{0}{1}SysNo:{2}",
                    DateTime.Now.ToString(), "物理删除特殊商品限制使用优惠券"
                    , item.SysNo)
                    , BizEntity.Common.BizLogType.ComputerConfig_Add
                    , item.SysNo, "8601");//[Mark][Alan.X.Luo 硬编码]
                }
                else //其他类型数据做逻辑删除（修改状态）
                {
                    _ProductUseCouponLimitDA.UpdateProductUseCouponLimitStatus(item.SysNo, ADStatus.Deactive);
                    ExternalDomainBroker.CreateOperationLog(
                      String.Format("{0}{1}SysNo:{2}状态:{3}",
                      DateTime.Now.ToString(), "修改特殊商品限制使用优惠券状态"
                      , item.SysNo, "无效")
                      , BizEntity.Common.BizLogType.ComputerConfig_Add
                      , item.SysNo, "8601");//[Mark][Alan.X.Luo 硬编码]
                }
            }
        }


        #region Job行为
        public virtual List<ProductJobLimitProductInfo> GetLimitProductList(string datacommandname)
        {
            return _ProductUseCouponLimitDA.GetLimitProductList(datacommandname);
        }
        public virtual void DeleteProductUseCouponLimit(int ProductSysNo)
        {
            _ProductUseCouponLimitDA.DeleteProductUseCouponLimit(ProductSysNo);
        }
        public virtual ProductJobLimitProductInfo GetLimitProductByProductSysNo(int productSysNo)
        {
            return _ProductUseCouponLimitDA.GetLimitProductByProductSysNo(productSysNo);
        }
        public virtual void CreateLimitProduct(ProductJobLimitProductInfo entity)
        {
            _ProductUseCouponLimitDA.CreateLimitProduct(entity);
        }
        #endregion
    }

}
