using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Transactions;
using ECCentral.Service.EventMessage.IM;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ManufacturerRequestProcessor))]
    public class ManufacturerRequestProcessor
    {
        private readonly IManufacturerRequestDA manufacturerRequestDA = ObjectFactory<IManufacturerRequestDA>.Instance;
        private readonly ManufacturerProcessor manufacturerProcessor = ObjectFactory<ManufacturerProcessor>.Instance;
        /// <summary>
        /// 生产商审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditManufacturerRequest(ManufacturerRequestInfo info)
        {
            // bug 2832 
            //if (!manufacturerRequestDA.CheckManufacturerUser(info.SysNo))
            //{
            ManufacturerInfo manufacture;//创建或更改后的生产商
            ManufacturerInfo manufacturrtInfo = new ManufacturerInfo()
                    {
                        Status = info.ManufacturerStatus,
                        ManufacturerNameLocal = new LanguageContent() { Content = info.ManufacturerName },
                        ManufacturerDescription = new LanguageContent() { Content = info.Reasons },
                        ManufacturerNameGlobal = info.ManufacturerBriefName,
                        SupportInfo = new ManufacturerSupportInfo(),
                        ManufacturerID = info.ManufacturerSysNo.ToString(),
                        SysNo = info.ManufacturerSysNo,
                        LanguageCode = info.LanguageCode,
                        CompanyCode = info.CompanyCode
                    };
            //CheckExistManufacturerName(info.ManufacturerBriefName, info.ManufacturerSysNo);
            using (TransactionScope scope = new TransactionScope()) //check 逻辑在创建和更新方法中
            {
                if (info.Status == 1)
                {
                    if (info.OperationType == 1)
                    {
                        //创建生产商
                        manufacture = manufacturerProcessor.CreateManufacturer(manufacturrtInfo);
                        info.ManufacturerSysNo = manufacture.SysNo;//将创建后的生产商在更新回审核表
                    }
                    if (info.OperationType == 2)
                    {
                        //修改生产商
                        manufacture = manufacturerProcessor.UpdateManufacturer(manufacturrtInfo);
                    }
                }
                manufacturerRequestDA.AuditManufacturerRequest(info);

                //状态更新之后发送消息
                switch (info.Status)
                {
                    //审核通过
                    case 1:
                        EventPublisher.Publish<ECCentral.Service.EventMessage.IM.ManufacturerAuditMessage>(new ECCentral.Service.EventMessage.IM.ManufacturerAuditMessage()
                        {
                            AuditUserSysNo = ServiceContext.Current.UserSysNo,
                            RequestSysNo = info != null ? info.SysNo : 0,
                            ManufacturerSysNo = info != null ? info.ManufacturerSysNo : 0
                        });
                        break;
                    //审核拒绝
                    case -1:
                        EventPublisher.Publish<ECCentral.Service.EventMessage.IM.ManufacturerRejectMessage>(new ECCentral.Service.EventMessage.IM.ManufacturerRejectMessage()
                        {
                            RejectUserSysNo = ServiceContext.Current.UserSysNo,
                            RequestSysNo = info != null ? info.SysNo : 0,
                            ManufacturerSysNo = info != null ? info.ManufacturerSysNo : 0
                        });
                        break;
                    //审核取消
                    case -2:
                        EventPublisher.Publish<ECCentral.Service.EventMessage.IM.ManufacturerCancelMessage>(new ECCentral.Service.EventMessage.IM.ManufacturerCancelMessage()
                        {
                            CancelUserSysNo = ServiceContext.Current.UserSysNo,
                            RequestSysNo = info != null ? info.SysNo : 0,
                            ManufacturerSysNo = info != null ? info.ManufacturerSysNo : 0
                        });
                        break;
                }

                scope.Complete();
            }
            //}
            //else
            //{
            //    throw new BizException("审核人和创建人不能是同一个人!");
            //}
        }
        public void InsertManufacturerRequest(ManufacturerRequestInfo info)
        {
            if (!manufacturerRequestDA.IsExistsManufacturerRequest(info))
            {
                if (!manufacturerRequestDA.CheckIsExistsManufacturer(info.ManufacturerName, info.ManufacturerBriefName))
                {
                    using (TransactionScope scope = new TransactionScope())
                    {

                        manufacturerRequestDA.InsertManufacturerRequest(info);

                        //提交审核之后发送消息
                        EventPublisher.Publish<ManufacturerAuditSubmitMessage>(new ManufacturerAuditSubmitMessage()
                        {
                            SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                            RequestSysNo = info != null ? info.SysNo : 0,
                            ManufacturerSysNo = info != null ? info.ManufacturerSysNo : 0
                        });

                        scope.Complete();
                    }
                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ExistNameManufacturer"));
                }
            }
            else
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerAuditing"));
            }
        }

        /// <summary>
        /// 是否存在某个同名的厂商
        /// </summary>
        /// <param name="name">国际化名称</param>
        /// <param name="manufacturerSysNo"></param>
        /// <returns></returns>
        public static void CheckExistManufacturerName(string name, int? manufacturerSysNo)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerNameIsNull"));
            }

            if (manufacturerSysNo == null)
            {
                manufacturerSysNo = 0;
            }
            if (manufacturerSysNo.Value < 0)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ManufacturerSysNOIsNull"));
            }
            var manufacturerDA = ObjectFactory<IManufacturerDA>.Instance;
            var isExist = manufacturerSysNo == 0 ? manufacturerDA.IsExistManufacturerName(name) : manufacturerDA.IsExistManufacturerName(name, manufacturerSysNo.Value)>0;
            if (isExist)
            {
                throw new BizException(ResouceManager.GetMessageString("IM.Manufacturer", "ExistManufacturerName"));
            }
        }
    }
}
