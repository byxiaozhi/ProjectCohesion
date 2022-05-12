using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace ProjectCohesion.Core.Reactive
{
    public static class ReactiveExtend
    {
        /// <summary>
        /// 获取一个观察指定属性变化的 Observable
        /// </summary>
        public static IObservable<T> WhenPropertyChanged<T>(this T reactiveObject, params string[] properties) where T : ReactiveObject
        {
            return reactiveObject.PropertyChangedObservable().Where(x => properties.Contains(x.PropertyName)).Select(x => reactiveObject);
        }
    }
}
