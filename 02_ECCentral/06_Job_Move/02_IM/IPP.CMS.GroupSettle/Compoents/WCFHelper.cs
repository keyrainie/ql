using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.ServiceModel.Channels;
using System.Configuration;
using System.Web.Configuration;
using System.Windows.Forms;

namespace IPP.Oversea.CN.ServiceAdapter
{
    public class WCFHelper
    {
        public static T GetProxy<T>() where T : class
        {
            return CreateChannel<T>();
        }

        private static T CreateChanel<T>(ServiceEndpointEntity entity)
        {
            ChannelFactory<T> factory
                = new ChannelFactory<T>(entity.ChannelBinding, entity.ChannelAddress);
            return factory.CreateChannel();
        }

        private static T CreateChannel<T>() where T : class
        {
            string key = typeof(T).FullName;
            ServiceEndpointEntity fromCache= CacheHelper.GetFromCache<ServiceEndpointEntity>(key);     
      
           
            if (fromCache == null)
            {
                Configuration configuration = null;
                

                configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ServiceModelSectionGroup smsg = (ServiceModelSectionGroup)configuration.GetSectionGroup("system.serviceModel");

                fromCache = GetEndpointEntity<T>(smsg);
                if (fromCache == null)
                {
                    throw new Exception("Invalid type" + typeof(T));
                }

                SetBindingParam(fromCache.ChannelBinding, smsg);
            }

            CacheHelper.AddToCache(key, fromCache);

            return CreateChanel<T>(fromCache);
        }

        private static void SetBindingParam(BasicHttpBinding binding, ServiceModelSectionGroup svcmod)
        {
            foreach (BasicHttpBindingElement el in svcmod.Bindings.BasicHttpBinding.ConfiguredBindings)
            {
                binding.MaxReceivedMessageSize = el.MaxReceivedMessageSize;
                binding.UseDefaultWebProxy = el.UseDefaultWebProxy;
                binding.Security.Mode = el.Security.Mode;
                binding.ReceiveTimeout = el.ReceiveTimeout;
                binding.SendTimeout = el.SendTimeout;
            }
        }

        private static ServiceEndpointEntity GetEndpointEntity<T>(ServiceModelSectionGroup smsg) where T : class
        {
            foreach (ChannelEndpointElement ce in smsg.Client.Endpoints)
            {              
                if (ce.Contract == typeof(T).FullName  )
                {
                    var entity = new ServiceEndpointEntity()
                    {
                        ChannelBinding = new BasicHttpBinding(),
                        ChannelAddress = new EndpointAddress(ce.Address)
                    };
                    return entity;
                }
            }
            return null;
        }
    }

    internal class ServiceEndpointEntity
    {
        public EndpointAddress ChannelAddress
        {
            get;
            set;
        }

        public BasicHttpBinding ChannelBinding
        {
            get;
            set;
        }
    }
}
