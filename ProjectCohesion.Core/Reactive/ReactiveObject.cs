using Autofac;
using ProjectCohesion.Core.Models.EventArgs;
using ProjectCohesion.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Reactive
{
    /// <summary>
    /// 响应式对象
    /// 可以使用 WhenPropertyChanged 监听到该对象的属性变化
    /// </summary>
    public class ReactiveObject : INotifyPropertyChanged, IDisposable
    {

        private readonly CompositeDisposable disposables = new();

        private readonly EventCenter eventCenter = Autofac.Container.Resolve<EventCenter>();

        public ReactiveObject()
        {
            PropertyChanged += PropertyChangedHandler;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            eventCenter.EmitEvent("PropertyChanged", this, new ReactiveEventArgs(GetType(), e.PropertyName, GetValue(e.PropertyName)));
        }

        public void ShouldDispose(IDisposable disposable)
        {
            disposables.Add(disposable);
        }

        public void Dispose()
        {
            disposables.Clear();
        }

        public object GetValue(string propertyName)
        {
            return GetType().GetProperty(propertyName).GetValue(this);
        }

        public void SetValue(string propertyName, object value)
        {
            GetType().GetProperty(propertyName).SetValue(this, value);
        }

#pragma warning disable CS0067
        // PropertyChanged.Fody 会自动监听属性变化并发出事件
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
