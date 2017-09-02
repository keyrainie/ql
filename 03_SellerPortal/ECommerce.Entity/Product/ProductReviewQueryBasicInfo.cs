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
    public class ProductReviewQueryBasicInfo
    {
        /// <summary>
        /// 系统编号
        /// </summary>
        /// <value>
        /// The system no.
        /// </value>
        public int SysNo { get; set; }
        /// <summary>
        /// Gets or sets the product system no.
        /// </summary>
        /// <value>
        /// 商品SysNo
        /// </value>
        public int ProductSysNo { get; set; }
        /// <summary>
        /// 商品SysNo
        /// </summary>
        /// <value>
        /// The product identifier.
        /// </value>
        public string ProductID { get; set; }
        /// <summary>
        /// 商品名
        /// </summary>
        /// <value>
        /// The name of the product.
        /// </value>
        public string ProductName { get; set; }
        /// <summary>
        /// Gets or sets the customer system no.
        /// </summary>
        /// <value>
        /// The customer system no.
        /// </value>
        public int CustomerSysNo { get; set; }
        /// <summary>
        /// Gets or sets the name of the vendor.
        /// </summary>
        /// <value>
        /// The name of the vendor.
        /// </value>
        public string VendorName { get; set; }
        /// <summary>
        /// Gets or sets the vendor score.
        /// </summary>
        /// <value>
        /// The vendor score.
        /// </value>
        public string VendorScore { get; set; }
        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>
        /// The title.
        /// </value>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the score.
        /// </summary>
        /// <value>
        /// The score.
        /// </value>
        public decimal Score { get; set; }
        /// <summary>
        /// Gets or sets the most use ful candidate.
        /// </summary>
        /// <value>
        /// The most use ful candidate.
        /// </value>
        public int MostUseFulCandidate { get; set; }
        /// <summary>
        /// Gets or sets the most use ful.
        /// </summary>
        /// <value>
        /// The most use ful.
        /// </value>
        public int MostUseFul { get; set; }
        /// <summary>
        /// Gets or sets the useful count.
        /// </summary>
        /// <value>
        /// The useful count.
        /// </value>
        public int UsefulCount { get; set; }
        /// <summary>
        /// Gets or sets the un useful count.
        /// </summary>
        /// <value>
        /// The un useful count.
        /// </value>
        public int UnUsefulCount { get; set; }
        /// <summary>
        /// Gets or sets the reply count.
        /// </summary>
        /// <value>
        /// The reply count.
        /// </value>
        public int ReplyCount { get; set; }
        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public string Status { get; set; }
        /// <summary>
        /// Gets or sets the is top.
        /// </summary>
        /// <value>
        /// The is top.
        /// </value>
        public string IsTop { get; set; }
        /// <summary>
        /// Gets or sets the is bottom.
        /// </summary>
        /// <value>
        /// The is bottom.
        /// </value>
        public string IsBottom { get; set; }
        /// <summary>
        /// Gets or sets the is digest.
        /// </summary>
        /// <value>
        /// The is digest.
        /// </value>
        public string IsDigest { get; set; }
        /// <summary>
        /// Gets or sets the in date.
        /// </summary>
        /// <value>
        /// The in date.
        /// </value>
        public DateTime InDate { get; set; }
        /// <summary>
        /// Gets or sets the in user.
        /// </summary>
        /// <value>
        /// The in user.
        /// </value>
        public string InUser { get; set; }
        /// <summary>
        /// Gets or sets the edit date.
        /// </summary>
        /// <value>
        /// The edit date.
        /// </value>
        public DateTime? EditDate { get; set; }
        /// <summary>
        /// Gets or sets the edit user.
        /// </summary>
        /// <value>
        /// The edit user.
        /// </value>
        public string EditUser { get; set; }
        /// <summary>
        /// Gets or sets the cs note.
        /// </summary>
        /// <value>
        /// The cs note.
        /// </value>
        public string CSNote { get; set; }
        /// <summary>
        /// Gets or sets the content of the topic mail.
        /// </summary>
        /// <value>
        /// The content of the topic mail.
        /// </value>
        public string TopicMailContent { get; set; }
        /// <summary>
        /// Gets or sets the type of the review.
        /// </summary>
        /// <value>
        /// The type of the review.
        /// </value>
        public int ReviewType { get; set; }
        /// <summary>
        /// Gets or sets the cons.
        /// </summary>
        /// <value>
        /// The cons.
        /// </value>
        public string Cons { get; set; }
        /// <summary>
        /// Gets or sets the prons.
        /// </summary>
        /// <value>
        /// The prons.
        /// </value>
        public string Prons { get; set; }
        /// <summary>
        /// Gets or sets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public string Service { get; set; }
        /// <summary>
        /// Gets or sets the image.
        /// </summary>
        /// <value>
        /// The image.
        /// </value>
        public string Image { get; set; }
        /// <summary>
        /// Gets or sets the score1.
        /// </summary>
        /// <value>
        /// The score1.
        /// </value>
        public int Score1 { get; set; }
        /// <summary>
        /// Gets or sets the score2.
        /// </summary>
        /// <value>
        /// The score2.
        /// </value>
        public int Score2 { get; set; }
        /// <summary>
        /// Gets or sets the score3.
        /// </summary>
        /// <value>
        /// The score3.
        /// </value>
        public int Score3 { get; set; }
        /// <summary>
        /// Gets or sets the score4.
        /// </summary>
        /// <value>
        /// The score4.
        /// </value>
        public int Score4 { get; set; }
        /// <summary>
        /// Gets or sets the complain status.
        /// </summary>
        /// <value>
        /// The complain status.
        /// </value>
        public int ComplainStatus { get; set; }
        /// <summary>
        /// Gets or sets the complain system no.
        /// </summary>
        /// <value>
        /// The complain system no.
        /// </value>
        public int ComplainSysNo { get; set; }
        /// <summary>
        /// Gets or sets the so sysno.
        /// </summary>
        /// <value>
        /// The so sysno.
        /// </value>
        public int SOSysno { get; set; }
        /// <summary>
        /// Gets or sets the is egg review.
        /// </summary>
        /// <value>
        /// The is egg review.
        /// </value>
        public int IsEggReview { get; set; }
        /// <summary>
        /// Gets or sets the is service review.
        /// </summary>
        /// <value>
        /// The is service review.
        /// </value>
        public int IsServiceReview { get; set; }
    }
}
