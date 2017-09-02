using System.Configuration;

namespace IPP.ContentMgmt.SendQuestionList.Providers
{
    public class ServiceJobInfoCollection : ConfigurationElementCollection
    {
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceJobInfo)element).JobName;
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceJobInfo();
        }

        protected override string ElementName
        {
            get { return "add"; }
        }

        public new ServiceJobInfo this[string jobName]
        {
            get { return (ServiceJobInfo)base.BaseGet(jobName); }
        }

        public ServiceJobInfo this[int index]
        {
            get { return (ServiceJobInfo)base.BaseGet(index); }
        } 
    }
}
