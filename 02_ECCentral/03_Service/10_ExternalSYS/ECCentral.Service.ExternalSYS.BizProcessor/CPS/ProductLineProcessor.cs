using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.Service.ExternalSYS.IDataAccess;
using ECCentral.BizEntity.ExternalSYS;
using ECCentral.BizEntity;
using System.Transactions;

namespace ECCentral.Service.ExternalSYS.BizProcessor
{
    [VersionExport(typeof(ProductLineProcessor))]
    public class ProductLineProcessor
    {
        private IProductLineDA productLineDA = ObjectFactory<IProductLineDA>.Instance;


        /// <summary>
        /// 创建产品线信息
        /// </summary>
        /// <param name="info"></param>
        public void CreateProductLine(ProductLineInfo info)
        {
            if (!productLineDA.IsExistsProductLine(info))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    ProductLineInfo product = productLineDA.CreateProductLine(info);
                    //创建之后修改优先级 
                    productLineDA.UpdatePriority(product.SysNo);
                    scope.Complete();
                }
            }
            else
            {
                throw new BizException("产品线名称在该类别下已存在,不能创建!");
            }
        }
        /// <summary>
        /// 更新产品线信息
        /// </summary>
        /// <param name="info"></param>
        public void UpdateProductLine(ProductLineInfo info)
        {
            if (!productLineDA.IsExistsProductLine(info))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    productLineDA.UpdateProductLine(info);
                    //更新基本信息后，在更新优先级
                    productLineDA.UpdatePriority(info.SysNo);
                    scope.Complete();
                }

            }
            else
            {
                throw new BizException("产品线名称在该类别下已存在,不能修改!");
            }
        }

        /// <summary>
        /// 删除产品线信息
        /// </summary>
        /// <param name="SysNo"></param>
        public void DeleteProductLine(int SysNo)
        {
            productLineDA.DeleteProductLine(SysNo);
        }
    }
}
