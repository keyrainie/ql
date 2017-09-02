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
using System.ComponentModel.DataAnnotations;

using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using Newegg.Oversea.Silverlight.CommonDomain.Resources;
using Newegg.Oversea.Silverlight.Utilities.Validation;

namespace Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService
{
    public partial class LogGlobalRegionBody
    {
        [Validate(ValidateType.Required)]
        public string MyGlobalName
        {
            get
            {
                return this.GlobalName;
            }
            set
            {
                this.SetValue("MyGlobalName", ref this.GlobalNameField, value);
            }
        }

        public string MyStatus
        {
            get
            {
                if (this.Status == Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService.Status.Inactive)
                {
                    return "I";
                }
                else
                {
                    return "A";
                }
            }
            set
            {
                if (value == "A")
                {
                    this.Status = Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService.Status.Active;
                }
                else
                {
                    this.Status = Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService.Status.Inactive;
                }
            }
        }

    }

    public partial class LogLocalRegionBody
    {
        [Validate(ValidateType.Required)]
        public string MyLocalName
        {
            get
            {
                return this.LocalName;
            }
            set
            {
                this.SetValue("MyLocalName", ref this.LocalNameField, value);
            }
        }
        public string MyStatus
        {
            get
            {
                if (this.Status == Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService.Status.Inactive)
                {
                    return "I";
                }
                else
                {
                    return "A";
                }
            }
            set
            {
                if (value == "A")
                {
                    this.Status = Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService.Status.Active;
                }
                else
                {
                    this.Status = Newegg.Oversea.Silverlight.CommonDomain.MaintainLogConfigService.Status.Inactive;
                }
            }
        }
    }

    public partial class LogCategoryBody
    {
        private bool m_isInstant;
        private string m_myRemoveOverDays;


        [Validate(ValidateType.Required)]
        public string MyGlobalID
        {
            get
            {
                return this.GlobalID;
            }
            set
            {
                this.SetValue("MyGlobalID", ref this.GlobalIDField, value);
            }
        }

        [Validate(ValidateType.Required)]
        public string MyLocalID
        {
            get
            {
                return this.LocalID;
            }
            set
            {
                this.SetValue("MyLocalID", ref this.LocalIDField, value);
            }
        }

        [Validate(ValidateType.Required)]
        public string MyCategoryName
        {
            get
            {
                return this.CategoryName;
            }
            set
            {
                SetValue("MyCategoryName", ref this.CategoryNameField, value);
            }
        }

        [Validate(ValidateType.Required)]
        public string MySubject
        {
            get
            {
                if (this.EmailNotification == null)
                {
                    return null;
                }
                else
                {
                    return this.EmailNotification.Subject;
                }
            }
            set
            {
                this.ValidateProperty("MySubject", value);
                this.EmailNotification.Subject = value;
            }
        }

        [Validate(ValidateType.Required)]
        public string MyEmailAddress
        {
            get
            {
                if (this.EmailNotification == null)
                {
                    return null;
                }
                else
                {
                    return this.EmailNotification.Address;
                }
            }
            set
            {
                this.ValidateProperty("MyEmailAddress", value);
                this.EmailNotification.Address = value;
            }
        }

        [Validate(ValidateType.Required)]
        [Validate(ValidateType.Regex, "^([1-9]|[1-9][0-9]|[1-2][0-9][0-9]|3[0-5][0-9]|36[0-5])$")]
        public string MyRemoveOverDays
        {
            get
            {
                return this.m_myRemoveOverDays;
            }
            set
            {
                var flag = SetValue("MyRemoveOverDays", ref this.m_myRemoveOverDays, value);

                if (flag && !string.IsNullOrEmpty(value))
                {
                    SetValue("RemoveOverDays", ref this.RemoveOverDaysField, int.Parse(value));
                }
            }
        }

        public string MyLogType
        {
            get
            {
                switch (this.LogType)
                {
                    case LogType.Audit:
                        return "A";
                    case LogType.Debug:
                        return "D";
                    case LogType.Error:
                        return "E";
                    case LogType.Info:
                        return "I";
                    case LogType.Trace:
                        return "T";
                    default:
                        return "E";
                }
            }
            set
            {
                switch (value)
                {
                    case "A":
                        this.LogType = LogType.Audit;
                        break;
                    case "D":
                        this.LogType = LogType.Debug;
                        break;
                    case "E":
                        this.LogType = LogType.Error;
                        break;
                    case "I":
                        this.LogType = LogType.Info;
                        break;
                    case "T":
                        this.LogType = LogType.Trace;
                        break;
                    default:
                        this.LogType = LogType.Error;
                        break;
                }
            }
        }

        public string MyStatus
        {
            get
            {
                if (this.Status == Status.Inactive)
                {
                    return "I";
                }
                else
                {
                    return "A";
                }
            }
            set
            {
                if (value == "I")
                {
                    this.Status = Status.Inactive;
                }
                else
                {
                    this.Status = Status.Active;
                }
            }
        }

