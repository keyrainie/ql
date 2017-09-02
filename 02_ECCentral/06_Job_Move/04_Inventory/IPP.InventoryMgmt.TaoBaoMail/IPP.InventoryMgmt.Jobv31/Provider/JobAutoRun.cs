using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.JobConsole.Client;
using IPP.InventoryMgmt.JobV31.Biz;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
using Newegg.Oversea.Framework.ExceptionHandler;
using IPP.InventoryMgmt.JobV31.Common;

namespace IPP.InventoryMgmt.JobV31.Provider
{
    public class JobAutoRun : IJobAction
    {
        public void Run(JobContext context)
        {
            Console.WriteLine("库存监测job已启动。");
            try
            {
                Console.WriteLine("正在进行数据检测，请稍后……");
                BIZ biz = new BIZ();
                MailDataEntity mailDataEntity = new MailDataEntity();
                int mappingCount = biz.ThirdPartInventoryEntityList == null ? 0 : biz.ThirdPartInventoryEntityList.Count;
                int taobaoCount = biz.TaobaoProductList == null ? 0 : biz.TaobaoProductList.Count;
                Console.WriteLine("数据检测完毕，本地Mapping数据量({0}),淘宝商品数量({1})", mappingCount, taobaoCount);
                Console.WriteLine("正在进行数据分析处理……");
                mailDataEntity.InventoryQtyNotEquels = biz.GetInventoryQtyNotEquels();
                mailDataEntity.TaobaoProductNotExists = biz.GetTaobaoProductNotExists();
                mailDataEntity.ThirdPartMappingNotExists = biz.GetThirdPartMappingNotExists();
                Console.WriteLine("数据分析完毕：");
                Console.WriteLine("\t本地Mapping有记录，淘宝已售完或无此商品({0})", mailDataEntity.TaobaoProductNotExists == null ? 0 : mailDataEntity.TaobaoProductNotExists.Count);
                Console.WriteLine("\t淘宝有商品，本地Mapping无记录({0})", mailDataEntity.ThirdPartMappingNotExists == null ? 0 : mailDataEntity.ThirdPartMappingNotExists.Count);
                Console.WriteLine("\t库存不同步({0})", mailDataEntity.InventoryQtyNotEquels == null ? 0 : mailDataEntity.InventoryQtyNotEquels.Count);
                Console.WriteLine("邮件发送中……");
                SendMailBiz.SendMail(mailDataEntity);
                Console.WriteLine("邮件发送成功。");
            }
            catch (Exception ex)
            {
                ExceptionHelper.HandleException(ex, Config.MailSubject + "_异常信息", new object[] { });
                Console.WriteLine("{0}\r\n{1}", ex.Message, ex.StackTrace);
            }
            Console.WriteLine("本次Job监测完毕。");
        }

    }
}
