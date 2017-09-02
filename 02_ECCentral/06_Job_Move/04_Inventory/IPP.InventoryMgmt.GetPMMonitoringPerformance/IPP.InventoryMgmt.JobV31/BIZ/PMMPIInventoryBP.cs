using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using IPP.InventoryMgmt.JobV31.DataAccess;
using Newegg.Oversea.Framework.ExceptionHandler;

using System.Configuration;
using System.Threading;

namespace IPP.InventoryMgmt.JobV31.BIZ
{
    public class PMMPIInventoryBP
    {
        private string LogFile;
        private StringBuilder LogList = new StringBuilder();
       
        private int Message = 0;


        protected string CompanyCode
        {
            get
            {
                return ConfigurationManager.AppSettings["CompanyCode"];
            }
        }


        private List<PMMPISysNoEntity> _pms;
        protected List<PMMPISysNoEntity> PMs
        {
            get 
            {
                if(_pms==null)
                    _pms = PMMPIInventoryDA.Querty(CompanyCode);
                return _pms;
            }
 
        }

        public void Run(JobContext context)
        {
            try
            {

                if (context.Properties["Model"] == "Init")
                {
                    //LogList.Remove(0, LogList.Length);
                    //LogFile = context.Properties["BizLog"];

                    WriteLog("正在检索操作数据信息……\n", true);
                    WriteLog("初始化... ", true);

                    PMMPIInventoryDA.Trucate();
                    Console.WriteLine("历史数据清理完成\n");
                    Console.WriteLine("正在写入新的数据请稍等......\n");

                    foreach (PMMPISysNoEntity item in PMs)
                    {
                       PMMPIInventoryDA.InsertMPI(item.PMSysNumber.ToString());
                       Thread.Sleep(200);
                    }

                    Console.WriteLine("更新数据数据完成\n");
                }

                if (context.Properties["Model"] == "Update")
                {
                    //删除商品状态是非1的
                    Console.WriteLine("删除商品状态是非1的商品\n");
                    PMMPIInventoryDA.DeleteProduct();

                    //添加新的商品
                    Console.WriteLine("添加新的商品\n");
                    string pms = string.Join(",", PMs.Select(x => x.PMSysNumber.ToString()).ToArray());
                    if (!string.IsNullOrEmpty(pms))
                    {
                        PMMPIInventoryDA.ImportNewProduct(pms);
                    }

                    //更新
                    Console.WriteLine("开始更新\n");
                    int loop=0;
                    int affectRows=1;
                    while (affectRows!=0 && loop<1000)
                    {

                        affectRows = PMMPIInventoryDA.UpdateProductNew();

                        loop += 1;
                    }


                    Console.WriteLine("更新数据数据完成\n");



                }





                //if (context.Properties["Model"] == "Update")
                //{

                //    string pms = string.Join(",", PMs.Select(x => x.PMSysNumber.ToString()).ToArray());

                //    if (!string.IsNullOrEmpty(pms))
                //    {
                //        PMMPIInventoryDA.ImportNewProduct(pms);
                //    }

                  

                //    //检索所有已有的缺货商品并更新为不缺货
                //    PMMPIInventoryDA.InitiStatus();

                //    //检索新的缺货商品,更新数据
                //    List<PMMPIInventoryEntity> shortageproduct = PMMPIInventoryDA.QueryShortageProductsNow();

                //    foreach (PMMPIInventoryEntity product in shortageproduct)
                //    {
                //        if (product.StockSysNo == 51
                //           || product.StockSysNo == 59)
                //        {


                //            List<PMMPIInventoryEntity> sameProducts = shortageproduct.FindAll(x => x.ProductSysNo == product.ProductSysNo
                //                && (x.StockSysNo == 51 || x.StockSysNo == 59) && x.IsOutOfStock == 1);

                //            if (sameProducts != null && sameProducts.Count>0)
                //                product.IsOutOfStock = 0;
                //            else
                //                product.IsOutOfStock = 1;

                //        }
                //        else
                //        {
                //            product.IsOutOfStock = 1;
                //        }

                //        PMMPIInventoryDA.UpdateProduct(product);
                //    }
                //    Console.WriteLine("更新数据数据完成\n");
                //}


            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex);
                WriteLog(string.Format("{0} \r\n {1}", ex.Message, ex.StackTrace), true);
            }
            finally
            {
            }


        }
   
        #region 私有方法
   
        private void WriteLine(string mes)
        {
            Console.WriteLine(mes);
        }

        private void WriteLog(string mes, bool output)
        {
            if (output)
            {
                WriteLine(mes);
            }
            LogList.AppendLine(string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), mes));
            if (LogList.Capacity > 5000)
            {
                EndLog();
            }
        }

        private void WriteLog(string mes)
        {
            WriteLog(mes, false);
        }
        private void EndLog()
        {
            LogBP.WriteLog(LogList.ToString(), LogFile);
            LogList.Remove(0, LogList.Length);
        }
        #endregion
    }
}
