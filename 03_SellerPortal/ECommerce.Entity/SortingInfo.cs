using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.Entity
{
    public class SortingInfo
    {
        private string _sortField;
        private SortOrder _sortOrder;

        public string SortField
        {
            get { return _sortField; }
            set { _sortField = value; }
        }

        public SortOrder SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }
    }

    public enum SortOrder
    {
        Descending = 0,
        Ascending = 1,
    }
}
