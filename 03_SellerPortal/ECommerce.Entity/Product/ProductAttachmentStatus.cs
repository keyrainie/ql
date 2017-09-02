using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Product
{
    [Serializable]
    public class ProductAttachmentStatus
    {
        /// <summary>
        /// Gets or sets the product system no.
        /// </summary>
        /// <value>
        /// The product system no.
        /// </value>
        public int? ProductSysNo { get; set; }

        /// <summary>
        /// Gets or sets the product identifier.
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductID { get; set; }

        /// <summary>
        /// Gets or sets the product status.
        /// </summary>
        /// <value>
        /// The product status.
        /// </value>
        public int? ProductStatus { get; set; }

        /// <summary>
        /// Gets or sets the attachment count.
        /// </summary>
        /// <value>
        /// The attachment count.
        /// </value>
        public int? AttachmentCount { get; set; }
    }
}
