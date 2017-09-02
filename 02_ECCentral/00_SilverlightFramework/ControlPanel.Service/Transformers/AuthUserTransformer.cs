using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newegg.Oversea.Silverlight.ControlPanel.Service.BizEntities;
using Newegg.Oversea.Silverlight.ControlPanel.Service.DataContracts;

namespace Newegg.Oversea.Silverlight.ControlPanel.Service.Transformers
{
    public static class AuthUserTransformer
    {
        public static AuthUserMsg ToMessage(this AuthUser entity)
        {
            return new AuthUserMsg
            {
                UniqueName = entity.UniqueName,
                FirstName = entity.FirstName,
                LastName = entity.LastName,
                FullName = entity.FirstName + " " + entity.LastName
            };
        }

        public static List<AuthUserMsg> ToMessage(this List<AuthUser> entities)
        {
            var msg = new List<AuthUserMsg>();

            entities.ForEach(item =>
            {
                msg.Add(item.ToMessage());
            });

            return msg;
        }
    }
}
