using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ProductAttachmentQueryBasicInfo
    {
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
        /// Gets or sets the operator.
        /// </summary>
        /// <value>
        /// The operator.
        /// </value>
        public string Operator { get; set; }
        /// <summary>
        /// Gets or sets the operation time.
        /// </summary>
        /// <value>
        /// The operation time.
        /// </value>
        public string OperationTime { get; set; }
        /// <summary>
        /// Gets or sets the in date.
        /// </summary>
        /// <value>
        /// The in date.
        /// </value>
        public string InDate { get; set; }
        /// <summary>
        /// Gets or sets the in user.
        /// </summary>
        /// <value>
        /// The in user.
        /// </value>
        public string InUser { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the product status.
        /// </summary>
        /// <value>
        /// The product status.
        /// </value>
        public int ProductStatus { get; set; }
    }
}
