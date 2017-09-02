using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IPP.InventoryMgmt.JobV31.Common;
using System.Reflection;

namespace IPP.InventoryMgmt.JobV31
{
    public static class Addapter
    {

        public static ICalculateInventoryQty CalculateInventoryQty
        {
            get
            {
                string assemblyFullName = new CommonConst().ICalculateInventoryQtyAssembly;

                string[] arr = assemblyFullName.Split(',');

                string fullName = arr[0];

                string assembly = arr[1];

                object obj = Assembly.Load(assembly).CreateInstance(fullName);
                //object obj = Assembly.GetAssembly(typeof(ICalculateInventoryQty)).CreateInstance(assembly);

                return obj as ICalculateInventoryQty;
            }
        }
    }
}
