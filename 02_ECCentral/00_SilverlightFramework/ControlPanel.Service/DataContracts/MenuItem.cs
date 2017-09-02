using System;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    // Restful
    public class MenuItem
    {
        private DateTime? m_InDate;
        private DateTime? m_EditDate;


        public Guid? MenuId { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string IconStyle { get; set; }

        public string LinkPath { get; set; }

        public string Type { get; set; }

        public string AuthKey { get; set; }

        public bool? IsDisplay { get; set; }

        public string Status { get; set; }

        public Guid? ParentMenuId { get; set; }

        public int? SortIndex { get; set; }

        public string ApplicationId { get; set; }

        public string ApplicationName { get; set; }

        public string LanguageCode { get; set; }

        public string InUser { get; set; }

        public string EditUser { get; set; }

        public DateTime? InDate 
        {
            get
            {
                if (this.m_InDate.HasValue)
                {
                    return DateTime.SpecifyKind(this.m_InDate.Value, DateTimeKind.Utc);
                }
                return null;
            }
            set
            {
                this.m_InDate = value;
            }
        }

        public DateTime? EditDate
        {
            get
            {
                if (this.m_EditDate.HasValue)
                {
                    return DateTime.SpecifyKind(this.m_EditDate.Value, DateTimeKind.Utc);
                }
                return null;
            }
            set
            {
                this.m_EditDate = value;
            }
        }

        public List<LocalizedRes> LocalizedResCollection { get; set; }
    }

    public class LocalizedRes
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string LanguageCode { get; set; }
    }

}
