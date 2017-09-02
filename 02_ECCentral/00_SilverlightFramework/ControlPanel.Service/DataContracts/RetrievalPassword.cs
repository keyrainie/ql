using System;
using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts
{
    // Restful
    public class RetrievalPassword
    {
        public string Email { get; set; }

        public string Answer { get; set; }

        public Question Question { get; set; }
    }

    public class RetrievalPasswordResult
    {
        public bool IsSuccess { get; set; }

        public string ResultDescription { get; set; }
    }

    public class Question
    {
        public string ID { get; set; }

        public string Description { get; set; }
    }


}
