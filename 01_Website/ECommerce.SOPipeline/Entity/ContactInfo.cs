using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    /// <summary>
    /// 订单的联系人信息
    /// </summary>
    public class ContactInfo : ExtensibleObject
    {
        public int ID { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系电话
        /// </summary> 
        public string Phone { get; set; }

        /// <summary>
        /// 联系手机
        /// </summary> 
        public string MobilePhone { get; set; }

        /// <summary>
        /// 收货地址标题
        /// </summary>
        public string AddressTitle { get; set; }

        /// <summary>
        /// 联系地址区域ID
        /// </summary>
        public int AddressAreaID { get; set; }

        /// <summary>
        /// 联系地址的详细地址
        /// </summary>
        public string AddressDetail { get; set; }

        /// <summary>
        /// 联系地址的邮编
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// 身份证件类型
        /// </summary>
        public int IDCardType { get; set; }

        /// <summary>
        /// 身份证件号
        /// </summary>
        public string IDCardNo { get; set; }

        public override ExtensibleObject CloneObject()
        {
            return new ContactInfo()
            {
                ID = this.ID,
                Name = this.Name,
                Phone = this.Phone,
                MobilePhone = this.MobilePhone,
                AddressTitle = this.AddressTitle,
                AddressAreaID = this.AddressAreaID,
                AddressDetail = this.AddressDetail,
                ZipCode = this.ZipCode,
                IDCardType = this.IDCardType,
                IDCardNo = this.IDCardNo,
            };
        }
    }
}
