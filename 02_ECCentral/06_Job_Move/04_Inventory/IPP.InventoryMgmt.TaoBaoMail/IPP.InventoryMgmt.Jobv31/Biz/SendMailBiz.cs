using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.BusinessEntities;
//[Mark:Remove]using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.ServiceContracts;
using Newegg.Oversea.Framework.ServiceConsole.Client;
//[Mark:Remove]using IPP.Oversea.CN.ServiceCommon.ServiceInterfaces.DataContracts;
using IPP.InventoryMgmt.JobV31.Common;
using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ExceptionHandler;

namespace IPP.InventoryMgmt.JobV31.Biz
{
    public class SendMailBiz
    {
        public string CreateBody(List<TaobaoProduct> ThirdPartMappingNotExists)
        {
            if (ThirdPartMappingNotExists == null || ThirdPartMappingNotExists.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();

            foreach (TaobaoProduct product in ThirdPartMappingNotExists)
            {
                sb.Append("<tr>");
                sb.Append(string.Format("<td>{0}</td>", product.ProductID));
                sb.Append(string.Format("<td>{0}</td>", product.NumberID));
                sb.Append(string.Format("<td>{0}</td>", product.Qty));
                sb.Append(string.Format("<td>{0}</td>", product.Status));
                sb.Append("</tr>");
            }

            return sb.ToString();
        }

        public string CreateBody(List<ThirdPartInventoryEntity> TaobaoProductNotExists)
        {
            if (TaobaoProductNotExists == null || TaobaoProductNotExists.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();

            foreach (ThirdPartInventoryEntity entity in TaobaoProductNotExists)
            {
                sb.Append("<tr>");
                sb.Append(string.Format("<td>{0}</td>", entity.ProductMappingSysno));
                sb.Append(string.Format("<td>{0}</td>", entity.ProductSysNo));
                sb.Append(string.Format("<td>{0}</td>", entity.SKU));
                sb.Append(string.Format("<td>{0}</td>", entity.Status));
                sb.Append(string.Format("<td>{0}</td>", entity.InventoryOnlineQty));
                //sb.Append(string.Format("<td>{0}</td>", entity.OldInventoryAlamQty));
                //sb.Append(string.Format("<td>{0}</td>", entity.InventoryAlamQty));
                //sb.Append(string.Format("<td>{0}</td>", entity.SynInventoryQty));
                sb.Append("</tr>");
            }

            return sb.ToString();
        }

        public string CreateBody(Dictionary<ThirdPartInventoryEntity, TaobaoProduct> InventoryQtyNotEquels)
        {
            if (InventoryQtyNotEquels == null || InventoryQtyNotEquels.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();

            foreach (ThirdPartInventoryEntity entity in InventoryQtyNotEquels.Keys)
            {
                TaobaoProduct product = InventoryQtyNotEquels[entity];
                sb.Append("<tr>");
                sb.Append(string.Format("<td>{0}</td>", entity.ProductMappingSysno));
                sb.Append(string.Format("<td>{0}</td>", entity.ProductSysNo));
                sb.Append(string.Format("<td>{0}</td>", entity.SKU));
                sb.Append(string.Format("<td>{0}</td>", product.ProductID));
                sb.Append(string.Format("<td style='color:red'>{0}</td>", entity.InventoryOnlineQty));
                sb.Append(string.Format("<td>{0}</td>", entity.OldInventoryAlamQty));
                sb.Append(string.Format("<td>{0}</td>", entity.InventoryAlamQty));
                sb.Append(string.Format("<td>{0}</td>", entity.Status));
                sb.Append(string.Format("<td style='color:red'>{0}</td>", product.Qty));
                sb.Append(string.Format("<td>{0}</td>", product.Status));
                sb.Append("</tr>");
            }
            return sb.ToString();
        }

        public string GetMailTemplate(MailType type)
        {
            switch (type)
            {
                case MailType.InventoryQtyNotEquels:
                    return Resource.MailTemplate_InventoryQtyNotEquels;
                case MailType.ThirdPartMappingNotExists:
                    return Resource.MailTemplate_ThirdPartMappingNotExists;
                case MailType.TaobaoProductNotExists:
                default:
                    return Resource.MailTemplate_TaobaoProductNotExists;
            }
        }

        public void SendMail(MailEntity entity)
        {
            ISendMail service = ServiceBroker.FindService<ISendMail>();
            MailBodyV31 mail = new MailBodyV31();
            mail.Body = new MailBodyMsg
            {
                CCMailAddress = entity.CC,
                CreateDate = DateTime.Now,
                MailBody = entity.Body,
                MailFrom = entity.From,
                MailTo = entity.To,
                Subjuect = entity.Subject
            };
            mail.Header = Util.CreateServiceHeader();

            DefaultDataContract contract = service.SendMail2MailDBInternal(mail);

            if (contract != null && contract.Faults != null && contract.Faults.Count > 0)
            {
                MessageFault fault = contract.Faults[0];
                throw new Exception(string.Format("{0}\r\n{1}\r\n{2}", fault.ErrorCode, fault.ErrorDescription, fault.ErrorDetail));
            }
        }

        public static void SendMail(MailDataEntity mailDataEntity)
        {
            SendMailDelegate<List<TaobaoProduct>> ThirdPartMappingNotExists = new SendMailDelegate<List<TaobaoProduct>>(SendMail);
            SendMailDelegate<List<ThirdPartInventoryEntity>> TaobaoProductNotExists = new SendMailDelegate<List<ThirdPartInventoryEntity>>(SendMail);
            SendMailDelegate<Dictionary<ThirdPartInventoryEntity, TaobaoProduct>> InventoryQtyNotEquels = new SendMailDelegate<Dictionary<ThirdPartInventoryEntity, TaobaoProduct>>(SendMail);

            IAsyncResult ThirdPartMappingNotExists_Result = ThirdPartMappingNotExists.BeginInvoke(mailDataEntity.ThirdPartMappingNotExists, null, null);
            IAsyncResult TaobaoProductNotExists_Result = TaobaoProductNotExists.BeginInvoke(mailDataEntity.TaobaoProductNotExists, null, null);
            IAsyncResult InventoryQtyNotEquels_Result = InventoryQtyNotEquels.BeginInvoke(mailDataEntity.InventoryQtyNotEquels, null, null);

            ThirdPartMappingNotExists.EndInvoke(ThirdPartMappingNotExists_Result);
            TaobaoProductNotExists.EndInvoke(TaobaoProductNotExists_Result);
            InventoryQtyNotEquels.EndInvoke(InventoryQtyNotEquels_Result);
        }

        private static void SendMail(List<TaobaoProduct> ThirdPartMappingNotExists)
        {
            SendMailBiz biz = new SendMailBiz();
            int PageSize = Config.MailContentPageSize;
            int page = 1;
            int PageCount = 1;
            int records = ThirdPartMappingNotExists == null ? 0 : ThirdPartMappingNotExists.Count;
            List<MailEntity> mailList = new List<MailEntity>();
            string template = biz.GetMailTemplate(MailType.ThirdPartMappingNotExists);
            if (records > 0)
            {
                PageCount = records <= PageSize ? 1 : (records % PageSize == 0 ? records / PageSize : records / PageSize + 1);
                for (; page <= PageCount; page++)
                {
                    var list = ThirdPartMappingNotExists.Skip((page - 1) * PageSize).Take(PageSize).ToList();
                    string ThirdPartMappingNotExists_Body = biz.CreateBody(list);

                    string templateResult = template.Replace("$ThirdPartMappingNotExists$", ThirdPartMappingNotExists_Body);
                    templateResult = templateResult.Replace("$ThirdPartMappingNotExists_Count$", list.Count.ToString());
                    MailEntity entity = new MailEntity
                    {
                        Body = templateResult,
                        CC = Config.MailCCAddress,
                        From = Config.MailFrom,
                        Subject = Config.MailSubject,
                        To = Config.MailAddress
                    };
                    entity.Subject += "_淘宝存在的商品，本地Mapping无映射";
                    if (PageCount > 1)
                    {
                        entity.Subject += string.Format("_数据总量({0})_第{1}页({2})", records, page, list.Count);
                    }
                    //biz.SendMail(entity);
                    mailList.Add(entity);
                }
            }
            else
            {
                string templateResult = template.Replace("$ThirdPartMappingNotExists$", "");
                templateResult = templateResult.Replace("$ThirdPartMappingNotExists_Count$", "0");
                MailEntity entity = new MailEntity
                {
                    Body = templateResult,
                    CC = Config.MailCCAddress,
                    From = Config.MailFrom,
                    Subject = Config.MailSubject,
                    To = Config.MailAddress
                };
                entity.Subject += "_淘宝存在的商品，本地Mapping无映射";
                mailList.Add(entity);
            }
            SendMail(mailList);
        }

        private static void SendMail(List<ThirdPartInventoryEntity> TaobaoProductNotExists)
        {
            SendMailBiz biz = new SendMailBiz();
            int PageSize = Config.MailContentPageSize;
            int page = 1;
            int PageCount = 1;
            int records = TaobaoProductNotExists == null ? 0 : TaobaoProductNotExists.Count;
            List<MailEntity> mailList = new List<MailEntity>();
            string template = biz.GetMailTemplate(MailType.TaobaoProductNotExists);
            if (records > 0)
            {
                PageCount = records <= PageSize ? 1 : (records % PageSize == 0 ? records / PageSize : records / PageSize + 1);
                for (; page <= PageCount; page++)
                {
                    var list = TaobaoProductNotExists.Skip((page - 1) * PageSize).Take(PageSize).ToList();
                    string TaobaoProductNotExists_Body = biz.CreateBody(list);

                    string templateResult = template.Replace("$TaobaoProductNotExists$", TaobaoProductNotExists_Body);
                    templateResult = templateResult.Replace("$TaobaoProductNotExists_Count$", list.Count.ToString());
                    MailEntity entity = new MailEntity
                    {
                        Body = templateResult,
                        CC = Config.MailCCAddress,
                        From = Config.MailFrom,
                        Subject = Config.MailSubject,
                        To = Config.MailAddress
                    };
                    entity.Subject += "_本地Mapping有数据，淘宝已售完或不存在商品";
                    if (PageCount > 1)
                    {
                        entity.Subject += string.Format("_数据总量({0})_第{1}页({2})", records, page, list.Count);
                    }
                    //biz.SendMail(entity);
                    mailList.Add(entity);
                }
            }
            else
            {
                string templateResult = template.Replace("$TaobaoProductNotExists$", "");
                templateResult = templateResult.Replace("$TaobaoProductNotExists_Count$", "0");
                mailList.Add(new MailEntity
                    {
                        Body = templateResult,
                        CC = Config.MailCCAddress,
                        From = Config.MailFrom,
                        Subject = Config.MailSubject + "_本地Mapping有数据，淘宝已售完或不存在商品",
                        To = Config.MailAddress
                    });
            }
            SendMail(mailList);
        }

        private static void SendMail(Dictionary<ThirdPartInventoryEntity, TaobaoProduct> InventoryQtyNotEquels)
        {
            SendMailBiz biz = new SendMailBiz();
            int PageSize = Config.MailContentPageSize;
            int page = 1;
            int PageCount = 1;
            int records = InventoryQtyNotEquels == null ? 0 : InventoryQtyNotEquels.Count;
            List<MailEntity> mailList = new List<MailEntity>();
            string template = biz.GetMailTemplate(MailType.InventoryQtyNotEquels);
            if (records > 0)
            {
                PageCount = records <= PageSize ? 1 : (records % PageSize == 0 ? records / PageSize : records / PageSize + 1);
                for (; page <= PageCount; page++)
                {
                    var list = InventoryQtyNotEquels.Skip((page - 1) * PageSize).Take(PageSize).ToDictionary(item => item.Key, elem => elem.Value);
                    string InventoryQtyNotEquels_Body = biz.CreateBody(list);
                    string templateResult = template.Replace("$InventoryQtyNotEquels$", InventoryQtyNotEquels_Body);
                    templateResult = templateResult.Replace("$InventoryQtyNotEquels_Count$", list.Count.ToString());
                    MailEntity entity = new MailEntity
                    {
                        Body = templateResult,
                        CC = Config.MailCCAddress,
                        From = Config.MailFrom,
                        Subject = Config.MailSubject,
                        To = Config.MailAddress
                    };
                    entity.Subject += "_本地库存和淘宝库存不同步";
                    if (PageCount > 1)
                    {
                        entity.Subject += string.Format("_数据总量({0})_第{1}页({2})", records, page, list.Count);
                    }
                    //biz.SendMail(entity);
                    mailList.Add(entity);
                }
            }
            else
            {
                string templateResult = template.Replace("$InventoryQtyNotEquels$", string.Empty);
                templateResult = templateResult.Replace("$InventoryQtyNotEquels_Count$", "0");
                mailList.Add(
                    new MailEntity
                    {
                        Body = templateResult,
                        CC = Config.MailCCAddress,
                        From = Config.MailFrom,
                        Subject = Config.MailSubject + "_本地库存和淘宝库存不同步",
                        To = Config.MailAddress
                    }
                    );
            }
            SendMail(mailList);
        }

        private static void SendMail(List<MailEntity> mailList)
        {
            SendMailBiz biz = new SendMailBiz();
            List<SendMailDelegate<MailEntity>> delegateList = new List<SendMailDelegate<MailEntity>>();
            List<IAsyncResult> resultList = new List<IAsyncResult>();
            for (int i = 0; i < mailList.Count; i++)
            {
                MailEntity entity = mailList[i];
                SendMailDelegate<MailEntity> sendMail = new SendMailDelegate<MailEntity>(biz.SendMail);
                delegateList.Add(sendMail);
                resultList.Add(sendMail.BeginInvoke(entity, null, null));
            }
            for (int i = 0; i < delegateList.Count; i++)
            {
                SendMailDelegate<MailEntity> mailDelegate = delegateList[i];
                mailDelegate.EndInvoke(resultList[i]);
            }
        }
    }
}
