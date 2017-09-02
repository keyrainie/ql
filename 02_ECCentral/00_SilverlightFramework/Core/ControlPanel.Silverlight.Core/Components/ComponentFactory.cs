using System;
using System.Collections.Generic;
using Newegg.Oversea.Silverlight.Controls.Components;
using Newegg.Oversea.Silverlight.ControlPanel.Core.Components;

namespace Newegg.Oversea.Silverlight.Core.Components
{
    public class ComponentFactory
    {
        internal static Dictionary<Type, object> s_list;

        static ComponentFactory()
        {
            object component = null;

            s_list = new Dictionary<Type, object>();

            object authObj = System.Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.KeystoneAuthManager, Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0"));
            s_list.Add(typeof(IAuth), authObj);
            s_list.Add(typeof(ILogin), authObj);

            s_list.Add(typeof(ILog),
                System.Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.Logger,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IConfiguration),
                System.Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.ApplicationConfiguration,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(ICache),
                System.Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.DefaultCacher,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IModuleManager),
                System.Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.ModuleManager,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IXapModuleLoader),
                System.Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.XapModuleLoader,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IUserProfile),
                Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.UserProfileComponent,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IXapVersionController),
                Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.XapVersionController,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IHistory),
                Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.History,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IMail),
                Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.MailComponent,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IEventTracker),
          Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.EventTrackerComponent,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(ICompanyManager),
          Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.CompanyManager,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

            s_list.Add(typeof(IAssemblyLoader),
         Activator.CreateInstance(Type.GetType("Newegg.Oversea.Silverlight.Controls.Components.AssemblyLoader,Newegg.Oversea.Silverlight.ControlPanel.Impl,Version=1.0.0.0")));

        }


        public static T GetComponent<T>() where T : IComponent
        {
            Type type = typeof(T);

            if (s_list.ContainsKey(type))
            {
                return (T)s_list[type];
            }

            throw new Exception("can not find type!");
        }

        public static IComponent GetComponent(string componentName)
        {
            IComponent component = null;
            componentName = componentName.ToLower();

            foreach (KeyValuePair<Type, object> item in s_list)
            {
                if (item.Value != null && (item.Value as IComponent).Name.ToLower() == componentName)
                {
                    component = item.Value as IComponent;
                    break;
                }
            }

            if (component == null)
            {
                throw new Exception("can not find specified component!");
            }

            return component;
        }


        public static ICache Cacher
        {
            get
            {
                return GetComponent<ICache>();
            }
        }

        public static ILog Logger
        {
            get
            {
                return GetComponent<ILog>();
            }
        }

        public static IConfiguration Configuration
        {
            get
            {
                return GetComponent<IConfiguration>();
            }
        }
    }

}
