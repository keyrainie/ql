using System;
using System.Collections.Generic;

namespace ECCentral.Portal.UI.Invoice.Utility
{
    public interface IStatisticInfo
    {
        string ToStatisticText();
    }

    public class StatisticCollection<T> : List<T>, IStatisticInfo
        where T : IStatisticInfo
    {
        #region IStatisticInfo Members

        public string ToStatisticText()
        {
            System.Text.StringBuilder msg = new System.Text.StringBuilder();
            if (this != null && this.Count > 0)
            {
                foreach (var s in this)
                {
                    msg.AppendLine(s.ToStatisticText());
                }
            }
            return msg.ToString();
        }

        #endregion IStatisticInfo Members
    }
}