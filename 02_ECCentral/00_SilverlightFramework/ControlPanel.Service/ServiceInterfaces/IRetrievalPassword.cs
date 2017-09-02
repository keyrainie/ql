using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Web;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces
{
    [ServiceContract]
    public interface IRetrievalPassword
    {
        [OperationContract]
        [WebGet(UriTemplate = "Questions")]
        List<Question> GetQuestionList();

        [OperationContract]
        [WebInvoke(UriTemplate = "FindPassword", Method = "POST")]
        RetrievalPasswordResult FindPassword(RetrievalPassword entity);
    }


}