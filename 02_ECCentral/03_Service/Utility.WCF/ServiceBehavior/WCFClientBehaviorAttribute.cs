using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Configuration;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace ECCentral.Service.Utility.WCF.ServiceBehavior
{
    public class WCFClinetBehaviorElement : BehaviorExtensionElement
    {
        public override Type BehaviorType
        {
            get { return typeof(WCFClinetBehavior); }
        }

        protected override object CreateBehavior()
        {
            return new WCFClinetBehavior();
        }
    }

    public class WCFClinetBehavior : Attribute, IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new WCFClientMessageInspector());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }

    public class WCFClientMessageInspector : IClientMessageInspector
    {

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        /// <summary>
        /// 注入上下文：操作用户系统编号
        /// </summary>
        /// <param name="request"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            System.ServiceModel.Channels.MessageHeader header = System.ServiceModel.Channels
                .MessageHeader.CreateHeader("X-User-SysNo", string.Empty, ServiceContext.Current.UserSysNo);
            request.Headers.Add(header);
            return null;
        }
    }
}
