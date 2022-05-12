using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Services
{
    /// <summary>
    /// 事件中心
    /// </summary>
    public class EventCenter
    {
        public class Event
        {
            public Event(string name, object sender, EventArgs args)
            {
                Name = name;
                Sender = sender;
                EventArgs = args;
            }

            /// <summary>
            /// 事件名称
            /// </summary>
            public string Name { get; }

            /// <summary>
            /// 事件发布者
            /// </summary>
            public object Sender { get; }

            /// <summary>
            /// 事件参数
            /// </summary>
            public EventArgs EventArgs { get; }
        }

        public class Event<TEventArgs> : Event where TEventArgs : EventArgs
        {
            public Event(string name, object sender, TEventArgs args) : base(name, sender, args)
            {
            }

            /// <summary>
            /// 事件参数
            /// </summary>
            public new TEventArgs EventArgs => base.EventArgs as TEventArgs;
        }

        private readonly Subject<Event> subject = new();

        /// <summary>
        /// 转换为 Observable
        /// </summary>
        public IObservable<Event> GetObservable()
        {
            return subject.AsObservable();
        }

        /// <summary>
        /// 转换为 Observable，并且过滤指定类型事件
        /// </summary>
        public IObservable<Event<TEventArgs>> GetObservable<TEventArgs>() where TEventArgs : EventArgs
        {
            return GetObservable().OfType<Event<TEventArgs>>();
        }

        /// <summary>
        /// 转换为 Observable，并且过滤指定名称事件
        /// </summary>
        public IObservable<Event> GetObservable(string name)
        {
            return GetObservable().Where(x => x.Name == name);
        }

        /// <summary>
        /// 转换为 Observable，并且过滤指定名称和类型事件
        /// </summary>
        public IObservable<Event<TEventArgs>> GetObservable<TEventArgs>(string name) where TEventArgs : EventArgs
        {
            return GetObservable<TEventArgs>().Where(x => x.Name == name);
        }

        /// <summary>
        /// 发布事件
        /// </summary>
        public void EmitEvent<TEventArgs>(string name, object sender, TEventArgs args) where TEventArgs : EventArgs
        {
            subject.OnNext(new Event<TEventArgs>(name, sender, args));
        }
    }
}
