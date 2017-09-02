using System;

using System.Collections.Generic;

namespace Newegg.Oversea.Silverlight.ControlPanel.SilverlightUI.UserControls.Menu
{
    public class ItemData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public bool IsPage { get; set; }

        public List<ItemData> SubItems { get; set; }

        public bool IsFavorited { get; set; }

        public override bool Equals(object obj)
        {
            var param = obj as ItemData;
            if (param != null)
            {
                return this.Id == param.Id;
            }
            return base.Equals(obj);
        }
    }
}
