using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ProjectCohesion.Core.Reactive
{
    public class ReactiveCommand<T> : ICommand
    {
        public event EventHandler CanExecuteChanged;

        private readonly Subject<T> executeSubject = new();

        private readonly BehaviorSubject<bool> canExecuteSubject;

        public ReactiveCommand() : this(true)
        {
        }

        public ReactiveCommand(bool canExecute)
        {
            canExecuteSubject = new(canExecute);
            canExecuteSubject.DistinctUntilChanged().Subscribe(_ => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        public bool CanExecute(object parameter)
        {
            return canExecuteSubject.Value;
        }

        public void Execute(object parameter)
        {
            executeSubject.OnNext((T)parameter);
        }

        public IObservable<T> OnExecute => executeSubject.AsObservable();

        public IObserver<bool> SetCanExecute => canExecuteSubject.AsObserver();
    }
}
