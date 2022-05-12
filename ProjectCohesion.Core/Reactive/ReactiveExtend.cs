using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Reactive
{
    public static class ReactiveExtend
    {
        public static IObservable<T> WhenPropertyChanged<T>(this T reactiveObject, params string[] properties) where T : ReactiveObject
        {
            return reactiveObject.PropertyChangedObservable().TakeWhile(x => properties.Contains(x.PropertyName)).Select(x => reactiveObject);
        }
    }
}
