using System;
using ECCentral.BizEntity;
using ECCentral.BizEntity.IM;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.Service.Utility;
using System.Transactions;
using ECCentral.Service.EventMessage.IM;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(BrandRequestProcessor))]
    public class BrandRequestProcessor
    {
        private readonly IBrandRequestDA brandRequestDA = ObjectFactory<IBrandRequestDA>.Instance;
        private readonly BrandProcessor brandprocessor = ObjectFactory<BrandProcessor>.Instance;
        /// <summary>
        /// 审核
        /// </summary>
        /// <param name="info"></param>
        public void AuditBrandRequest(BrandRequestInfo info)
        {
            if (!brandRequestDA.BrandCheckUser(info))
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    if (info.ReustStatus == "A")//审核通过
                    {
                        BrandInfo brandInfo = new BrandInfo()
                        {
                            BrandDescription = info.BrandDescription,
                            BrandNameGlobal = info.BrandNameGlobal,
                            BrandNameLocal = info.BrandNameLocal,
                            BrandSupportInfo = info.BrandSupportInfo,
                            Manufacturer = info.Manufacturer,
                            Status = info.Status,
                            LanguageCode = info.LanguageCode,
                            CompanyCode = info.CompanyCode,
                            BrandCode = info.BrandCode,
                            User = info.User
                        };



                        //创建Brand
                        BrandInfo b = brandprocessor.CreateBrand(brandInfo);
                    }
                    //更新BrandRequest表
                    brandRequestDA.AuditBrandRequest(info);

                    //更新之后发送消息
                    switch (info.ReustStatus)
                    {
                        //审核通过
                        case "A":
                            EventPublisher.Publish<ECCentral.Service.EventMessage.IM.BrandAuditMessage>(new ECCentral.Service.EventMessage.IM.BrandAuditMessage()
                            {
                                AuditUserSysNo = ServiceContext.Current.UserSysNo,
                                RequestSysNo = info != null ? info.SysNo : 0
                            });
                            break;
                        //审核拒绝
                        case "D":
                            EventPublisher.Publish<ECCentral.Service.EventMessage.IM.BrandRejectMessage>(new ECCentral.Service.EventMessage.IM.BrandRejectMessage()
                            {
                                RejectUserSysNo = ServiceContext.Current.UserSysNo,
                                RequestSysNo = info != null ? info.SysNo : 0
                            });
                            break;
                    }

                    scope.Complete();
                }
            }

        }
        ///// <summary>
        ///// 提交审核 
        ///// </summary>
        ///// <param name="info"></param>
        //public void InsertBrandRequest(BrandRequestInfo info)
        //{
        //    if (info.Manufacturer == null)
        //    {
        //        int sysno = ObjectFactory<IManufacturerDA>.Instance.IsExistManufacturerName(info.BrandNameLocal.Content,0);
        //        if (sysno > 0)
        //        {
        //            info.Manufacturer = new ManufacturerProcessor().GetManufacturerInfoBySysNo(sysno);
        //        }
        //        else
        //        {
        //            ManufacturerInfo manufacturer = new ManufacturerInfo();
        //            manufacturer.ManufacturerNameLocal = info.BrandNameLocal;
        //            manufacturer.ManufacturerNameGlobal = info.BrandNameGlobal;
        //            manufacturer.Status = ManufacturerStatus.Active;
        //            manufacturer.SupportInfo = new ManufacturerSupportInfo();
        //            info.Manufacturer = new ManufacturerProcessor().CreateManufacturer(manufacturer);
        //        }
        //    }
        //    if (!brandRequestDA.IsExistsBrandRequest(info))
        //    {
        //        if (!brandRequestDA.IsExistsBrandRequest_New(info))
        //        {
        //            using (TransactionScope scope = new TransactionScope())
        //            {

        //                brandRequestDA.InsertBrandRequest(info);

        //                //提交审核之后发送消息
        //                EventPublisher.Publish<BrandAuditSubmitMessage>(new BrandAuditSubmitMessage()
        //                {
        //                    SubmitUserSysNo = ServiceContext.Current.UserSysNo,
        //                    RequestSysNo = info != null ? info.SysNo : 0
        //                });

        //                scope.Complete();
        //            }
        //        }
        //        else
        //        {
        //            throw new BizException("该品牌已经被重复提交，请检查是否已存在或者正处于审核阶段！");
        //        }

        //    }
        //    else
        //    {
        //        throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandAuditing"));
        //    }
        //}

        /// <summary>
        /// 提交审核 
        /// </summary>
        /// <param name="info"></param>
        public void InsertBrandRequest(BrandRequestInfo info)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                if (info.Manufacturer == null)
                {
                    int sysno = ObjectFactory<IManufacturerDA>.Instance.IsExistManufacturerName(info.BrandNameLocal.Content, 0);
                    if (sysno > 0)
                    {
                        info.Manufacturer = new ManufacturerProcessor().GetManufacturerInfoBySysNo(sysno);
                    }
                    else
                    {
                        ManufacturerInfo manufacturer = new ManufacturerInfo();
                        manufacturer.ManufacturerNameLocal = info.BrandNameLocal;
                        manufacturer.ManufacturerNameGlobal = info.BrandNameGlobal;
                        manufacturer.Status = ManufacturerStatus.Active;
                        manufacturer.SupportInfo = new ManufacturerSupportInfo();
                        info.Manufacturer = new ManufacturerProcessor().CreateManufacturer(manufacturer);
                    }
                }
                if (!brandRequestDA.IsExistsBrandRequest(info))
                {
                    if (!brandRequestDA.IsExistsBrandRequest_New(info))
                    {

                            brandRequestDA.InsertBrandRequest(info);

                            //提交审核之后发送消息
                            EventPublisher.Publish<BrandAuditSubmitMessage>(new BrandAuditSubmitMessage()
                            {
                                SubmitUserSysNo = ServiceContext.Current.UserSysNo,
                                RequestSysNo = info != null ? info.SysNo : 0
                            });
                    }
                    else
                    {
                        throw new BizException("该品牌已经被重复提交，请检查是否已存在或者正处于审核阶段！");
                    }

                }
                else
                {
                    throw new BizException(ResouceManager.GetMessageString("IM.Brand", "BrandAuditing"));
                }
                scope.Complete();
            }
        }
    }
}
