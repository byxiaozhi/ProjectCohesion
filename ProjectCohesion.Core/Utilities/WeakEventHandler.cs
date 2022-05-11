using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Utilities
{
    public class WeakEventHandler<TEventArgs>
    {
        public WeakReference Reference { get; private set; }

        public MethodInfo Method { get; private set; }

        public EventHandler<TEventArgs> Handler { get; private set; }

        public WeakEventHandler(EventHandler<TEventArgs> eventHandler)
        {
            Reference = new WeakReference(eventHandler.Target);
            Method = eventHandler.Method;
            Handler = Invoke;
        }

        public void Invoke(object sender, TEventArgs e)
        {
            if (Reference.IsAlive)
                Method.Invoke(Reference.Target, new object[] { sender, e });
        }

        public static implicit operator EventHandler<TEventArgs>(WeakEventHandler<TEventArgs> weakHandler)
        {
            return weakHandler.Handler;
        }
    }
}
