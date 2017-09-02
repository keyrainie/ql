using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Framework.ExceptionHandler;
using ECCentral.Job.SO.SIMUnicomSO.Common;
using ECCentral.Job.SO.SIMUnicomSO.DataAccess;
using ECCentral.Job.Utility.SendEmail.Resource;
using ECCentral.Job.Utility.Email;
using ECCentral.Job.Utility.Email.DataContracts;
using Newegg.Oversea.Framework.Utilities.String;
using Newegg.Oversea.Framework.ExceptionBase;
namespace ECCentral.Job.SO.SIMUnicomSO.Logic
{
   public class UnicomSOBP
    {
       //每页查询的数据
       private const  int Page_Size = 200;
       private static int Index = 0;
       private static int Count = 0;
       private static List<SOSIMCardEntity> GetAllData(){
           int count = UnicomSODA.GetUnicomSOListCount();
           int page = 1;
           int pageCount = (count > page) ? (count % page == 0 ? count / page : count / page + 1) : page;
           IEnumerable<SOSIMCardEntity> list = new List<SOSIMCardEntity>();
           for (; page <= pageCount; page++)
           {
               var temp = UnicomSODA.GetList(page, Page_Size);
               list = list.Union(temp);
           }
           if (list.Count() > 0)
           {
               IEqualityComparer<SOSIMCardEntity> compare = new SOSIMCardCompare();
               return list.Distinct(compare).ToList();
           }
           return list.ToList();
       }

       public static void Run() 
       {
           Index = 0;
           Count = 0;
           Console.WriteLine("正在检测数据");
           List<SOSIMCardEntity> list = null;
           try 
           {
             list = GetAllData();  //上面的方法
           }
           catch(Exception ex)
           {
               Console.WriteLine(ex.Message);
               Console.WriteLine(ex.StackTrace);
               ExceptionHelper.HandleException(ex);
           }
           Console.WriteLine("本次共检测到{0}条数据", list.Count);
           if (list != null && list.Count > 0)
           {
               Console.WriteLine("已启动邮件发送...");
               list.ForEach(delegate(SOSIMCardEntity entity)
               {
                  EmailBP.SendMail(entity, OnSendMailCallback, OnSendMailExceptionHandle);                      
               });
           }
           else 
           {
               EndJOB();
           }
       }
           
       private static void OnSendMailCallback(OnSendingMailArgs args)
       {
           Index++;
           Console.WriteLine("SO#_{0}SIM卡激活提醒邮件已发送", args.SIMCardEntity.SOSysNo);
           EndJOB();
       }

       private static void OnSendMailExceptionHandle(Exception ex, OnSendingMailArgs args)
       {
           Index++;
           string mes = string.Format("SO#_{0}SIM卡激活提醒邮件发送失败", args.SIMCardEntity.SOSysNo);
           ExceptionHelper.HandleException(ex, string.Format("联通合约机{0}", mes), new object[] { args.SIMCardEntity.SOSysNo });
           Console.WriteLine(mes);
           Console.WriteLine(ex.Message);
           Console.WriteLine(ex.StackTrace);
           EndJOB();
       }

       private static void EndJOB() 
       { 
          if(Index>=Count)
          {
              Console.WriteLine(Index > 0 ? "邮件发送完毕,JOB退出" : "Job退出");
          }
       }
    }
   internal class SOSIMCardCompare : IEqualityComparer<SOSIMCardEntity> 
   {
       public bool Equals(SOSIMCardEntity x,SOSIMCardEntity y)
       {
         if(x==null||y==null)
         {
             return x == y;
         }
         return x.SOSysNo == y.SOSysNo && x.SIMSN == y.SIMSN;
       }
       public int GetHashCode(SOSIMCardEntity obj)
       {
          if(obj==null)
          {
              return 0;
          }
          return obj.GetHashCode();
       }
   }
}
