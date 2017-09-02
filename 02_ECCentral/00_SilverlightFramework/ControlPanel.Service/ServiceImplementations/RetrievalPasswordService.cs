using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;
using Newegg.Oversea.Framework.ExceptionBase;


namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    [RestService]
    public class RetrievalPasswordService : IRetrievalPassword
    {
        public List<Question> GetQuestionList()
        {
            List<Question> list = new List<Question>();
            list.Add(new Question(){ ID="1",Description= "abc"});
            list.Add(new Question(){ID="2", Description="123"});
            return list;
        }

        public RetrievalPasswordResult FindPassword(RetrievalPassword entity)
        {      
            RetrievalPasswordResult result = new RetrievalPasswordResult();
            if (entity.Answer == "123")
            {
                result.IsSuccess = false;
                result.ResultDescription = "Error!!!!";
            }
            else
            {        
                result.IsSuccess = true;
                result.ResultDescription = string.Format("Password retrieval email has been sent to {0} successfully,please check.", entity.Email);
            }
            return result;
        } 
    }
}
