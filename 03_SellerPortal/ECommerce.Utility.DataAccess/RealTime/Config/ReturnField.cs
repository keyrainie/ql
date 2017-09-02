using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nesoft.Utility.DataAccess.RealTime
{
    public class RealTimeMethod
    {
        public string Name { get; set; }
        public string DataType { get; set; }       
        public List<FilterField> FilterFields { get; set; }
        public List<ReturnField> ReturnFields { get; set; }
    }

    public class ReturnField
    {
        public string Name { get; set; }

        public string DBType { get; set; }

        public string ValuePath { get; set; }        
    }

    public class FilterField
    {
        public string Name { get; set; }

        public string DBType { get; set; }

        public string ValuePath { get; set; }        

        public string OperatorType { get; set; }

        public string RelationType { get; set; }
    }
}
