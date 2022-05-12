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
    public class ReactiveObject : INotifyPropertyChanged, IDisposable
    {
        private readonly Subject<PropertyChangedEventArgs> subject = new();

        private readonly CompositeDisposable disposables = new();

        public ReactiveObject()
        {
            PropertyChanged += (o, e) => subject.OnNext(e);
        }

        /// <summary>
        /// 获取一个观察属性变化的 Observable
        /// </summary>
        public IObservable<PropertyChangedEventArgs> PropertyChangedObservable()
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

#pragma warning disable CS0067
        // PropertyChanged.Fody 会自动监听属性变化并发出事件
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
