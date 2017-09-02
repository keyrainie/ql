using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.Taobao.JobV31.BusinessEntities;
using IPP.InventoryMgmt.JobV31.DataAccess;
using Newegg.Oversea.Framework.ExceptionHandler;

namespace IPP.InventoryMgmt.Taobao.JobV31.BIZ
{
    internal class LocalInventoryQtyBP
    {
        /// <summary>
        /// 修改总仓库存
        /// </summary>
        /// <param name="entityList"></param>
        public void Modify(List<InventoryQtyEntity> entityList)
        {
            try
            {
                ProductDA.ModifyQty(entityList);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
                throw ex;
            }
        }

        /// <summary>
        /// 修改分仓库存
        /// </summary>
        /// <param name="entityList"></param>
        public void Modify(List<ProductEntity> entityList)
        {
            try
            {
                ProductDA.ModifyQty(entityList);
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
                throw ex;
            }
        }
    }
}
