using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ECommerce.Enums;
using ECommerce.Utility;

namespace ECommerce.Entity.Order
{
    public class SOAuthenticationInfo : EntityBase
    {
        /// <summary>
        /// Gets or sets the so system no.
        /// </summary>
        /// <value>
        /// The so system no.
        /// </value>
        public int SOSysNo { get; set; }
        /// <summary>
        /// Gets or sets the customer system no.
        /// </summary>
        /// <value>
        /// The customer system no.
        /// </value>
        public int CustomerSysNo { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the gender.
        /// </summary>
        /// <value>
        /// The gender.
        /// </value>
        public ECustomerGender Gender { get; set; }

        /// <summary>
        /// Gets the gender string.
        /// </summary>
        /// <value>
        /// The gender string.
        /// </value>
        public string GenderString
        {
            get
            {
                return EnumHelper.GetDescription(this.Gender);
            }
        }

        /// <summary>
        /// Gets or sets the type of the identifier card.
        /// </summary>
        /// <value>
        /// The type of the identifier card.
        /// </value>
        public IDCardType IDCardType { get; set; }

        /// <summary>
        /// Gets the identifier card type string.
        /// </summary>
        /// <value>
        /// The identifier card type string.
        /// </value>
        public string IDCardTypeString
        {
            get
            {
                return EnumHelper.GetDescription(this.IDCardType);
            }
        }

        /// <summary>
        /// Gets or sets the identifier card number.
        /// </summary>
        /// <value>
        /// The identifier card number.
        /// </value>
        public string IDCardNumber { get; set; }
        /// <summary>
        /// Gets or sets the birthday.
        /// </summary>
        /// <value>
        /// The birthday.
        /// </value>
        public DateTime Birthday { get; set; }
        /// <summary>
        /// Gets or sets the phone number.
        /// </summary>
        /// <value>
        /// The phone number.
        /// </value>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        /// <value>
        /// The email.
        /// </value>
        public string Email { get; set; }
        /// <summary>
        /// Gets or sets the address.
        /// </summary>
        /// <value>
        /// The address.
        /// </value>
        public string Address { get; set; }
    }
}
