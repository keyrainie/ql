using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ECommerce.SOPipeline.Impl;
using ECommerce.Utility;

namespace ECommerce.SOPipeline
{

    public class FreeShippingConfig
    {
        public List<FreeShippingItemConfig> Rules { get; set; }
        private FreeShippingConfig() { }

        public static FreeShippingConfig GetConfig()
        {
            var config = new FreeShippingConfig();
            config.Rules = PipelineDA.GetFreeShippingConfig();

            return config;
        }
    }

    public class FreeShippingItemConfig
    {
        public int SysNo { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public string AmountSettingType { get; set; }

        public string AmountSettingValue { get; set; }

        public string PayTypeSettingValue { get; set; }

        public string ShipAreaSettingValue { get; set; }

        public List<int> ProductSettingValue { get; set; }

        public bool IsGlobal { get; set; }

        public int SellerSysNo { get; set; }
    }
}