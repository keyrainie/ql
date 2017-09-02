using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Xml;
using System.IO;
using System.Windows.Resources;
using System.Text.RegularExpressions;

using Newegg.Oversea.Silverlight.Controls.Containers;
using Newegg.Oversea.Silverlight.ControlPanel.Impl.Resources;
using Newegg.Oversea.Silverlight.Core.Components;
using System.Xml.Linq;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Base;
using System.Net;

namespace Newegg.Oversea.Silverlight.Controls.Components
{
    public class ModuleManager : IModuleManager
    {
        public event EventHandler<LoadProgressEventArgs> LoadProgress;

        private static readonly List<IModuleInfo> m_modules;
        private object m_mutex = new object();

        static ModuleManager()
        {
            m_modules = new List<IModuleInfo>();
        }

        #region IModuleManager Members
        public List<IModuleInfo> GetAllModuleInfo()
        {
            return m_modules;
        }

        public IModuleInfo GetModuleInfoByName(string moduleName)
        {
            moduleName = moduleName.ToLower();

            foreach (IModuleInfo module in m_modules)
            {
                if (module.Name.ToLower() == moduleName)
                {
                    return module;
                }
            }

            return null;
        }

        public IModuleInfo GetModuleInfoByToken(string token)
        {
            token = token.ToLower();

            foreach (IModuleInfo module in m_modules)
            {
                if (module.Token.ToLower() == token)
                {
                    return module;
                }
            }

            return null;
        }

        public void Add(IModuleInfo module)
        {
            if (module != null)
            {
                m_modules.Add(module);
            }
        }

        public void LoadModule(Request request)
        {
            LoadModule(request, null);
        }

        public void LoadModule(Request request, LoadModuleCallback callback)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }

            if (callback != null)
            {
                callback(this, new LoadedMoudleEventArgs(request, null, false, LoadModuleStatus.Begin, null));
            }

