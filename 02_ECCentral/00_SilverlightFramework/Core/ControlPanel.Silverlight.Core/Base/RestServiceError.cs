using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Collections.ObjectModel;

namespace Newegg.Oversea.Silverlight.ControlPanel.Core.Base
{
    [DataContract(Name = "ServiceError", Namespace = "")]
    public class RestServiceError
    {
        [DataMember]
        public int StatusCode { get; set; }

        [DataMember]
        public string StatusDescription { get; set; }

        [DataMember]
        public ObservableCollection<Error> Faults { get; set; }

        public RestServiceError()
        {
            Faults = new ObservableCollection<Error>();
        }
    }

    [DataContract(Name = "Error", Namespace = "")]
    public class Error
    {
        [DataMember]
        public string ErrorCode { get; set; }

        [DataMember]
        public string ErrorDescription { get; set; }

        public bool IsBusinessException
        {
            get
            {
                return this.ErrorCode != "00000";
            }
        }
    }
}
