using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Activation;

using Newegg.Oversea.Framework.Contract;
using Newegg.Oversea.Framework.ExceptionBase;
using Newegg.Oversea.Framework.ExceptionHandler;
using Newegg.Oversea.Framework.WCF.Behaviors;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.Resources;
using Newegg.Oversea.Silverlight.ControlPanel.Service.ServiceInterfaces;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service
{
    [ServiceErrorHandling]
    [InternationalBehavior]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, InstanceContextMode = InstanceContextMode.PerCall, AddressFilterMode = AddressFilterMode.Any)]
    public class ConfigurationService : IConfigurationServiceV40
    {
        private string applicationID = CPConfig.Application.Id;
        private CPDataContext m_dataContext = new CPDataContext(OperationType.Action);

        [ErrorHandling]
        public List<ConfigKeyValueMsg> GetApplicationConfig()
        {
#if TRACE
            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
#endif
            if(string.IsNullOrEmpty(applicationID))
            {
                throw new ArgumentNullException("applicationID");
            }

            List<ConfigKeyValueMsg> list = new List<ConfigKeyValueMsg>();

            CPDataContext ctx = new CPDataContext(OperationType.Query);

            var result = from item in ctx.ConfigurationEntities
                         where item.ApplicationId == applicationID
                         orderby item.DomainName ascending
                         select item;

            foreach (ConfigurationEntity item in result)
            {
                list.Add(new ConfigKeyValueMsg()
                {
                    Domain = item.DomainName,
                    Key = item.ConfigKey,
                    Value = item.ConfigValue,
                    ConfigID = item.ConfigId,
                    Description = item.ConfigDescription,
                    EditDate = item.EditDate,
                    EditUser = item.EditUser,
                    InDate = item.InDate,
                    InUser = item.InUser
                });
            }
#if TRACE
            stopwatch.Stop();
            System.Diagnostics.Trace.WriteLine(String.Format("Newegg.Oversea.Silverlight.ControlPanel.Service.GetApplicationConfig() -> Elapsed time: {0} ms", stopwatch.ElapsedMilliseconds));
#endif
            return list;
        }

        [ErrorHandling]
        public ConfigKeyValueV40 CreateConfig(ConfigKeyValueV40 msg)
        {
            var entity = ToEntity(msg);

            var item = (from c in m_dataContext.ConfigurationEntities
                        where string.Compare(c.DomainName, entity.DomainName,true)==0
                        && string.Compare(c.ConfigKey, entity.ConfigKey,true)==0
                        && string.Compare(c.ApplicationId, applicationID, true) == 0
                        select c).SingleOrDefault();

            

            if (item != null)
            {
                throw new BusinessException(InfoMessage.Error_Config_ContainsException);
            }


            entity.InDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);

            this.m_dataContext.ConfigurationEntities.InsertOnSubmit(entity);
            this.m_dataContext.SubmitChanges();
            msg.Body = ToMsg(entity);

            return msg;
        }

        [ErrorHandling]
        public ConfigKeyValueV40 DeleteConfig(ConfigKeyValueV40 msg)
        {

            var entity = ToEntity(msg);

            var original = from config in this.m_dataContext.ConfigurationEntities
                           where config.ConfigId == entity.ConfigId
                           select config;

            this.m_dataContext.ConfigurationEntities.DeleteAllOnSubmit(original);
            this.m_dataContext.SubmitChanges();

            return msg;
        }

        [ErrorHandling]
        public ConfigKeyValueV40 EditConfig(ConfigKeyValueV40 msg)
        {
            var entity = ToEntity(msg);

            var flag = (from c in m_dataContext.ConfigurationEntities
                        where string.Compare(c.DomainName, entity.DomainName, true) == 0
                        && string.Compare(c.ConfigKey, entity.ConfigKey, true) == 0
                        && c.ConfigId != entity.ConfigId
                        && string.Compare(c.ApplicationId, applicationID, true) == 0
                        select c).SingleOrDefault();


            if (flag != null)
            {
                throw new BusinessException(InfoMessage.Error_Config_ContainsException);
            }

            var original = (from config in this.m_dataContext.ConfigurationEntities
                            where config.ConfigId == entity.ConfigId
                            select config).FirstOrDefault();

            entity.EditDate = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Unspecified);
            if (original != null)
            {
                original.DomainName = entity.DomainName;
                original.ConfigKey = entity.ConfigKey;
                original.ConfigValue = entity.ConfigValue;
                original.ConfigDescription = entity.ConfigDescription;
                original.EditUser = entity.EditUser;
                original.EditDate = entity.EditDate;

                this.m_dataContext.SubmitChanges();
                msg.Body = ToMsg(original);
            }

            return msg;
        }


        private ConfigurationEntity ToEntity(ConfigKeyValueV40 msg)
        {
            var body = msg.Body;

            ConfigurationEntity entity = new ConfigurationEntity()
            {
                ApplicationId = applicationID,
                ConfigDescription = body.Description,
                ConfigKey = body.Key,
                ConfigValue = body.Value,
                DomainName = body.Domain,
                InUser = body.InUser,
                InDate = body.InDate,
                EditDate = body.EditDate,
                EditUser = body.EditUser,
                ConfigId = body.ConfigID
            };

            return entity;
        }

        private ConfigKeyValueMsg ToMsg(ConfigurationEntity entity)
        {
            return new ConfigKeyValueMsg
            {
                ConfigID = entity.ConfigId,
                Key = entity.ConfigKey,
                Value = entity.ConfigValue,
                Description = entity.ConfigDescription,
                Domain = entity.DomainName,
                InUser = entity.InUser,
                InDate = entity.InDate,
                EditUser = entity.EditUser,
                EditDate = entity.EditDate
            };
        }

        [ErrorHandling]
        public ECCentralMsg GetECCentralConfig()
        {
            ECCentralMsg msg = new ECCentralMsg();
            msg.ServiceURL = CPConfig.ECCentral.ServiceURL;
            msg.ConfigPrefix = CPConfig.ECCentral.ConfigPrefix;
            return msg;
        }
    }
}