using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using ECCentral.BizEntity.IM.Product;
using ECCentral.Service.IM.IDataAccess;
using ECCentral.BizEntity.IM;
using ECCentral.BizEntity;

namespace ECCentral.Service.IM.BizProcessor
{
    [VersionExport(typeof(ProductEntryInfoProcessor))]
    public class ProductEntryInfoProcessor
    {
        private readonly IProductEntryInfoDA _ProductEntryInfo = ObjectFactory<IProductEntryInfoDA>.Instance;

        //public bool InsertEntryInfo(ProductEntryInfo entity)
        //{
        //    return _ProductEntryInfo.InsertEntryInfo(entity);
        //}

        public bool UpdateEntryInfo(ProductEntryInfo entity)
        {
            return _ProductEntryInfo.UpdateEntryInfo(entity);
        }

        public ProductEntryInfo LoadProductEntryInfo(int productSysNo)
        {
            List<int> productSysNoList = new List<int>();
            productSysNoList.Add(productSysNo);

            List<ProductEntryInfo> productEntryList = _ProductEntryInfo.GetProductEntryInfoList(productSysNoList);
            if (productEntryList != null && productEntryList.Count > 0)
            {
                return productEntryList.First();
            }

            return null;
        }

        /// <summary>
        /// 审核通过
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool AuditSucess(ProductEntryInfo info)
        {
            ProductEntryInfo temp = LoadProductEntryInfo(info.ProductSysNo.Value);
            string InspectionNum;
            if (temp.EntryStatus != ProductEntryStatus.WaitingAudit)
            {
                throw new BizException("备案状态不是待审核，无法审核通过！");
            }

            if(temp.AuditNote==null)
                temp.AuditNote="";
            if (string.IsNullOrWhiteSpace(info.AuditNote))
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：审核通过。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：审核通过。 备注：{1}。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), info.AuditNote);
            }

            do
            {
                InspectionNum = GenerateRandomNumber(6);
            } while (!ObjectFactory<IProductEntryInfoDA>.Instance.checkInspectionNum(InspectionNum));

            info.InspectionNum = InspectionNum;

            return ObjectFactory<IProductEntryInfoDA>.Instance.AuditSucess(info);
        }

        /// <summary>
        /// 审核不通过
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool AuditFail(ProductEntryInfo info)
        {
            ProductEntryInfo temp = LoadProductEntryInfo(info.ProductSysNo.Value);

            if (temp.EntryStatus != ProductEntryStatus.WaitingAudit)
            {
                throw new BizException("备案状态不是待审核，无法审核不通过！");
            }

            if (temp.AuditNote == null)
                temp.AuditNote = "";
            if (string.IsNullOrWhiteSpace(info.AuditNote))
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：审核不通过。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：审核不通过。 备注：{1}。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), info.AuditNote);
            }

            return ObjectFactory<IProductEntryInfoDA>.Instance.AuditFail(info);
        }

        /// <summary>
        /// 提交商检
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool ToInspection(ProductEntryInfo info)
        {
            ProductEntryInfo temp = LoadProductEntryInfo(info.ProductSysNo.Value);

            if (temp.EntryStatus != ProductEntryStatus.AuditSucess)
            {
                throw new BizException("备案状态不是审核成功，无法提交商检！");
            }

            if (temp.AuditNote == null)
                temp.AuditNote = "";
            if (string.IsNullOrWhiteSpace(info.AuditNote))
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：提交商检。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：提交商检。 备注：{1}。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), info.AuditNote);
            }

            return ObjectFactory<IProductEntryInfoDA>.Instance.ToInspection(info);
        }

        /// <summary>
        /// 商检通过
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool InspectionSucess(ProductEntryInfo info)
        {
            ProductEntryInfo temp = LoadProductEntryInfo(info.ProductSysNo.Value);

            if (temp.EntryStatus != ProductEntryStatus.Entry)
            {
                throw new BizException("备案状态不是备案中，无法商检成功！");
            }
            if (temp.EntryStatusEx != ProductEntryStatusEx.Inspection)
            {
                throw new BizException("备案扩展状态不是待商检，无法商检成功！");
            }

            if (temp.AuditNote == null)
                temp.AuditNote = "";
            if (string.IsNullOrWhiteSpace(info.AuditNote))
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：商检通过。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。操作：商检通过。 备注：{1}。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), info.AuditNote);
            }

            return ObjectFactory<IProductEntryInfoDA>.Instance.InspectionSucess(info);
        }

        /// <summary>
        /// 商检不通过
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool InspectionFail(ProductEntryInfo info)
        {
            ProductEntryInfo temp = LoadProductEntryInfo(info.ProductSysNo.Value);
            if (temp.EntryStatus != ProductEntryStatus.Entry)
            {
                throw new BizException("备案状态不是备案中，无法商检不通过！");
            }
            if (temp.EntryStatusEx != ProductEntryStatusEx.Inspection)
            {
                throw new BizException("备案扩展状态不是待商检，无法商检不通过！");
            }

            if (temp.AuditNote == null)
                temp.AuditNote = "";
            if (string.IsNullOrWhiteSpace(info.AuditNote))
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：商检不通过。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：商检不通过。 备注：{1}。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), info.AuditNote);
            }

            return ObjectFactory<IProductEntryInfoDA>.Instance.InspectionFail(info);
        }

        /// <summary>
        /// 提交报关
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool ToCustoms(ProductEntryInfo info)
        {
            ProductEntryInfo temp = LoadProductEntryInfo(info.ProductSysNo.Value);
            if (temp.EntryStatus != ProductEntryStatus.Entry)
            {
                throw new BizException("备案状态不是备案中，无法提交报关！");
            }
            if (temp.EntryStatusEx != ProductEntryStatusEx.InspectionSucess)
            {
                throw new BizException("备案扩展状态不是商检成功，无法提交报关！");
            }

            if (temp.AuditNote == null)
                temp.AuditNote = "";
            if (string.IsNullOrWhiteSpace(info.AuditNote))
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：提交报关。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：提交报关。 备注：{1}。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), info.AuditNote);
            }

            return ObjectFactory<IProductEntryInfoDA>.Instance.ToCustoms(info);
        }

        /// <summary>
        /// 报关通过
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool CustomsSuccess(ProductEntryInfo info)
        {
            ProductEntryInfo temp = LoadProductEntryInfo(info.ProductSysNo.Value);
            if (temp.EntryStatus != ProductEntryStatus.Entry)
            {
                throw new BizException("备案状态不是备案中，无法报关通过！");
            }
            if (temp.EntryStatusEx != ProductEntryStatusEx.Customs)
            {
                throw new BizException("备案扩展状态不是待报关，无法报关通过！");
            }

            if (temp.AuditNote == null)
                temp.AuditNote = "";
            if (string.IsNullOrWhiteSpace(info.AuditNote))
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：报关通过。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：报关通过。 备注：{1}。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), info.AuditNote);
            }

            return ObjectFactory<IProductEntryInfoDA>.Instance.CustomsSuccess(info);
        }

        /// <summary>
        /// 报关不通过
        /// </summary>
        /// <param name="sysNo"></param>
        /// <returns></returns>
        public bool CustomsFail(ProductEntryInfo info)
        {
            ProductEntryInfo temp = LoadProductEntryInfo(info.ProductSysNo.Value);
            if (temp.EntryStatus != ProductEntryStatus.Entry)
            {
                throw new BizException("备案状态不是备案中，无法报关不通过！");
            }
            if (temp.EntryStatusEx != ProductEntryStatusEx.Customs)
            {
                throw new BizException("备案扩展状态不是待报关，无法报关不通过！");
            }

            if (temp.AuditNote == null)
                temp.AuditNote = "";
            if (string.IsNullOrWhiteSpace(info.AuditNote))
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：报关不通过。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                info.AuditNote = temp.AuditNote + string.Format("时间：{0}。 操作：报关不通过。 备注：{1}。 \n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), info.AuditNote);
            }

            return ObjectFactory<IProductEntryInfoDA>.Instance.CustomsFail(info);
        }

        #region 生成随机商检备案流水号

        static Random rd;

        private static char[] constant =   
        {   
            '0','1','2','3','4','5','6','7','8','9',  
            'a','b','c','d','e','f','g','h','i','j','k','m','n','p','q','r','s','t','u','v','w','x','y','z',   
            'A','B','C','D','E','F','G','H','J','K','L','M','N','P','Q','R','S','T','U','V','W','X','Y','Z'   
        };

        public static string GenerateRandomNumber(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(58);
            if (rd == null)
            {
                rd = new Random();
            }
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(58)]);
            }
            return newRandom.ToString();
        }
        #endregion
    }
}
