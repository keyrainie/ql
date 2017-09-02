using System;
using System.Collections;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public interface IHistory: IComponent
    {
        Request Current { get; }
        IList ClosedTabs { get; }
        IList VisitedTabs { get; }
        IList VisitedTabViews { get; }
        void Previous();
        void Next();
        void Clear();
        void RemoveRecoveriedTab();
    }

    public class TabView
    {
        public string Header { get; internal set; }

        public string Url { get; internal set; }

        public Request Request { get; set; }
    }
}
