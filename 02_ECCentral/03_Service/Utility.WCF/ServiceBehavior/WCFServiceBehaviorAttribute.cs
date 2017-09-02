using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Configuration;
using System.Configuration;
using System.Collections.ObjectModel;

namespace ECCentral.Service.Utility.WCF.ServiceBehavior
{
    public class WCFBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(WCFBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new WCFBehavior();
        }
    }

    public class WCFBehavior : Attribute, IServiceBehavior
    {
        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher channelDispatcher in serviceHostBase.ChannelDispatchers)
            {
                channelDispatcher.ErrorHandlers.Add(new WCFErrorHandler());
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }
    }

    public class WCFErrorHandler : IErrorHandler
    {
        private Type m_BizExceptionType;

        public WCFErrorHandler()
        {
            if (m_BizExceptionType == null)
            {
                m_BizExceptionType = Type.GetType(ConfigurationManager.AppSettings["BizException"].ToString());
            }
        }

        public bool HandleError(Exception error)
        {
            ExceptionHelper.HandleException(error);
            return true;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            FaultException e;
            if (m_BizExceptionType != null && m_BizExceptionType.IsAssignableFrom(error.GetType()))
            {
                e = new FaultException(error.Message, new FaultCode(error.GetType().FullName));
            }
            else
            {
                e = new FaultException("系统异常，请联系管理员!", new FaultCode(error.GetType().FullName));
            }

            System.ServiceModel.Channels.MessageFault m = e.CreateMessageFault();
            fault = Message.CreateMessage(version, m, e.Action);
        }
    }
}
