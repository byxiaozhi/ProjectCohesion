using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ProjectCohesion.Win32.Controls
{
    /// <summary>
    /// 响应式组件
    /// </summary>
    public class ReactiveControl : UserControl, IDisposable
    {
        private readonly Subject<DependencyPropertyChangedEventArgs> subject = new();

        private readonly CompositeDisposable disposables = new();

        /// <summary>
        /// 属性变化回调
        /// </summary>
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            subject.OnNext(e);
        }

        /// <summary>
        /// 获取一个观察属性变化的 Observable
        /// </summary>
        public IObservable<DependencyPropertyChangedEventArgs> PropertyChangedObservable()
        {
            return subject.AsObservable();
        }

        public void ShouldDispose(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        public void Dispose()
        {
            disposables.Clear();
        }
    }

    public static class ReactiveExtend
    {
        /// <summary>
        /// 获取一个观察指定属性变化的 Observable
        /// </summary>
        public static IObservable<T> WhenPropertyChanged<T>(this T reactiveControl, params string[] properties) where T : ReactiveControl
        {
            return reactiveControl.PropertyChangedObservable().Where(x => properties.Contains(x.Property.Name)).Select(x => reactiveControl);
        }
    }
}
