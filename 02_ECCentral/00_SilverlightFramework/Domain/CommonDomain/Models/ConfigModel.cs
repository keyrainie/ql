using Newegg.Oversea.Silverlight.CommonDomain.ConfigurationService;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace Newegg.Oversea.Silverlight.CommonDomain.Models
{
    public class ConfigModel : ConfigKeyValueMsg
    {
        [Validate(ValidateType.Required)]
        public string Domain
        {
            get
            {
                return base.Domain;
            }
            set
            {
                base.Domain = value;
            }
        }

       [Validate(ValidateType.Required)]
        public string Key
        {
            get
            {
                return base.Key;
            }
            set
            {
                base.Key = value;
            }
        }

        [Validate(ValidateType.Required)]
        public string Value
        {
            get
            {
                return base.Value;
            }
            set
            {
                base.Value = value;
            }
        }        
    }
}
