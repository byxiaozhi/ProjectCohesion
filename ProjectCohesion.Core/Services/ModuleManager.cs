using Autofac.Core;
using ProjectCohesion.Core.Models;
using ProjectCohesion.Core.Models.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCohesion.Core.Services
{
    /// <summary>
    /// 组件管理服务
    /// </summary>
    public class ModuleManager
    {
        /// <summary>
        /// 注册组件事件
        /// </summary>
        public event EventHandler<ModuleEventArgs> Registered;

        /// <summary>
        /// 移除组件事件
        /// </summary>
        public event EventHandler<ModuleEventArgs> Removed;

        /// <summary>
        /// Key为组件ID，Value为组件
        /// </summary>
        private readonly Dictionary<Guid, object> moduleDictionary = new();

        /// <summary>
        /// 注册一个组件
        /// </summary>
        public void RegisterModule(Guid guid, object module)
        {
            if (moduleDictionary.ContainsKey(guid))
                RemoveModule(guid);
            moduleDictionary.Add(guid, module);
            Registered?.Invoke(this, new ModuleEventArgs() { Guid = guid, Module = module });
        }

        /// <summary>
        /// 通过关键字组件
        /// </summary>
        public object GetModule(Guid guid)
        {
            return moduleDictionary[guid];
        }

        /// <summary>
        /// 通过类型获取组件
        /// </summary>
        public T GetModule<T>()
        {
            return (T)moduleDictionary.Values.Where(x => x is T).First();
        }

        /// <summary>
        /// 通过类型获取组件列表
        /// </summary>
        public List<T> GetModules<T>()
        {
            return moduleDictionary.Values.Where(x => x is T).Select(x => (T)x).ToList();
        }

        /// <summary>
        /// 移除组件
        /// </summary>
        public void RemoveModule(Guid guid)
        {
            if (!moduleDictionary.ContainsKey(guid))
                return;
            var module = moduleDictionary[guid];
            moduleDictionary.Remove(guid);
            Removed?.Invoke(this, new ModuleEventArgs() { Guid = guid, Module = module });
        }
    }
}
