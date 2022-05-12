using Autofac;
using ProjectCohesion.Core.Models.EventArgs;
using ProjectCohesion.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
    /// 响应式控件
    /// 可以使用 WhenPropertyChanged 监听到该对象的属性变化
    /// </summary>
    public class ReactiveControl : UserControl, IDisposable
    {
        private readonly EventCenter eventCenter = Core.Autofac.Container.Resolve<EventCenter>();

        private readonly CompositeDisposable disposables = new();

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            eventCenter.EmitEvent("PropertyChanged", this, new ReactiveEventArgs(GetType(), e.Property.Name));
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
}