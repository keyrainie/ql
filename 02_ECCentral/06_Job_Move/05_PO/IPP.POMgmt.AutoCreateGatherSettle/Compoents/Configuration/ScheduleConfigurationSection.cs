using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace POASNMgmt.AutoCreateVendorSettle.Compoents.Configuration
{
    public class ScheduleConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("monthly", IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(ScheduleElementCollection),
            AddItemName = "add",
            ClearItemsName = "clear",
            RemoveItemName = "remove")]
        public ScheduleElementCollection MonthlySchedule
        {
            get
            {
                ScheduleElementCollection schedule =
                    (ScheduleElementCollection)base["monthly"];

                return schedule;
            }
        }

        /*
         * Weekly and Daily ara not supported in this version
         */

        //[ConfigurationProperty("weekly", IsDefaultCollection = false)]
        //[ConfigurationCollection(typeof(KeyValueConfigurationCollection),
        //    AddItemName = "add",
        //    ClearItemsName = "clear",
        //    RemoveItemName = "remove")]
        //public KeyValueConfigurationCollection WeeklySchedule
        //{
        //    get
        //    {
        //        KeyValueConfigurationCollection schedule =
        //            (KeyValueConfigurationCollection)base["weekly"];

        //        return schedule;
        //    }
        //}

        //[ConfigurationProperty("daily", IsDefaultCollection = false)]
        //[ConfigurationCollection(typeof(KeyValueConfigurationCollection),
        //    AddItemName = "add",
        //    ClearItemsName = "clear",
        //    RemoveItemName = "remove")]
        //public KeyValueConfigurationCollection DailySchedule
        //{
        //    get
        //    {
        //        KeyValueConfigurationCollection schedule =
        //            (KeyValueConfigurationCollection)base["daily"];

        //        return schedule;
        //    }
        //}
    }

    public class ScheduleElement : ConfigurationElement
    {
        [ConfigurationProperty("consignToAccType",IsRequired = true, IsKey = true)]
        public int ConsignToAccType
        {
            get
            {
                return (int)this["consignToAccType"];
            }
        }

        [ConfigurationProperty("dayOfMonth", IsRequired = true)]
        public string DayOfMonth
        {
            get
            {
                return (string)this["dayOfMonth"];
            }
        }
    }

    public class ScheduleElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ScheduleElement();
        }

        protected override Object GetElementKey(ConfigurationElement element)
        {
            return ((ScheduleElement)element).ConsignToAccType;
        }
    }
}
