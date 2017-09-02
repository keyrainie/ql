using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity.Shipping
{
    public class ShippingContactInfo
    {
        #region [Fields]

        private int sysno;
        private int customerSysno;
        private string name;
        private string contact;
        private string phone;
        private string cellPhone;
        private string fax;
        private int addressAreaID;
        private int addressProvinceID;
        private int addressCityID;
        private string addressArea;
        private string address;
        private string zipCode;
        private bool isDefault;
        private string addressTitle;
        private int? paytypesysno;
        private int? shiptypesysno;
        private int? paymentCategoryID;

        #endregion

        #region [Properties]

        public int SysNo
        {
            get { return this.sysno; }
            set { this.sysno = value; }
        }

        public int CustomerSysNo
        {
            get { return this.customerSysno; }
            set { this.customerSysno = value; }
        }

        /// <summary>
        /// 地址简称
        /// </summary>
        public string AddressTitle
        {
            get { return this.addressTitle; }
            set { this.addressTitle = value; }
        }

        public bool IsDefault
        {
            get { return this.isDefault; }
            set { this.isDefault = value; }
        }

        public string ReceiveName
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// Gets or sets contact
        /// </summary>
        public string ReceiveContact
        {
            get { return this.contact; }
            set { this.contact = value; }
        }

        /// <summary>
        /// Gets or sets phone number
        /// </summary>
        public string ReceivePhone
        {
            get { return this.phone; }
            set { this.phone = value; }
        }

        /// <summary>
        /// Gets or sets cell phone
        /// </summary>
        public string ReceiveCellPhone
        {
            get { return this.cellPhone; }
            set { this.cellPhone = value; }
        }

        /// <summary>
        /// Gets or sets fax
        /// </summary>
        public string ReceiveFax
        {
            get { return this.fax; }
            set { this.fax = value; }
        }

        /// <summary>
        /// Gets or sets address that is exists in system.
        /// </summary>
        public int ReceiveAreaSysNo
        {
            get { return this.addressAreaID; }
            set { this.addressAreaID = value; }
        }


        /// <summary>
        /// Gets or sets address that is exists in system.
        /// </summary>
        public int ReceiveAreaProvinceSysNo
        {
            get { return this.addressProvinceID; }
            set { this.addressProvinceID = value; }
        }


        /// <summary>
        /// Gets or sets address that is exists in system.
        /// </summary>
        public int ReceiveAreaCitySysNo
        {
            get { return this.addressCityID; }
            set { this.addressCityID = value; }
        }

        /// <summary>
        /// Gets or sets address that is exists in system.
        /// </summary>
        public string AddressArea
        {
            get { return this.addressArea; }
            set { this.addressArea = value; }
        }

        /// <summary>
        /// Gets or sets inputed address
        /// </summary>
        public string ReceiveAddress
        {
            get { return this.address; }
            set { this.address = value; }
        }

        /// <summary>
        /// Gets or sets zip code
        /// </summary>
        public string ReceiveZip
        {
            get { return this.zipCode; }
            set { this.zipCode = value; }
        }

        public int? PaymentCategoryID
        {
            get { return this.paymentCategoryID; }
            set { this.paymentCategoryID = value; }
        }

        public int? PayTypeSysNo
        {
            get { return this.paytypesysno; }
            set { this.paytypesysno = value; }
        }

        public int? ShipTypeSysNo
        {
            get { return this.shiptypesysno; }
            set { this.shiptypesysno = value; }
        }

        #endregion
    }
}
