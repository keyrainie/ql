using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECCentral.Service.Utility;
using System.Xml.Serialization;

namespace ECCentral.Service.EventMessage.MKT
{

    [Serializable]
    public class ProductConsultCreatedMessage : ECCentral.Service.Utility.EventMessage
    {

        public override string Subject
        {
            get
            {
                return "ECC_ProductConsult_Created";
            }
        }

        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductSysNo { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }


    }

    [Serializable]
    public class ProductConsultReplyCreatedMessage : ECCentral.Service.Utility.EventMessage
    {

        public override string Subject
        {
            get
            {
                return "ECC_ProductConsultReply_Created";
            }
        }

        /// <summary>
        /// 编号
        /// </summary>
        public int SysNo { get; set; }

        /// <summary>
        /// 咨询编号
        /// </summary>
        public int ConsultSysNo { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>
        public int CustomerSysNo { get; set; }

        public int CurrentUserSysNo { get; set; }

    }
}