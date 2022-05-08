using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ProjectCohesion.Win32.Utilities
{
    public class PropertyBridge
    {
        readonly Dictionary<Tuple<object, object>, Action> disposeActions = new();

        private void AddBindingDisposeAction(object target, object targetProp, Action disposeAction)
        {
            var key = Tuple.Create(target, targetProp);
            if (disposeActions.ContainsKey(key))
                disposeActions[key]();
            disposeActions.Add(key, disposeAction);
        }

        public void RemoveBinding(object target, object targetProp)
        {
            var key = Tuple.Create(target, targetProp);
            if (disposeActions.ContainsKey(key))
            {
                disposeActions[key]();
                disposeActions.Remove(key);
            }
        }

        public void TwoWayBinding(
            Windows.UI.Xaml.DependencyObject target,
            Windows.UI.Xaml.DependencyProperty targetProp,
            System.Windows.DependencyObject source,
            System.Windows.DependencyProperty sourceProp)
        {
            target.SetValue(targetProp, source.GetValue(sourceProp));
            var targetToSourceToken = target.RegisterPropertyChangedCallback(targetProp, (s, e) =>
            {
                source.SetValue(sourceProp, target.GetValue(targetProp));
            });
            void sourceToTargetHandler(object s, EventArgs e)
            {
                target.SetValue(targetProp, source.GetValue(sourceProp));
            }
            var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(sourceProp, source.GetType());
            dependencyPropertyDescriptor.AddValueChanged(source, sourceToTargetHandler);
            AddBindingDisposeAction(target, targetProp, () =>
            {
                target.UnregisterPropertyChangedCallback(targetProp, targetToSourceToken);
                dependencyPropertyDescriptor.RemoveValueChanged(source, sourceToTargetHandler);
            });
        }

        public void OneWayBinding(
            Windows.UI.Xaml.DependencyObject target,
            Windows.UI.Xaml.DependencyProperty targetProp,
            System.Windows.DependencyObject source,
            System.Windows.DependencyProperty sourceProp)
        {
            target.SetValue(targetProp, source.GetValue(sourceProp));
            void sourceToTargetHandler(object s, EventArgs e)
            {
                target.SetValue(targetProp, source.GetValue(sourceProp));
            }
            var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(sourceProp, source.GetType());
            dependencyPropertyDescriptor.AddValueChanged(source, sourceToTargetHandler);
            AddBindingDisposeAction(target, targetProp, () =>
            {
                dependencyPropertyDescriptor.RemoveValueChanged(source, sourceToTargetHandler);
            });
        }

        public void OneWayToSourceBinding(
            Windows.UI.Xaml.DependencyObject target,
            Windows.UI.Xaml.DependencyProperty targetProp,
            System.Windows.DependencyObject source,
            System.Windows.DependencyProperty sourceProp)
        {
            source.SetValue(sourceProp, target.GetValue(targetProp));
            var targetToSourceToken = target.RegisterPropertyChangedCallback(targetProp, (s, e) =>
            {
                source.SetValue(sourceProp, target.GetValue(targetProp));
            });
            AddBindingDisposeAction(target, targetProp, () =>
            {
                target.UnregisterPropertyChangedCallback(targetProp, targetToSourceToken);
            });
        }

        ~PropertyBridge()
        {
            foreach (var action in disposeActions.Values)
                action();
        }
    }
}
