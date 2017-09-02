using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;

namespace IPP.OrderMgmt.Job.Biz
{
    /// <summary>
    /// Look for the job in this factory which you want.
    /// </summary>
    public abstract class AbstractJobFactory
    {
        //Warning 01:if you add or remove the assembly, pls modify the index in time
        private static string currentAssemblyNameDll =
            Assembly.GetExecutingAssembly().GetModules(false)[0].Name;

        private static string currentAssemblyName = currentAssemblyNameDll.Remove(
            currentAssemblyNameDll.Length-4,4);

        /// <summary>
        /// Get the job instance by job name.
        /// The Factory will return null if can't find the job instance instead.
        /// </summary>
        /// <param name="jobName"></param>
        /// <returns></returns>
        public static JobInstance GetJobInstance(string jobName)
        {
            if (string.IsNullOrEmpty(jobName)) return null;
            else
            {
                try
                {
                    var instance = (JobInstance)Assembly.Load
                        (currentAssemblyName).CreateInstance(currentAssemblyName + "." + jobName);
                    return instance ;
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine("Can't find the job, pls check the job name carefully");
                    return null;
                }
            }
        }

        /// <summary>
        /// get the name list of the jobs
        /// </summary>
        /// <returns></returns>
        public static List<string> GetTheAllJobNameFromAssmeble()
        {
            var myModule = Assembly.GetExecutingAssembly().GetModules(false)[0];
            var tArray = myModule.FindTypes(Module.FilterTypeName,"*");
            if (tArray.Count() > 0)
            {
                var jobNames = new List<string>();
                foreach (Type t in tArray)
                {
                    if (t.IsClass
                        && t.Name.Substring(t.Name.Length - 3, 3) == "Job")
                    {
                        jobNames.Add(t.Name);
                    }
                }
                return jobNames;
            }
            return null;
        }
    }
}
