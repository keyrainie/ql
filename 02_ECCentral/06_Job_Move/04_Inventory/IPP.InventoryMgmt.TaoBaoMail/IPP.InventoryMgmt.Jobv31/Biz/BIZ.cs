using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.Common;
using IPP.InventoryMgmt.JobV31.BusinessEntities;

namespace IPP.InventoryMgmt.JobV31.Biz
{
    public class BIZ
    {
        public BIZ()
        {
            Init();
        }

        public List<ThirdPartInventoryEntity> ThirdPartInventoryEntityList { get; private set; }

        public List<TaobaoProduct> TaobaoProductList { get; private set; }

        private void Init()
        {
            QueryThirdPartInventoryEntity QueryThirdPart = new QueryThirdPartInventoryEntity(InventoryQtyBiz.QueryProduct);
            QueryTaobaoProduct QueryTaobao = new QueryTaobaoProduct(SynInventoryQtyBiz.QuerySynProduct);
            IAsyncResult thirdPartResult = QueryThirdPart.BeginInvoke(null, null);
            IAsyncResult taobaoResult = QueryTaobao.BeginInvoke(null, null);
            ThirdPartInventoryEntityList = QueryThirdPart.EndInvoke(thirdPartResult);
            TaobaoProductList = QueryTaobao.EndInvoke(taobaoResult);
        }

        /// <summary>
        /// 检测Mapping中没有记录的淘宝商品
        /// </summary>
        /// <returns></returns>
        public List<TaobaoProduct> GetThirdPartMappingNotExists()
        {
            List<TaobaoProduct> list = new List<TaobaoProduct>();
            foreach (TaobaoProduct product in TaobaoProductList)
            {
                ThirdPartInventoryEntity entity = ThirdPartInventoryEntityList.Find(item => item.ProductID == product.ProductID);
                if (entity == null)
                {
                    list.Add(product);
                }
            }
            return list;
        }

        /// <summary>
        /// 检测Mapping表中有记录，但淘宝没有的商品
        /// </summary>
        /// <returns></returns>
        public List<ThirdPartInventoryEntity> GetTaobaoProductNotExists()
        {
            List<ThirdPartInventoryEntity> list = new List<ThirdPartInventoryEntity>();
            foreach (ThirdPartInventoryEntity entity in ThirdPartInventoryEntityList)
            {
                TaobaoProduct product = TaobaoProductList.Find(item => item.ProductID == entity.ProductID);
                if (product == null)
                {
                    list.Add(entity);
                }
            }
            return list;
        }

        /// <summary>
        /// 检测本地库存和淘宝库存不平衡的信息
        /// </summary>
        /// <returns></returns>
        public Dictionary<ThirdPartInventoryEntity, TaobaoProduct> GetInventoryQtyNotEquels()
        {
            Dictionary<ThirdPartInventoryEntity, TaobaoProduct> dic = new Dictionary<ThirdPartInventoryEntity, TaobaoProduct>();
            foreach (ThirdPartInventoryEntity entity in ThirdPartInventoryEntityList)
            {
                TaobaoProduct product = TaobaoProductList.Find(item => item.ProductID == entity.ProductID);
                if (product != null)
                {
                    if (entity.InventoryOnlineQty - (entity.InventoryAlamQty ?? 0) != product.Qty)
                    {
                        if (!dic.ContainsKey(entity))
                        {
                            dic.Add(entity, product);
                        }
                    }
                }
            }
            return dic;
        }
    }
}
