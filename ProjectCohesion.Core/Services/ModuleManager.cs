using Autofac.Core;
using ProjectCohesion.Core.Models;
using ProjectCohesion.Core.Models.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
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
        private readonly Subject<ModuleEventArgs> registeredSubject = new();
        public IObservable<ModuleEventArgs> Registered => registeredSubject.AsObservable();

        /// <summary>
        /// 移除组件事件
        /// </summary>
        private readonly Subject<ModuleEventArgs> removedSubject = new();
        public IObservable<ModuleEventArgs> Removed => removedSubject.AsObservable();

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
            registeredSubject.OnNext(new ModuleEventArgs() { Guid = guid, Module = module });
        }

        /// <summary>
        /// 注册一个组件
        /// </summary>
        public Guid RegisterModule(object module)
        {
            Guid guid = Guid.NewGuid();
            RegisterModule(guid, module);
            return guid;
        }

        /// <summary>
        /// 通过关键字组件
        /// </summary>
        public object GetModule(Guid guid)
        {
            if (moduleDictionary.ContainsKey(guid))
                return moduleDictionary[guid];
            return null;
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
            removedSubject.OnNext(new ModuleEventArgs() { Guid = guid, Module = module });
        }
    }
}
