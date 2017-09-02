using System.Collections.Generic;

namespace ECommerce.Facade.Common.RestClient
{
    public class RestServiceError
    {
        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public List<Error> Faults { get; set; }

        public RestServiceError()
        {
            Faults = new List<Error>();
        }
    }

    public class Error
    {
        public string ErrorCode { get; set; }

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
