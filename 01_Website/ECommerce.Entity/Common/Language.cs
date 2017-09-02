using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    public class Language
    {
        public string LanguageCode { get; set; }

        public string LanguageName { get; set; }

        public int IsDefault { get; set; }
    }
}
