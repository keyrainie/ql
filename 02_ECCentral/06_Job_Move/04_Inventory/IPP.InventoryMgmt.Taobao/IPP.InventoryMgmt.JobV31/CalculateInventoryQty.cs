using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;

using IPP.InventoryMgmt.JobV31.Common;

namespace IPP.InventoryMgmt.JobV31
{
    internal class CalculateInventoryQty : ICalculateInventoryQty
    {
        private CommonConst _Common = null;
        private CommonConst Common { get { if (_Common == null) _Common = new CommonConst(); return _Common; } }
        //public List<InventoryQtyEntity> CalculateQty(List<ProductEntity> entityList)
        //{
        //    CommonConst commonConst = new CommonConst();
        //    List<InventoryQtyEntity> result = new List<InventoryQtyEntity>();
        //    List<string> skus = (from list in entityList select list.SKU).Distinct().ToList();
        //    foreach (string id in skus)
        //    {
        //        int InventoryQty = entityList.FindAll(item => item.SKU == id).Sum(item => item.ResultQty);

        //        //如果库存未发生变化，则不做同步处理
        //        if (InventoryQty != 0)
        //        {
        //            ProductEntity entity = entityList.First(item => item.SKU == id);
        //            InventoryQtyEntity qtyEntity = new InventoryQtyEntity();

        //            int alarmQty = entity.InventoryAlarmQty.HasValue ? entity.InventoryAlarmQty.Value : 0;
        //            //总库存-(预警值-原始预警值)
        //            InventoryQty -= (commonConst.InventoryAlarmQty - alarmQty);
        //            //总库存变化量
        //            qtyEntity.InventoryQty = InventoryQty;

        //            if (entity.MappingInventoryQty < 0)
        //            {
        //                InventoryQty += entity.MappingInventoryQty;
        //            }

        //            qtyEntity.SKU = id;
        //            qtyEntity.ProductMappingSysNo = entity.ProductMappingSysNo;
        //            qtyEntity.ProductID = entity.ProductID;
        //            qtyEntity.ProductSysNo = entity.ProductSysNo;
        //            qtyEntity.SynInventoryQty = InventoryQty;
        //            qtyEntity.PartnerType = entity.PartnerType;
        //            qtyEntity.InventoryAlarmQty = entity.InventoryAlarmQty;
        //            result.Add(qtyEntity);
        //        }
        //        //移除已被计算过的数据项，已提交集合下次筛选的速度
        //        // entityList.RemoveAll(item => item.SKU == id);
        //    }
        //    return result;
        //}

        //public QueryProduct CreateQueryProduct()
        //{
        //    //CommonConst commonConst = new CommonConst();
        //    QueryProduct query = new QueryProduct();

        //    query.PartnerType = Common.PartnerType;
        //    query.WareHourseNumber = Common.WareHourseNumbers;
        //    query.CompanyCode = Common.CompanyCode;

        //    return query;
        //}

        //public List<ProductEntity> FilterModifyInventerResult(List<ProductEntity> entityList)
        //{
        //    return entityList.FindAll(item => item.ResultQty != 0);
        //}

        public int CalculateSynQty(ThirdPartInventoryEntity entity)
        {
            int result = CalculateQty(entity);
            if (entity.SynInventoryQty < 0)
            {
                result += entity.SynInventoryQty;
            }

            return result;
        }

        public int CalculateQty(ThirdPartInventoryEntity entity)
        {
            int result = 0;
            int alam = entity.InventoryAlamQty ?? Common.InventoryAlarmQty;

            result = entity.InventoryOnlineQty - entity.SynInventoryQty;

            result = result - alam ;

            return result;
        }
    }
}