            if (request.ModuleInfo.Status == ModuleStatus.Preloading)
            {
                XapModuleLoader loader = new XapModuleLoader();
                loader.LoadCompleted += new EventHandler<LoadCompletedEventArgs>(OnLoadModule);
                loader.LoadProgress += new EventHandler<LoadProgressEventArgs>(OnLoadModuleProgress);
                loader.UserState = callback;
                loader.Load(request);
                lock (request.ModuleInfo)
                {
                    (request.ModuleInfo as ModuleInfo).Status = ModuleStatus.Loading;
                }
            }
            else if (request.ModuleInfo.Status == ModuleStatus.Loading)
            {
                (request.ModuleInfo as ModuleInfo).RequestQueue.Enqueue(new KeyValuePair<Request,LoadModuleCallback>(request,callback));
            }
            else if (request.ModuleInfo.Status == ModuleStatus.Loaded)
            {
                OnLoadModule(this, new LoadCompletedEventArgs(request,null,null,false,callback));
            }
            else if (request.ModuleInfo.Status == ModuleStatus.Initialized)
            {
                (request.ModuleInfo as ModuleInfo).Raise(this, new LoadCompletedEventArgs(request, null, null, false, null));
                if (callback != null)
                {
                    callback(this, new LoadedMoudleEventArgs(request, null, false, LoadModuleStatus.End, null));
                }
            }
        }

        public IModuleInfo CreateModuleInfo(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
            {
                throw new ArgumentNullException("moduleName");
            }

            IModuleInfo module = null;
            lock (m_mutex)
            {
                if ((module = this.GetModuleInfoByName(moduleName)) == null)
                {
                    module = new ModuleInfo(moduleName);
                    this.Add(module);
                }
            }
            return module;
        }

        private void OnLoadModule(object sender, LoadCompletedEventArgs e)
        {
            LoadModuleCallback callback = e.UserState as LoadModuleCallback;

            if (e.Error != null)
            {
                ModuleInfo module = e.Request.ModuleInfo as ModuleInfo;
                lock (module)
                {
                    module.Status = ModuleStatus.Preloading;
                }
                module.Raise(this, e);

                if (callback != null)
                {
                    callback(this, new LoadedMoudleEventArgs(e.Request, e.Error, false, LoadModuleStatus.End, e.UserState));
                }
                else
                {
                    throw new PageException(MessageResource.PageException_PageLoadFailure_Title,
                        MessageResource.PageException_PageLoadFailure_Message, e.Error, e.Request);
                }
            }

            /* 
             * 如果发生页面初始化失败的时候，会导致再次打开这个页面的时候一直处于Loading状态
             * 原因是EntryPointType可能在第一次加载的时候已经初始化，导致第二次不能进行加载；
             * (By Aaron.L.Zhou)
             */
            if (e.Request.ModuleInfo.Status == ModuleStatus.Loading && e.Request.ModuleInfo.EntryPointType == null)
            {
                //是否所有的Xap Dll都已经加载完成;
                bool m_isLoaded = false;
                try
                {
                    int loadedAssemblyCount = 0;

                    Stream stream = e.Result;
                    IEnumerable<AssemblyPart> parts = GetParts(stream, e.Request.ModuleInfo as ModuleInfo);
                    ModuleInfo module = e.Request.ModuleInfo as ModuleInfo;
                    string moduleFile = (module.m_entryPointType.Split(',')[1] + ".dll").ToLower();
                    module.m_stream = new StreamResourceInfo(stream, "xap");
                    LoadModuleConfig(module.m_stream, module.m_config);
                    foreach (AssemblyPart item in parts)
                    {
                        LoadAssembly(stream, item, (a) =>
                            {
                                Interlocked.Increment(ref loadedAssemblyCount);

                                if (item.Source.ToLower() == moduleFile)
                                {
                                    module.m_entryPointType = string.Format("{0},Version={1}",
                                        module.m_entryPointType, Regex.Match(a.FullName, @"Version=(?<version>[\d.]+)").Groups["version"].Value);
                                }
                            
                                if (parts.Count() == loadedAssemblyCount)
                                {
                                    m_isLoaded = true;
                                    loaded(e, callback);
                                }
                            });
                    }
                }
                catch (Exception ex)
                {
                    ModuleInfo module = e.Request.ModuleInfo as ModuleInfo;
                    if (!m_isLoaded)
                    {
                        lock (module)
                        {
                            module.Status = ModuleStatus.Preloading;
                        }
                    }

                    module.Raise(this, new LoadCompletedEventArgs(e.Request, null, ex, e.Cancelled, e.UserState));

                    if (callback != null)
                    {
                        callback(this, new LoadedMoudleEventArgs(e.Request, ex, false, LoadModuleStatus.End, e.UserState));
                    }
                    else
                    {
                        throw new PageException(MessageResource.PageException_ModuleInitializeError_Ttitle,
                            MessageResource.PageException_ModuleInitializeError_Message, ex, e.Request);
                    }
                }
            }



        }

        private void loaded(LoadCompletedEventArgs e, LoadModuleCallback callback)
        {
            lock (e.Request.ModuleInfo)
            {
                (e.Request.ModuleInfo as ModuleInfo).Status = ModuleStatus.Loaded;
            }

            try
            {
                e.Request.ModuleInfo.Initialize();
            }
            catch (Exception ex)
            {
                (e.Request.ModuleInfo as ModuleInfo).Raise(this, new LoadCompletedEventArgs(e.Request, null, ex, e.Cancelled, e.UserState));
                if (callback != null)
                {
                    callback(this, new LoadedMoudleEventArgs(e.Request, ex, false, LoadModuleStatus.End, e.UserState));
                }
                else
                {
                    throw new PageException(MessageResource.PageException_ModuleInitializeError_Ttitle,
                        MessageResource.PageException_ModuleInitializeError_Message, ex, e.Request);
                }
            }

            (e.Request.ModuleInfo as ModuleInfo).Raise(this, e);

            if (callback != null)
            {
                callback(this, new LoadedMoudleEventArgs(e.Request, null, false, LoadModuleStatus.End, e.UserState));
            }
        }

        private void OnLoadModuleProgress(object sender, LoadProgressEventArgs e)
        {
            if (LoadProgress != null)
            {
                LoadProgress(sender, e);
            }
        }

        private static void LoadModuleConfig(StreamResourceInfo sourceStream, Dictionary<string, string> config)
        {
            StreamResourceInfo configStream = Application.GetResourceStream(sourceStream, new Uri(Request.CONST_CONFIGNAME, UriKind.Relative));

            if (configStream != null)
            {
                XDocument doc = XDocument.Load(configStream.Stream);
                var list = from node in doc.Descendants("add")
                           select new { Key = node.Attribute("key").Value, Value = node.Attribute("value").Value };

                foreach (var item in list)
                {
                    config.Add(item.Key, item.Value);
                }
            }
        }



        //private static Assembly LoadAssemblyFromStream(Stream sourceStream, AssemblyPart assemblyPart)
        //{
        //    Stream assemblyStream = Application.GetResourceStream(
        //        new StreamResourceInfo(sourceStream, null),
        //        new Uri(assemblyPart.Source, UriKind.Relative)).Stream;
        //    return assemblyPart.Load(assemblyStream);
        //}

        private static void LoadAssembly(Stream sourceStream, AssemblyPart assemblyPart, Action<Assembly> callback)
        {   
            Assembly a = null;
            try
            {
                Stream assemblyStream = Application.GetResourceStream(
                    new StreamResourceInfo(sourceStream, null),
                    new Uri(assemblyPart.Source, UriKind.Relative)).Stream;
                a = assemblyPart.Load(assemblyStream);

                if (callback != null)
                {
                    callback(a);
                }
            }
            catch (NullReferenceException)
            {
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += (s, e) =>
                {
                    if (e.Error == null)
                    {
                        a = assemblyPart.Load(e.Result);
                    }  
                    
                    if (callback != null)
                    {
                        callback(a);
                    }
                };
                wc.OpenReadAsync(new Uri(assemblyPart.Source, UriKind.RelativeOrAbsolute));
            }
        }


        private static IEnumerable<AssemblyPart> GetParts(Stream stream, ModuleInfo moduleInfo)
        {
            List<AssemblyPart> assemblyParts = new List<AssemblyPart>();
            using (var streamReader = new StreamReader(Application.GetResourceStream(
                                                           new StreamResourceInfo(stream, null),
                                                           new Uri("AppManifest.xaml", UriKind.Relative)).Stream))
            {
                using (XmlReader xmlReader = XmlReader.Create(streamReader))
                {
                    xmlReader.MoveToContent();

                    if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Deployment")
                    {
                        string value = string.Empty,entryPointType = string.Empty;


                        if (string.IsNullOrEmpty(value = xmlReader.GetAttribute("EntryPointType")))
                        {
                            throw new ArgumentNullException("EntryPointType", new Exception("The EntryPointType is not found in AppManifest.xaml!"));
                        }

                        entryPointType = value + ",";

                        if (string.IsNullOrEmpty(value = xmlReader.GetAttribute("EntryPointAssembly")))
                        {
                            throw new ArgumentNullException("EntryPointAssembly", new Exception("The EntryPointAssembly is not found in AppManifest.xaml!"));
                        }

                        entryPointType += value;
                        if (moduleInfo.EntryPointType == null)
                        {
                            moduleInfo.m_entryPointType = entryPointType;
                        }
                    }

                    while (xmlReader.Read())
                    {
                        if (xmlReader.NodeType == XmlNodeType.Element && xmlReader.Name == "Deployment.Parts")
                        {
                            using (XmlReader xmlReaderAssemblyParts = xmlReader.ReadSubtree())
                            {
                                while (xmlReaderAssemblyParts.Read())
                                {
                                    if (xmlReaderAssemblyParts.NodeType == XmlNodeType.Element && xmlReaderAssemblyParts.Name == "AssemblyPart")
                                    {
                                        AssemblyPart assemblyPart = new AssemblyPart();
                                        assemblyPart.Source = xmlReaderAssemblyParts.GetAttribute("Source");
                                        assemblyParts.Add(assemblyPart);
                                    }
                                }
                            }

                            break;
                        }
                    }
                }
            }

            return assemblyParts;
        }

        #endregion

        #region IComponent Members

        public string Name
        {
            get { return "ModuleManager"; }
        }

        public string Version
        {
            get { return "1.0.0.0"; }
        }

        public void InitializeComponent(IPageBrowser browser)
        {
           
        }

        public object GetInstance(System.Windows.Controls.TabItem tab)
        {
            return this;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion

    }

    public class ModuleInfo:IModuleInfo
    {
        private string m_name;
        private string m_token;
        private Assembly m_assmbly;
        internal Dictionary<string, string> m_config;
        internal StreamResourceInfo m_stream;
        private List<IViewInfo> m_views;
        private ModuleStatus m_status;
        private readonly Queue<KeyValuePair<Request, LoadModuleCallback>> m_queue = new Queue<KeyValuePair<Request, LoadModuleCallback>>();
        internal string m_entryPointType;
        internal Queue<KeyValuePair<Request, LoadModuleCallback>> RequestQueue
        {
            get
            {
                return m_queue;
            }
        }
        private object m_mutex = new object();

        public string Name
        {
            get
            {
                return m_name;
            }
        }

        public string Token
        {
            get
            {
                return m_token;
            }
        }

        public Type EntryPointType
        {
            get;
            internal set;
        }

        public Assembly Assembly
        {
            get
            {
                return m_assmbly;
            }

            internal set
            {
                m_assmbly = value;
                GetViews();
            }
        }

        public ICollection Views
        {
            get
            {
                return m_views as ICollection;
            }
        }

        public ModuleStatus Status
        {
            get
            {
                return m_status;
            }

            internal set
            {
                m_status = value;
            }
        }

        public Dictionary<string, string> Configuration
        {
            get
            {
                return m_config;
            }
        }

        public StreamResourceInfo Stream
        {
            get
            {
                return m_stream;
            }
        }

        public ModuleInfo(string name)
        {
            m_name = name;
            m_token = Guid.NewGuid().ToString("N");
            m_views = new List<IViewInfo>();
            m_status = ModuleStatus.Preloading;
            m_config = new Dictionary<string, string>();
        }

        public ModuleInfo(string name, Assembly assembly):this(name)
        {
            m_assmbly = assembly;
            this.Initialize();
        }

        private void GetViews()
        {
            if (m_assmbly != null)
            {
                Type type = typeof(View);
                object[] attributes;
                View attr;
                m_views.Clear();

                foreach (Type item in m_assmbly.GetTypes().Where(t => t.IsClass && !t.IsAbstract).ToList())
                {
                    attributes = item.GetCustomAttributes(type, false);

                    if (attributes != null && attributes.Length > 0)
                    {
                        attr = attributes[0] as View;
                        m_views.Add(new ViewInfo(string.IsNullOrEmpty(attr.ViewName) ? item.Name : attr.ViewName, item, this, attr.IsSingleton, attr.NeedAccess, attr.SingletonType));
                    }
                }
            }
        }

        public IViewInfo GetViewInfoByName(string viewName)
        {
            viewName = viewName.ToLower();

            foreach (IViewInfo view in m_views)
            {
                if (view.ViewName.ToLower() == viewName)
                {
                    return view;
                }
            }

            return null;
        }

        public void Initialize()
        {
            IModule moduleInstance = null;
            try
            {
                if (this.Assembly == null)
                {
                    moduleInstance = CreateModule(this.m_entryPointType);
                    this.EntryPointType = moduleInstance.GetType();
                    moduleInstance.Initialize();
                    this.Assembly = moduleInstance.GetType().Assembly;
                }
                GetViewsFromAssembly();
                lock (m_mutex)
                {
                    this.Status = ModuleStatus.Initialized;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetViewsFromAssembly()
        {
            if (this.Assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            Type type = typeof(View);
            object[] attributes;
            View attr;
            m_views.Clear();

            foreach (Type item in Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract).ToList())
            {
                attributes = item.GetCustomAttributes(type, false);

                if (attributes != null && attributes.Length > 0)
                {
                    attr = attributes[0] as View;
                    m_views.Add(new ViewInfo(string.IsNullOrEmpty(attr.ViewName) ? item.Name : attr.ViewName, item, this, attr.IsSingleton, attr.NeedAccess, attr.SingletonType));
                }
            }
        }

        private static IModule CreateModule(string typeName)
        {
            Type moduleType = Type.GetType(typeName);
            if (moduleType == null)
            {
                throw new Exception(
                    string.Format(@"Unable to retrieve the module type {0} from the loaded assemblies.You may need to specify a more fully-qualified type name.", typeName));
            }

            return Activator.CreateInstance(moduleType) as IModule;
        }

        internal void Raise(object sender,LoadCompletedEventArgs e)
        {
            lock (m_queue)
            {
                KeyValuePair<Request,LoadModuleCallback> pair;
                while (m_queue.Count > 0)
                {
                    pair = m_queue.Dequeue();
                    pair.Value(sender,new LoadedMoudleEventArgs(pair.Key,e.Error,e.Cancelled, LoadModuleStatus.End,e.UserState));
                }
            }
        }
    }

    public class ViewInfo:IViewInfo
    {
        private IModuleInfo m_module;
        private string m_viewName;
        private Type m_viewType;
        private bool m_isSingleton;
        private bool m_needAccess;
        private SingletonTypes m_singletonType;

        public string ViewName
        {
            get
            {
                return m_viewName;
            }
        }

        public Type ViewType
        {
            get
            {
                return m_viewType;
            }
        }

        public IModuleInfo Module
        {
            get
            {
                return m_module;
            }
        }

        public bool IsSingleton
        {
            get
            {
                return m_isSingleton;
            }
        }

        public bool NeedAccess
        {
            get
            {
                return m_needAccess;
            }
        }

        public SingletonTypes SingletonType
        {
            get
            {
                return m_singletonType;
            }
        }

        private ViewInfo()
        { }

        public ViewInfo(string viewName, Type viewType, IModuleInfo module, bool isSingleton, bool needAccess, SingletonTypes type)
        {
            m_viewType = viewType;
            m_viewName = viewName;
            m_module = module;
            m_isSingleton = isSingleton;
            m_needAccess = needAccess;
            m_singletonType = type;
        }

        public object GetViewInstance(PageContext context)
        {
            object view = null;

            try
            {
                if (ViewType.IsSubclassOf(typeof(PageBase)))
                {
                    view = Activator.CreateInstance(ViewType);
                    (view as PageBase).Context = context;
                }
                else
                {
                    view = Activator.CreateInstance(ViewType, new object[] { context });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return view;
        }
    }
}
