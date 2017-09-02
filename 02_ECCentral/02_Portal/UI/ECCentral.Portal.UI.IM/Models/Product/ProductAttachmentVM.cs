using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using ECCentral.BizEntity.Common;
using ECCentral.BizEntity.IM;
using ECCentral.Portal.UI.IM.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;

namespace ECCentral.Portal.UI.IM.Models
{
    public class ProductAttachmentVM : ModelBase
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 商品SysNo
        /// </summary>
        public int ProductSysNo { get; set; }

        private string _productName;
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName
        {
            get { return _productName; }
            set
            {
                _productName = value;
                _productName = RemoveHtml(_productName);
            }
        }

        private string _attachmentID;
        /// <summary>
        /// 附件ID
        /// </summary>
        public string AttachmentID
        {
            get
            {
                return _attachmentID;
            }
            set
            {
                _attachmentID = value.Replace("<br/>", "\r\n");
            }
        }

        private string _attachmentName;
        /// <summary>
        /// 附件名称
        /// </summary>
        public string AttachmentName
        {
            get
            {
                return _attachmentName;
            }
            set
            {
                _attachmentName = value.Replace("<br/>", "\r\n");
                _attachmentName = RemoveHtml(_attachmentName);
            }
        }
        /// <summary>
        /// 创建人/修改人
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// 创建时间/修改时间
        /// </summary>
        public string OperationTime { get; set; }

        /// <summary>
        /// 创建人/修改人
        /// </summary>
        public string InUser { get; set; }

        /// <summary>
        /// 创建时间/修改时间
        /// </summary>
        public string InDate { get; set; }


        public string Status { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public ProductStatus ProductStatus { get; set; }

        private string RemoveHtml(string src)
        {
            src = src ?? "";
            var htmlReg = new Regex(@"<[^>]+>", RegexOptions.IgnoreCase);
            var htmlSpaceReg = new Regex("\\ \\;", RegexOptions.IgnoreCase);
            var styleReg = new Regex(@"<style(.*?)</style>", RegexOptions.IgnoreCase);
            var scriptReg = new Regex(@"<script(.*?)</script>", RegexOptions.IgnoreCase);
            src = styleReg.Replace(src, string.Empty);
            src = scriptReg.Replace(src, string.Empty);
            src = htmlReg.Replace(src, string.Empty);
            src = htmlSpaceReg.Replace(src, " ");
            return src.Trim();
        }
    }

    public class ProductAttachmentDetailsListVM : ModelBase
    {
        public ProductAttachmentDetailsListVM()
        {
            ProductAttachmentList = new List<ProductAttachmentDetailsVM>();
        }

        /// <summary>
        /// 商品编号
        /// </summary>
        private int? _productSysNo;
        public int? ProductSysNo
        {
            get { return _productSysNo; }
            set { SetValue("ProductSysNo", ref _productSysNo, value); }
        }

        /// <summary>
        /// 附件组
        /// </summary>
        public List<ProductAttachmentDetailsVM> ProductAttachmentList { get; set; }
    }

    public class ProductAttachmentDetailsVM : ModelBase
    {
        private int? _productAttachmentSysNo;
        /// <summary>
        /// 附件商品信息
        /// </summary>
        [UintValidation(ErrorMessageResourceType = typeof(ResProductAttachmentMaintain), ErrorMessageResourceName = "ProductAttachmentSysNoInvalid")]
        public int? ProductAttachmentSysNo
        {
            get { return _productAttachmentSysNo; }
            set { SetValue("ProductAttachmentSysNo", ref _productAttachmentSysNo, value); }
        }

        private string _quantity;
        /// <summary>
        /// 附件数量
        /// </summary>
        [IntRangeCustomValidation(1, 3, ErrorMessageResourceType = typeof(ResProductAttachmentMaintain), ErrorMessageResourceName = "QuantityInvalid")]
        public string AttachmentQuantity
        {
            get { return _quantity; }
            set { SetValue("AttachmentQuantity", ref _quantity, value); }
        }

        /// <summary>
        /// 创建人
        /// </summary>
        public UserInfo InUser { get; set; }

        /// <summary>
        /// 更新人
        /// </summary>
        public UserInfo EditUser { get; set; }

        /// <summary>
        /// 更新日期
        /// </summary>
        public DateTime? EditDate { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? InDate { get; set; }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ProductAttachmentDetailsVM)
            {
                var o = obj as ProductAttachmentDetailsVM;
                if (o.ProductAttachmentSysNo != null)
                {
                    if (o.ProductAttachmentSysNo == ProductAttachmentSysNo)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int? _productSysNo;
        /// <summary>
        /// 商品编号
        /// </summary>
        [UintValidation(ErrorMessageResourceType = typeof(ResProductAttachmentMaintain), ErrorMessageResourceName = "ProductSysNoInvalid")]
        public int? ProductSysNo
        {

            get { return _productSysNo; }
            set { SetValue("ProductSysNo", ref _productSysNo, value); }
        }

        /// <summary>
        /// 附件名称
        /// </summary>
        public string AttachmentProductName { get; set; }

        /// <summary>
        /// 附件ID
        /// </summary>
        public string AttachmentProductID { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public AttachmentOperator Operator { get; set; } 

    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum AttachmentOperator
    {
        Add,
        Update
    }
}
