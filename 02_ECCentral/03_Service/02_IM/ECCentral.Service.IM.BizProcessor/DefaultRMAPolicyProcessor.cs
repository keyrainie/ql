using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Transactions;
using ECCentral.BizEntity;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(DefaultRMAPolicyProcessor))]
    public class DefaultRMAPolicyProcessor
    {
        #region Private
        private readonly IDefaultRMAPolicy rmaPolicyRequestDA =  ObjectFactory<IDefaultRMAPolicy>.Instance;
        private readonly IBrandDA brandRequestDA = ObjectFactory<IBrandDA>.Instance;
        #endregion
        
        #region Method
        public void DefaultRMAPolicyInfoAdd(DefaultRMAPolicyInfo defaultRMAPolicy)
        {
            //如果品牌为空 对应的所有品牌
            //是否存在重复
            //新增操作
            //记录日志
            List<String> errorlist = new List<String>();
            try
            {
                DefaultRMAPolicyInfo _oldDefaultRMAPolicy = rmaPolicyRequestDA.DefaultRMAPolicyByAll()
                 .Where(p => p.C3SysNo == defaultRMAPolicy.C3SysNo)
                 .Where(p => p.BrandSysNo == defaultRMAPolicy.BrandSysNo)
                 .Where(p => p.RMAPolicySysNo == defaultRMAPolicy.RMAPolicySysNo).Count() > 0
                  ? rmaPolicyRequestDA.DefaultRMAPolicyByAll()
                 .Where(p => p.C3SysNo == defaultRMAPolicy.C3SysNo)
                 .Where(p => p.BrandSysNo == defaultRMAPolicy.BrandSysNo)
                 .Where(p => p.RMAPolicySysNo == defaultRMAPolicy.RMAPolicySysNo).FirstOrDefault()
                 : null;
                if (_oldDefaultRMAPolicy != null)
                {
                    errorlist.Add(String.Format("C3SysNo:{0} " + ResouceManager.GetMessageString("IM.Brand", "Brand") + ":{1} " + ResouceManager.GetMessageString("IM.Product", "Repeated")
                        , _oldDefaultRMAPolicy.C3SysNo, _oldDefaultRMAPolicy.BrandSysNo));
                }
                else
                {
                    using (var tran = new TransactionScope())
                    {
                        defaultRMAPolicy.BrandSysNo = defaultRMAPolicy.BrandSysNo;
                        int _newId = rmaPolicyRequestDA.InsertDefaultRMAPolicyInfo(defaultRMAPolicy);
                        string LogNote = String.Format(ResouceManager.GetMessageString("IM.Product", "UserName") +
                               "：{0} " + ResouceManager.GetMessageString("IM.Product", "AddOperation") + " C3SysNo:{1} BrandSysNo:{2}  RMAPolicySysNo:{3}"
                               , defaultRMAPolicy.CreateUser.UserDisplayName
                               , defaultRMAPolicy.C3SysNo
                               , defaultRMAPolicy.BrandSysNo
                               , defaultRMAPolicy.RMAPolicySysNo);
                        //记录日志
                        ExternalDomainBroker.CreateOperationLog(LogNote
                           , BizLogType.DefaultRMAPolicy_Add, _newId, "8601");
                        tran.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                errorlist.Add(ex.ToString());
            }

            if (errorlist.Count > 0)
            {
                throw new BizException(string.Format(ResouceManager.GetMessageString("IM.Product", "Fail") + "{0}" + ResouceManager.GetMessageString("IM.Product", "NumberUnit") + "，\n" + ResouceManager.GetMessageString("IM.Product", "ContentIs") + "：{1}"
                    , errorlist.Count, errorlist.Join(";")));
            }
        }

        public void UpdateDefaultRMAPolicyBySysNo(DefaultRMAPolicyInfo defaultRMAPolicy)
        {
            //更新
            //是否存在重复
            //更新操作
            using (var tran = new TransactionScope())
            {
                List<DefaultRMAPolicyInfo> _defaultRMAPolicys = rmaPolicyRequestDA.DefaultRMAPolicyByAll();
                DefaultRMAPolicyInfo _oldDefaultRMAPolicy =
                      rmaPolicyRequestDA.DefaultRMAPolicyByAll()
                      .Where(p => p.C3SysNo == defaultRMAPolicy.C3SysNo)
                      .Where(p => p.BrandSysNo == defaultRMAPolicy.BrandSysNo)
                      .Where(p => p.RMAPolicySysNo == defaultRMAPolicy.RMAPolicySysNo).Count() > 0
                       ? rmaPolicyRequestDA.DefaultRMAPolicyByAll()
                      .Where(p => p.C3SysNo == defaultRMAPolicy.C3SysNo)
                      .Where(p => p.BrandSysNo == defaultRMAPolicy.BrandSysNo)
                      .Where(p => p.RMAPolicySysNo == defaultRMAPolicy.RMAPolicySysNo).FirstOrDefault()
                      : null;
                if (_oldDefaultRMAPolicy != null)
                    throw new BizException(String.Format("C3SysNo:{0} " + ResouceManager.GetMessageString("IM.Product", "Brand") + ":{1} " + ResouceManager.GetMessageString("IM.Product", "Repeated")
                            , _oldDefaultRMAPolicy.C3SysNo, _oldDefaultRMAPolicy.BrandSysNo));

                rmaPolicyRequestDA.UpdateDefaultRMAPolicyBySysNo(defaultRMAPolicy);
                string LogNote = String.Format(ResouceManager.GetMessageString("IM.Product", "UserName") +
                                 "：{0} " + ResouceManager.GetMessageString("IM.Product", "Updated") + ResouceManager.GetMessageString("IM.Product", "SourceData") + " RMAPolicySysNo:{1} " + ResouceManager.GetMessageString("IM.Product", "UpdateData") + " RMAPolicySysNo:{2}"
                                 , defaultRMAPolicy.EditUser.UserDisplayName
                                 , _defaultRMAPolicys.Where(p => p.SysNo == defaultRMAPolicy.SysNo).First().RMAPolicySysNo
                                 , defaultRMAPolicy.RMAPolicySysNo);
                //记录日志
                ExternalDomainBroker.CreateOperationLog(LogNote
                   , BizLogType.DefaultRMAPolicy_Edit
                   , Int32.Parse(defaultRMAPolicy.SysNo.ToString()), "8601");
                tran.Complete();
            }
           // throw new BizException("保存成功！");
        }

        public void DelDelDefaultRMAPolicyBySysNoBySysNos(List<DefaultRMAPolicyInfo> defaultRMAPolicyInfos)
        {
            using (var tranDel = new TransactionScope())
            {
                defaultRMAPolicyInfos.ForEach(p =>
                {
                    string LogNote =
                        String.Format(ResouceManager.GetMessageString("IM.Product", "UserName") + "：{0} " + ResouceManager.GetMessageString("IM.Product", "DeleteOperation")
 + " " + ResouceManager.GetMessageString("IM.Product", "BrandSysNo") + "{1}"
                        , p.EditUser.UserDisplayName, p.SysNo.ToString());
                    rmaPolicyRequestDA.DelDefaultRMAPolicyBySysNo(int.Parse(p.SysNo.ToString()));
                    ExternalDomainBroker.CreateOperationLog(
                        LogNote, BizLogType.DefaultRMAPolicy_Del
                        , int.Parse(p.SysNo.ToString()), "8601");
                });
                tranDel.Complete();
            }
            //throw new BizException("删除成功！");
        }
        #endregion
    }
}
