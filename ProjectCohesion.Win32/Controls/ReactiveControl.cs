using Autofac;
using ProjectCohesion.Core.Models.EventArgs;
using ProjectCohesion.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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

        public static DependencyProperty RegisterProperty(string name, Type ownerType, PropertyMetadata propertyMetadata = null)
        {
            var propertyType = ownerType.GetProperty(name).PropertyType;
            return DependencyProperty.Register(name, propertyType, ownerType, propertyMetadata);
        }
    }

    public static class ReactiveExtension
    {
        /// <summary>
        /// 获取表达式依赖
        /// TODO: 对更复杂的表达式进行依赖分析
        /// </summary>
        public static (object, string) GetExpressionDependency<T, TResult>(this T obj, Expression<Func<T, TResult>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
            {
                var lamdba = System.Linq.Expressions.Expression.Lambda(memberExpression.Expression, expression.Parameters).Compile();
                return (lamdba.DynamicInvoke(obj), memberExpression.Member.Name);
            }
            else
                throw new ArgumentException("暂不支持对 MemberAccess 以外的表达式进行依赖分析");
        }

        /// <summary>
        /// 获取一个观察指定依赖属性变化的 Observable
        /// 仅监听最后一级属性的变化
        /// </summary>
        public static IObservable<TResult> WhenPropertyChanged<T, TResult>(this T obj, Expression<Func<T, TResult>> expr)
        {
            var func = expr.Compile();
            (var component, var propertyName) = GetExpressionDependency(obj, expr);
            return Observable.Create<TResult>((o) =>
            {
                var ownerType = component.GetType();
                if (component is INotifyPropertyChanged notifiable)
                {
                    void Handler(object sender, PropertyChangedEventArgs e)
                    {
                        if (e.PropertyName == propertyName)
                            o.OnNext((TResult)ownerType.GetProperty(propertyName).GetValue(component));
                    };
                    notifiable.PropertyChanged += Handler;
                    return () => notifiable.PropertyChanged -= Handler;
                }
                else if (component is DependencyObject dependencyObject)
                {
                    var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromName(propertyName, ownerType, ownerType);
                    void Handler(object s, EventArgs e) => o.OnNext((TResult)dependencyPropertyDescriptor.GetValue(component));
                    dependencyPropertyDescriptor.AddValueChanged(component, Handler);
                    return () => dependencyPropertyDescriptor.RemoveValueChanged(component, Handler);
                }
                throw new ArgumentException("监听属性失败");
            }).Select(x => func(obj)).DistinctUntilChanged();
        }

        /// <summary>
        /// 获取一个观察指定依赖属性变化的 Observable
        /// 仅监听表达式最后一级属性的变化
        /// </summary>
        public static IObservable<Tuple<TResult1, TResult2>> WhenPropertyChanged<T, TResult1, TResult2>(this T obj,
            Expression<Func<T, TResult1>> expr1,
            Expression<Func<T, TResult2>> expr2)
        {
            var func1 = expr1.Compile();
            var func2 = expr2.Compile();
            var observable1 = WhenPropertyChanged(obj, expr1).Select(x => (object)x);
            var observable2 = WhenPropertyChanged(obj, expr2).Select(x => (object)x);
            return Observable.Merge(observable1, observable2).Select(x => Tuple.Create(func1(obj), func2(obj)));
        }

        /// <summary>
        /// 获取一个观察指定依赖属性变化的 Observable
        /// 仅监听表达式最后一级属性的变化
        /// </summary>
        public static IObservable<Tuple<TResult1, TResult2, TResult3>> WhenPropertyChanged<T, TResult1, TResult2, TResult3>(this T obj,
            Expression<Func<T, TResult1>> expr1,
            Expression<Func<T, TResult2>> expr2,
            Expression<Func<T, TResult3>> expr3)
        {
            var func1 = expr1.Compile();
            var func2 = expr2.Compile();
            var func3 = expr3.Compile();
            var observable1 = WhenPropertyChanged(obj, expr1).Select(x => (object)x);
            var observable2 = WhenPropertyChanged(obj, expr2).Select(x => (object)x);
            var observable3 = WhenPropertyChanged(obj, expr3).Select(x => (object)x);
            return Observable.Merge(observable1, observable2, observable3).Select(x => Tuple.Create(func1(obj), func2(obj), func3(obj)));
        }

        /// <summary>
        /// 获取一个观察指定依赖属性变化的 Observable
        /// 仅监听表达式最后一级属性的变化
        /// </summary>
        public static IObservable<Tuple<TResult1, TResult2, TResult3, TResult4>> WhenPropertyChanged<T, TResult1, TResult2, TResult3, TResult4>(this T obj,
            Expression<Func<T, TResult1>> expr1,
            Expression<Func<T, TResult2>> expr2,
            Expression<Func<T, TResult3>> expr3,
            Expression<Func<T, TResult4>> expr4)
        {
            var func1 = expr1.Compile();
            var func2 = expr2.Compile();
            var func3 = expr3.Compile();
            var func4 = expr4.Compile();
            var observable1 = WhenPropertyChanged(obj, expr1).Select(x => (object)x);
            var observable2 = WhenPropertyChanged(obj, expr2).Select(x => (object)x);
            var observable3 = WhenPropertyChanged(obj, expr3).Select(x => (object)x);
            var observable4 = WhenPropertyChanged(obj, expr4).Select(x => (object)x);
            return Observable.Merge(observable1, observable2, observable3).Select(x => Tuple.Create(func1(obj), func2(obj), func3(obj), func4(obj)));
        }
    }
}