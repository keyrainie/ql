using System;
using System.Windows;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Windows.Resources;

using Newegg.Oversea.Silverlight.Controls;
using Newegg.Oversea.Silverlight.Controls.Components;
using System.ComponentModel;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public enum ModuleStatus
    { 
        Preloading = 0,
        Loading,
        Loaded,
        Initialized
    }
    public enum LoadModuleStatus
    {
       Begin = 0,
       End
    }

    public delegate void LoadModuleCallback(object sender,LoadedMoudleEventArgs e);

    public class LoadedMoudleEventArgs : AsyncCompletedEventArgs
    {
        private Request m_request;
        private LoadModuleStatus m_status;

        public object Param { get; set; }

        public Request Request
        {
            get
            {
                return m_request;
            }
        }

        public LoadModuleStatus Status
        {
            get
            {
                return m_status;
            }
        }

        public LoadedMoudleEventArgs(Request request, Exception error, bool cancelled, LoadModuleStatus status,object userState)
            : base(error, cancelled, userState)
        {
            m_request = request;
            m_status = status;
        }
    }

    public interface IModuleManager:Newegg.Oversea.Silverlight.Controls.Components.IComponent
    {
        List<IModuleInfo> GetAllModuleInfo();
        IModuleInfo GetModuleInfoByName(string moduleName);
        IModuleInfo GetModuleInfoByToken(string token);
        void Add(IModuleInfo module);
        void LoadModule(Request request);
        void LoadModule(Request request, LoadModuleCallback callback);
        IModuleInfo CreateModuleInfo(string moduleName);

        event EventHandler<LoadProgressEventArgs> LoadProgress;
    }

    public interface IModuleInfo
    {
        /// <summary>
        /// 获取Module的名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 获取Module的Token值
        /// </summary>
        string Token { get; }
        /// <summary>
        /// 获取Module的入口点类型
        /// </summary>
        Type EntryPointType { get; }
        /// <summary>
        /// 获取Module的程序集实例
        /// </summary>
        Assembly Assembly { get; }
        /// <summary>
        /// 获取Module中所有页面的实例
        /// </summary>
        ICollection Views { get; }
        /// <summary>
        /// 获取当前Module的状态,加载中还是已经加载完毕
        /// </summary>
        ModuleStatus Status { get; }
        /// <summary>
        /// 获取Module中Client.config中对应的键值对；
        /// </summary>
        Dictionary<string, string> Configuration { get; }
        /// <summary>
        /// 获取Module的xap的stream
        /// </summary>
        StreamResourceInfo Stream { get; }


        /// <summary>
        /// 获取指定页面类名的实例
        /// </summary>
        IViewInfo GetViewInfoByName(string viewName);
        /// <summary>
        /// 初始化ModuleInfo
        /// </summary>
        void Initialize();
    }

    public interface IViewInfo
    { 
        string ViewName { get; }

        Type ViewType   { get; }

        IModuleInfo Module { get; }

        bool IsSingleton { get; }

        bool NeedAccess { get; }

        SingletonTypes SingletonType { get; }

        object GetViewInstance(PageContext context);
    }
}
