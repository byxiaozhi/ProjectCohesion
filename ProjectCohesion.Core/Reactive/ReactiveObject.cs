using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Reactive
{
    public class ReactiveObject : INotifyPropertyChanged
    {
        private readonly Subject<PropertyChangedEventArgs> subject = new();

        public ReactiveObject()
        {
            PropertyChanged += (o, e) => subject.OnNext(e);
        }

        public IObservable<PropertyChangedEventArgs> PropertyChangedObservable()
        {
            return subject.AsObservable();
        }

#pragma warning disable CS0067
        // PropertyChanged.Fody 会自动监听属性变化并发出事件
        public event PropertyChangedEventHandler PropertyChanged;
#pragma warning restore CS0067
    }
}
