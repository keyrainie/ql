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
using Newegg.Oversea.Silverlight.Controls.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public interface ICache : IComponent
    {
        TimeSpan DefaultCacheTime { get; set; }

        object this[string key] { get; set; }

        void Add(string key, object value);

        void Add(string key, object value, TimeSpan cacheTime);

        void AddAsync(string key, Action<string, Action<object>> getDataAction);

        void AddAsync(string key, TimeSpan cacheTime, Action<string, Action<object>> getDataAction);

        object Get(string key);

        T Get<T>(string key);

        void GetAsync(string key, Action<object> callback);

        void GetAsync<T>(string key, Action<T> callback);

        void Remove(string key);

        void RemoveAll();
    }

}
