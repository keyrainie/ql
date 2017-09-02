using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 商品附件信息
    /// </summary>
    [Serializable]
    public class ProductAttachmentInfo : EntityBase
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        public int? SysNo { get; set; }

        /// <summary>
        /// 附件数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the product system no.
        /// </summary>
        /// <value>
        /// The product system no.
        /// </value>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductID { get; set; }

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName { get; set; }

        /// <summary>
        /// Gets or sets the attachment system no.
        /// </summary>
        /// <value>
        /// The attachment system no.
        /// </value>
        public int AttachmentSysNo { get; set; }

        /// <summary>
        /// Gets or sets the attachment identifier.
        /// </summary>
        /// <value>
        /// The attachment identifier.
        /// </value>
        public string AttachmentID { get; set; }

        /// <summary>
        /// Gets or sets the name of the attachment.
        /// </summary>
        /// <value>
        /// The name of the attachment.
        /// </value>
        public string AttachmentName { get; set; }

        /// <summary>
        /// Gets or sets the in user.
        /// </summary>
        /// <value>
        /// The in user.
        /// </value>
        public string InUser { get; set; }

        /// <summary>
        /// Gets or sets the edit user.
        /// </summary>
        /// <value>
        /// The edit user.
        /// </value>
        public string EditUser { get; set; }

        /// <summary>
        /// Gets the in date text.
        /// </summary>
        /// <value>
        /// The in date text.
        /// </value>
        public string InDateText
        {
            get
            {
                return InDate.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }

        /// <summary>
        /// Gets the edit date text.
        /// </summary>
        /// <value>
        /// The edit date text.
        /// </value>
        public string EditDateText
        {
            get
            {
                if (EditDate.HasValue)
                {
                    return EditDate.Value.ToString("yyyy-MM-dd HH:mm:ss");
                }
                return string.Empty;
            }
        }
    }
}
