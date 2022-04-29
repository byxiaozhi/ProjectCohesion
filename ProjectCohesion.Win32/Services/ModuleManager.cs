using ProjectCohesion.Core.Models.UIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectCohesion.Win32.Services
{
    /// <summary>
    /// UI组件管理服务
    /// </summary>
    public class ModuleManager
    {
        /// <summary>
        /// Key为组件ID，Value为组件
        /// </summary>
        private readonly Dictionary<string, UIModule> uiModuleDictionary = new();

        /// <summary>
        /// 添加一个UI模块
        /// </summary>
        public void RegisterUIModule(string key, UIModule module)
        {
            if (uiModuleDictionary.ContainsKey(key))
                throw new Exception("模块关键字冲突");
            uiModuleDictionary.Add(key, module);
        }

        /// <summary>
        /// 通过关键字获取UI模块
        /// </summary>
        public UIModule GetUIModule(string key)
        {
            return uiModuleDictionary[key];
        }

        /// <summary>
        /// 通过类型获取UI模块
        /// </summary>
        public T GetUIModule<T>() where T : UIModule
        {
            return (T)uiModuleDictionary.Values.Where(x => x is T).First();
        }

        /// <summary>
        /// 通过类型获取UI模块列表
        /// </summary>
        public List<T> GetUIModuleList<T>() where T : UIModule
        {
            return uiModuleDictionary.Values.Where(x => x is T).Select(x => (T)x).ToList();
        }

        /// <summary>
        /// 移除UI模块
        /// </summary>
        public void RemoveUIModule(string key)
        {
            uiModuleDictionary.Remove(key);
        }
    }
}