        private string m_interval;

        [Validate(ValidateType.Required)]
        [CustomValidation(typeof(MyEntityCustomValidation), "IntervalValidation")]
        public string MyInterval
        {
            get
            {
                return m_interval;
            }
            set
            {
                bool flag = SetValue("MyInterval", ref m_interval, value);

                if (flag)
                {                    
                    int input = int.Parse(value);
                    this.EmailNotification.Interval = input;                    
                }
               
            }
        }

        public bool IsInstant
        {
            get
            {
                return m_isInstant;
            }
            set
            {
                m_isInstant = value;
                RaisePropertyChanged("IsInstant");
            }
        }
    }

    public class MyEntityCustomValidation
    {
        public static ValidationResult IntervalValidation(object value, ValidationContext vc)
        {
            if (value != null && vc != null)
            {
                LogCategoryBody entity = vc.ObjectInstance as LogCategoryBody;

                string myValue = value.ToString();

                ValidationResult vr = new ValidationResult(CommonResource.ValidationMsg_Interval, new string[] { "MyInterval" });

                if (entity.EmailNotification != null && entity.EmailNotification.NeedNotify)
                {
                    try
                    {
                        int input = int.Parse(myValue);

                        if (input >= 1 && input <= 60)
                        {
                            return ValidationResult.Success;
                        }
                        else
                        {
                            return vr;
                        }
                    }
                    catch
                    {
                        return vr;
                    }
                }
            }
            return ValidationResult.Success;
        }
    }

    public class ContractConvert
    {
        public static QueryLogConfigService.Status? ConvertFromMaintainToQuery(MaintainLogConfigService.Status? status)
        {
            switch (status)
            {
                case MaintainLogConfigService.Status.Active:
                    return QueryLogConfigService.Status.Active;
                case MaintainLogConfigService.Status.Inactive:
                    return QueryLogConfigService.Status.Inactive;
                default:
                    return QueryLogConfigService.Status.Active;
            }
        }

        public static MaintainLogConfigService.Status? ConvertFromQueryToMaintain(QueryLogConfigService.Status? status)
        {
            switch (status)
            {
                case QueryLogConfigService.Status.Active:
                    return MaintainLogConfigService.Status.Active;
                case QueryLogConfigService.Status.Inactive:
                    return MaintainLogConfigService.Status.Inactive;
                default:
                    return MaintainLogConfigService.Status.Active;
            }
        }

        public static MaintainLogConfigService.Status ConvertFromQueryToMaintain(QueryLogConfigService.Status status)
        {
            switch (status)
            {
                case QueryLogConfigService.Status.Active:
                    return MaintainLogConfigService.Status.Active;
                case QueryLogConfigService.Status.Inactive:
                    return MaintainLogConfigService.Status.Inactive;
                default:
                    return MaintainLogConfigService.Status.Active;
            }
        }

        public static QueryLogConfigService.Status ConvertFromMaintainToQuery(MaintainLogConfigService.Status status)
        {
            switch (status)
            {
                case MaintainLogConfigService.Status.Active:
                    return QueryLogConfigService.Status.Active;
                case MaintainLogConfigService.Status.Inactive:
                    return QueryLogConfigService.Status.Inactive;
                default:
                    return QueryLogConfigService.Status.Active;
            }
        }

        public static MaintainLogConfigService.LogType ConvertFromQueryToMaintain(QueryLogConfigService.LogType status)
        {
            switch (status)
            {
                case QueryLogConfigService.LogType.Audit:
                    return MaintainLogConfigService.LogType.Audit;
                case QueryLogConfigService.LogType.Debug:
                    return MaintainLogConfigService.LogType.Debug;
                case QueryLogConfigService.LogType.Error:
                    return MaintainLogConfigService.LogType.Error;
                case QueryLogConfigService.LogType.Info:
                    return MaintainLogConfigService.LogType.Info;
                case QueryLogConfigService.LogType.Trace:
                    return MaintainLogConfigService.LogType.Trace;
                default:
                    return MaintainLogConfigService.LogType.Audit;
            }
        }

        public static QueryLogConfigService.LogType ConvertFromMaintainToQuery(MaintainLogConfigService.LogType status)
        {
            switch (status)
            {
                case MaintainLogConfigService.LogType.Audit:
                    return QueryLogConfigService.LogType.Audit;
                case MaintainLogConfigService.LogType.Debug:
                    return QueryLogConfigService.LogType.Debug;
                case MaintainLogConfigService.LogType.Error:
                    return QueryLogConfigService.LogType.Error;
                case MaintainLogConfigService.LogType.Info:
                    return QueryLogConfigService.LogType.Info;
                case MaintainLogConfigService.LogType.Trace:
                    return QueryLogConfigService.LogType.Trace;
                default:
                    return QueryLogConfigService.LogType.Audit;
            }
        }
    }
}