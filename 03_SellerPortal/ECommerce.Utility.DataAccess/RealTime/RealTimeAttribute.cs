using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PostSharp.Aspects;

namespace Nesoft.Utility.DataAccess.RealTime
{
    [Serializable]
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class RealTimeAttribute : OnMethodBoundaryAspect
    {
        public BusinessDataType DataType { get; set; }

        public RealTimeAttribute()
        {
        }

        public RealTimeAttribute(BusinessDataType dataType)
        {
            this.DataType = dataType;
        }

        public override void OnEntry(MethodExecutionArgs args)
        {
            
        }

        public override void OnSuccess(MethodExecutionArgs args)
        {                        
            int key = int.Parse(args.Arguments[0].ToString());
            var data = RealTimeHelper.LoadData(key);
            if (data != null)
            {
                args.ReturnValue = data;
            }            
        }
    }
}
