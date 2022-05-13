using Autofac;
using ProjectCohesion.Core.Models.EventArgs;
using ProjectCohesion.Core.Services;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectCohesion.Core.Reactive
{
    public static class ReactiveExtension
    {
        private static EventCenter EventCenter => Autofac.Container.Resolve<EventCenter>();

        /// <summary>
        /// 获取表达式依赖
        /// TODO: 对更复杂的表达式进行依赖分析
        /// </summary>
        public static (Type, string) GetExpressionDependency<T, TResult>(Expression<Func<T, TResult>> expression)
        {
            if (expression.Body is MemberExpression memberExpression)
                return (memberExpression.Member.DeclaringType, memberExpression.Member.Name);
            else
                throw new ArgumentException("暂不支持对 MemberAccess 以外的表达式进行依赖分析");
        }

        /// <summary>
        /// 获取一个观察表达式结果变化的 Observable
        /// 当属性依赖的类为 ReactiveObject 或 ReactiveControl 的子类时可以使用
        /// </summary>
        public static IObservable<TResult> WhenAny<T, TResult>(this T obj,
            Expression<Func<T, TResult>> expr)
        {
            var func = expr.Compile();
            (var declaringType, var propertyName) = GetExpressionDependency(expr);
            return EventCenter.GetObservable<ReactiveEventArgs>("PropertyChanged")
                .Select(x => x.EventArgs)
                .Where(x => x.DeclaringType == declaringType)
                .Where(x => x.PropertyName == propertyName)
                .Select(x => func(obj))
                .DistinctUntilChanged();
        }

        /// <summary>
        /// 获取一个观察表达式结果变化的 Observable
        /// 当属性依赖的类为 ReactiveObject 或 ReactiveControl 的子类时可以使用
        /// </summary>
        public static IObservable<Tuple<TResult1, TResult2>> WhenAny<T, TResult1, TResult2>(this T obj,
            Expression<Func<T, TResult1>> expr1,
            Expression<Func<T, TResult2>> expr2)
        {
            var func1 = expr1.Compile();
            var func2 = expr2.Compile();
            var observable1 = WhenAny(obj, expr1).Select(x => (object)x);
            var observable2 = WhenAny(obj, expr2).Select(x => (object)x);
            return Observable.Merge(observable1, observable2).Select(x => Tuple.Create(func1(obj), func2(obj)));
        }

        /// <summary>
        /// 获取一个观察表达式结果变化的 Observable
        /// 当属性依赖的类为 ReactiveObject 或 ReactiveControl 的子类时可以使用
        /// </summary>
        public static IObservable<Tuple<TResult1, TResult2, TResult3>> WhenAny<T, TResult1, TResult2, TResult3>(this T obj,
            Expression<Func<T, TResult1>> expr1,
            Expression<Func<T, TResult2>> expr2,
            Expression<Func<T, TResult3>> expr3)
        {
            var func1 = expr1.Compile();
            var func2 = expr2.Compile();
            var func3 = expr3.Compile();
            var observable1 = WhenAny(obj, expr1).Select(x => (object)x);
            var observable2 = WhenAny(obj, expr2).Select(x => (object)x);
            var observable3 = WhenAny(obj, expr3).Select(x => (object)x);
            return Observable.Merge(observable1, observable2, observable3).Select(x => Tuple.Create(func1(obj), func2(obj), func3(obj)));
        }

        /// <summary>
        /// 获取一个观察表达式结果变化的 Observable
        /// 当属性依赖的类为 ReactiveObject 或 ReactiveControl 的子类时可以使用
        /// </summary>
        public static IObservable<Tuple<TResult1, TResult2, TResult3, TResult4>> WhenAny<T, TResult1, TResult2, TResult3, TResult4>(this T obj,
            Expression<Func<T, TResult1>> expr1,
            Expression<Func<T, TResult2>> expr2,
            Expression<Func<T, TResult3>> expr3,
            Expression<Func<T, TResult4>> expr4)
        {
            var func1 = expr1.Compile();
            var func2 = expr2.Compile();
            var func3 = expr3.Compile();
            var func4 = expr4.Compile();
            var observable1 = WhenAny(obj, expr1).Select(x => (object)x);
            var observable2 = WhenAny(obj, expr2).Select(x => (object)x);
            var observable3 = WhenAny(obj, expr3).Select(x => (object)x);
            var observable4 = WhenAny(obj, expr4).Select(x => (object)x);
            return Observable.Merge(observable1, observable2, observable3).Select(x => Tuple.Create(func1(obj), func2(obj), func3(obj), func4(obj)));
        }
    }
}
