using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace SendMKTPointEmail
{
    // Define a custom section containing 
    // a simple element and a collection of 
    // the same element. It uses two custom 
    // types: UrlsCollection and 
    // UrlsConfigElement.
    public class AccountPointNoticeMailEntitySection :
        ConfigurationSection
    {
        [ConfigurationProperty("items",
            IsDefaultCollection = false)]
        public AccountPointNoticeMailEntityCollection Items
        {
            get
            {
                AccountPointNoticeMailEntityCollection itemList = (AccountPointNoticeMailEntityCollection)base["items"];

                return itemList;
            }
        }
    }
}