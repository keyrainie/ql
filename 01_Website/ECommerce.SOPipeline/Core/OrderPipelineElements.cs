using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECommerce.SOPipeline
{
    internal class OrderPipelineElements
    {
        public List<IValidate> PreValidators { get; set; }

        public List<ICalculate> Calculators { get; set; }

        public List<IValidate> PostValidators { get; set; }

        public List<IPersist> Persisters { get; set; }

        public List<IInitialize> Initializers { get; set; }

        public bool TransactionWithPersisters { get; set; }

        public bool BreakOnceValidationError { get; set; }
    }
}
