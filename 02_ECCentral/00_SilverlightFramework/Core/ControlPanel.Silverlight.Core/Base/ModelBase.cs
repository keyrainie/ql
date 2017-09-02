using System;
using System.Linq;
using System.ComponentModel;
using System.ServiceModel.DomainServices.Client;
using System.ComponentModel.DataAnnotations;

namespace Newegg.Oversea.Silverlight.ControlPanel.Core.Base
{
    //[System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "3.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name = "ModelBase", Namespace = "http://oversea.newegg.com/SilverlightFramework/ControlPanel/Contract")]
    public class ModelBase : ComplexObject//Entity
    {
        protected bool SetValue(string propertyName, object value, Action setValueAction)
        {
            this.RaiseDataMemberChanging(propertyName);

            ComplexObject newValue = value as ComplexObject;
            if (newValue == null)
            {
                this.ValidateProperty(propertyName, value);
            }

            setValueAction();

            RaiseDataMemberChanged(propertyName);

            var error = this.ValidationErrors.FirstOrDefault(p => p.MemberNames.Contains(propertyName));
            if (error != null)
            {
                return false;
            }

            return true;
        }

        protected bool SetValue<TPropertyType>(string propertyName, ref TPropertyType target, TPropertyType value)
        {
            this.RaiseDataMemberChanging(propertyName);

            ComplexObject newValue = value as ComplexObject;
            if (newValue == null)
            {
                this.ValidateProperty(propertyName, value);
            }

            TypeCode typeCode = System.Type.GetTypeCode(typeof(TPropertyType));
            if (typeCode == TypeCode.String)
            {
                object str = null;
                if (value != null)
                {
                    str = value.ToString().Trim();
                    target = (TPropertyType)str;
                    value = (TPropertyType)str;
                }
                else
                {
                    target = value;
                }
            }
            else
            {
                target = value;
            }

            RaiseDataMemberChanged(propertyName);

            var error = this.ValidationErrors.FirstOrDefault(p => p.MemberNames.Contains(propertyName));
            if (error != null)
            {
                return false;
            }                       
            
            return true;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(e.PropertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public virtual object Clone()
        {
            return Clone(null);
        }

        public virtual T Clone<T>()
        {
            throw new NotImplementedException();
        }

        public virtual void SetFields(ModelBase model)
        {
            throw new NotImplementedException();
        }

        public virtual T ConvertTo<T>() where T : ModelBase, new()
        {
            throw new NotImplementedException();
        }

        protected virtual object Clone(object o)
        {
            throw new NotImplementedException();
        }
    }   
}
