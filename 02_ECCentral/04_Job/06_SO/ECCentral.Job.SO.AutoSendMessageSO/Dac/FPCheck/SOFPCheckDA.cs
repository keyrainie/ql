using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Job.SO.AutoSendMessageSO.BusinessEntities.FPCheck;
using Newegg.Oversea.Framework.DataAccess;
using ECCentral.Job.SO.AutoSendMessageSO.BusinessEntities.Common;

namespace ECCentral.Job.SO.AutoSendMessageSO.Dac.FPCheck
{
   public class SOFPCheckDA
    {
       public static List<SOEntity4FPEntity> GetSOList4FPCheck(int topCount, string CompanyCode)
       {
           List<SOEntity4FPEntity> result = new List<SOEntity4FPEntity>();
           CustomDataCommand command = DataCommandManager.CreateCustomDataCommandFromConfig("GetSOList4FPCheck");
           command.CommandText = command.CommandText.Replace("#TOPCOUNT#", topCount.ToString());
           command.AddInputParameter("@CompanyCode", System.Data.DbType.StringFixedLength, CompanyCode);
           result = command.ExecuteEntityList<SOEntity4FPEntity>();
           result = (result == null) ? new List<SOEntity4FPEntity>() : result;
           return result;
       }
       public static List<int> GetAutoRMACustomerSysNos(string CompanyCode)
       {
           List<int> result = new List<int>();
           List<SingleValueEntity> queryResult = new List<SingleValueEntity>();
           DataCommand command=DataCommandManager.GetDataCommand("GetAutoRMACustomerSysNos");
           command.SetParameterValue("@CompanyCode", CompanyCode);
           queryResult = command.ExecuteEntityList<SingleValueEntity>();
           if(queryResult!=null && queryResult.Count>0)
           {
               queryResult.ForEach(x => result.Add(x.Int32Value));
           }
           queryResult=null;
           GC.Collect();
           if(result==null){
           result=new List<int>();
           }
           return result;
       }

       public static List<FPCheckMasterEntity> GetFPCheckMasterList(string CompanyCode)
       {
           List<FPCheckMasterEntity> result = new List<FPCheckMasterEntity>();
           return result;
       } 
    }
}
