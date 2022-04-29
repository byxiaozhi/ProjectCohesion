using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Services
{
    /// <summary>
    /// 事件中心
    /// </summary>
    public class EventCenter
    {
        private readonly Dictionary<string, HashSet<EventHandler>> eventHandlers = new();

        /// <summary>
        /// 添加事件监听器
        /// </summary>
        public Action AddEventListener(string name, EventHandler handler)
        {
            if (!eventHandlers.ContainsKey(name))
                eventHandlers.Add(name, new HashSet<EventHandler>());
            eventHandlers[name].Add(handler);
            return () => RemoveEventListener(name, handler);
        }

        /// <summary>
        /// 移除事件监听器
        /// </summary>
        public void RemoveEventListener(string name, EventHandler handler)
        {
            if (!eventHandlers.ContainsKey(name))
                return;
            eventHandlers[name].Remove(handler);
            if (!eventHandlers[name].Any())
                eventHandlers.Remove(name);
        }

        /// <summary>
        /// 移除所有事件监听器
        /// </summary>
        public void RemoveAllEventListener(string name)
        {
            eventHandlers.Remove(name);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void EmitEvent(string name, object sender, EventArgs args)
        {
            if (!eventHandlers.ContainsKey(name))
                return;
            foreach (EventHandler handler in eventHandlers[name])
                handler.Invoke(sender, args);
        }
    }
}
