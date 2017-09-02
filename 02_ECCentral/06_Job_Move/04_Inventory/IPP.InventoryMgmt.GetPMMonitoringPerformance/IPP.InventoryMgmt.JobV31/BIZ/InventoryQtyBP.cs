using System;
using System.Collections.Generic;
using System.Linq;
using Newegg.Oversea.Framework.Entity;
using IPP.InventoryMgmt.Taobao.JobV31.Provider;
using IPP.InventoryMgmt.Taobao.JobV31.BusinessEntities;
using IPP.InventoryMgmt.Taobao.JobV31.Common;
using Newegg.Oversea.Framework.JobConsole.Client;
using System.Text;

namespace IPP.InventoryMgmt.Taobao.JobV31.BIZ
{
    public class InventoryQtyBP
    {
        private string LogFile;
        private List<string> LogList = new List<string>();

        public void Run(JobContext context)
        {
            LogList.Clear();

            LogFile = context.Properties["BizLog"];

            WriteLog("第三方库存同步Job已启动。\r\n");

            WriteLog("正在检索本次需要同步库存的数据信息……");

            SynInventoryQtyBase synBP = new SynInventoryQtyBP();
            synBP.OnRunningAfter += new SynInventoryQtyRunning(ModifyLocalInventoryQty);

            //库存发生变化的商品
            List<ProductEntity> productList = GetProductEntityList();

            //需要同步库存的商品库存信息
            List<InventoryQtyEntity> inventoryQtyEntityList = GetInventoryEntity(productList);

            WriteLog(string.Format("本次共检测到{0}条需同步的数据", inventoryQtyEntityList.Count));

            CommonConst commonConst = new CommonConst();

            int batchNum = commonConst.BatchNumber;

            int pageSize = inventoryQtyEntityList.Count <= 0 ? 0 : (inventoryQtyEntityList.Count <= batchNum ? 1 : (inventoryQtyEntityList.Count % batchNum == 0 ? inventoryQtyEntityList.Count / batchNum : inventoryQtyEntityList.Count / batchNum + 1));

            WriteLog("开始第三方库存同步，请稍后……");

            for (int i = 0; i < pageSize; i++)
            {
                List<InventoryQtyEntity> list = inventoryQtyEntityList.Skip(i * batchNum).Take(batchNum).ToList();
                List<ProductEntity> productEntityListResult = FindProductListByInventoryQtyEntity(productList, list);
                WriteLog(string.Format("正在同步第{0}批数据，本批共{1}条数据", i + 1, list.Count));
                try
                {
                    synBP.SynInventoryQty(productEntityListResult, list);
                    WriteLog("本批次第三方库存同步结束，同步结果：成功");
                }
                catch (Exception ex)
                {

                    StringBuilder sb = new StringBuilder();
                    foreach (ProductEntity p in productEntityListResult)
                    {
                        sb.AppendFormat("ProductSysNo:({0})_SKU:({1})_ResultQty:({2})_WarehouseNumber:({3})\r\n", new object[]{
                            p.ProductSysNo,
                            p.SKU,
                            p.ResultQty,
                            p.WarehouseNumber
                        });
                    }
                    WriteLog(string.Format("同步出错商品信息：库存预警({0})\r\n{1}",commonConst.InventoryAlarmQty, sb.ToString()));

                    WriteLog(string.Format("{0}\r\n{1}", ex.Message, ex.StackTrace));
                    //break;
                }
            }

            WriteLog("同步库存完成，Job结束。\r\n\r\n\r\n");

            EndLog();
        }
        #region 私有方法

        private List<ProductEntity> GetProductEntityList()
        {
            QueryConditionEntity<QueryProduct> query = new QueryConditionEntity<QueryProduct>();
            query.PagingInfo = new PagingInfoEntity
            {
                MaximumRows = int.MaxValue,
                //SortField = "its.StockSysNo",
                StartRowIndex = 0
            };
            query.Condition = GetQueryProduct();

            List<ProductEntity> list = ProductQueryProvider.Query(query);

            return Addapter.CalculateInventoryQty.FilterModifyInventerResult(list);
            //return list;
        }
        private QueryProduct GetQueryProduct()
        {
            QueryProduct query = Addapter.CalculateInventoryQty.CreateQueryProduct();
            return query;
        }
        private List<InventoryQtyEntity> GetInventoryEntity(List<ProductEntity> list)
        {
            return Addapter.CalculateInventoryQty.CalculateQty(list);
        }
        private List<ProductEntity> FindProductListByInventoryQtyEntity(List<ProductEntity> list, InventoryQtyEntity inventoryQtEntity)
        {
            return list.FindAll(item => item.SKU == inventoryQtEntity.SKU);
        }
        private List<ProductEntity> FindProductListByInventoryQtyEntity(List<ProductEntity> list, List<InventoryQtyEntity> inventoryQtEntityList)
        {
            List<ProductEntity> result = new List<ProductEntity>();
            foreach (InventoryQtyEntity item in inventoryQtEntityList)
            {
                List<ProductEntity> temp = FindProductListByInventoryQtyEntity(list, item);
                result.InsertRange(0, temp);
            }
            return result;
        }

        private void ModifyLocalInventoryQty(object sender, InventoryQtyArgs args)
        {
            LocalInventoryQtyBP localBP = new LocalInventoryQtyBP();
            WriteLog("开始调整本地第三方库存……");
            try
            {
                //更新分仓库存
                localBP.Modify(args.ProductEntityList);

                //更新总库存
                localBP.Modify(args.InventoryQtyEntityList);

                WriteLog("本地第三方库存调整完毕。");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void WriteLog(string content)
        {
            Console.WriteLine(content);
            LogList.Add(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), content));
        }

        private void EndLog()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string log in LogList)
            {
                sb.AppendLine(log);
            }

            LogBP.WriteLog(sb.ToString(), LogFile);
        }
        #endregion
    }
}
