﻿using System;
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

        private Action CreateOneWayChannel(
            Windows.UI.Xaml.DependencyObject target,
            Windows.UI.Xaml.DependencyProperty targetProp,
            System.Windows.DependencyObject source,
            System.Windows.DependencyProperty sourceProp)
        {
            void Handler(object s, EventArgs e)
            {
                var newValue = source.GetValue(sourceProp);
                var oldValue = target.GetValue(targetProp);
                if (!(newValue?.Equals(oldValue) ?? newValue == oldValue))
                    target.SetValue(targetProp, newValue);
            }
            var dependencyPropertyDescriptor = DependencyPropertyDescriptor.FromProperty(sourceProp, source.GetType());
            dependencyPropertyDescriptor.AddValueChanged(source, Handler);
            return () => dependencyPropertyDescriptor.RemoveValueChanged(source, Handler);
        }

        private Action CreateOneWayChannel(
             System.Windows.DependencyObject target,
            System.Windows.DependencyProperty targetProp,
            Windows.UI.Xaml.DependencyObject source,
            Windows.UI.Xaml.DependencyProperty sourceProp)
        {
            var token = source.RegisterPropertyChangedCallback(sourceProp, (s, e) =>
            {
                var newValue = source.GetValue(sourceProp);
                var oldValue = target.GetValue(targetProp);
                if (!(newValue?.Equals(oldValue) ?? newValue == oldValue))
                    target.SetValue(targetProp, newValue);
            });
            return () => source.UnregisterPropertyChangedCallback(sourceProp, token);
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
            var dispose1 = CreateOneWayChannel(target, targetProp, source, sourceProp);
            var dispose2 = CreateOneWayChannel(source, sourceProp, target, targetProp);
            AddBindingDisposeAction(target, targetProp, () =>
            {
                dispose1();
                dispose2();
            });
        }

        public void OneWayBinding(
            Windows.UI.Xaml.DependencyObject target,
            Windows.UI.Xaml.DependencyProperty targetProp,
            System.Windows.DependencyObject source,
            System.Windows.DependencyProperty sourceProp)
        {
            target.SetValue(targetProp, source.GetValue(sourceProp));
            var dispose = CreateOneWayChannel(target, targetProp, source, sourceProp);
            AddBindingDisposeAction(target, targetProp, dispose);
        }

        public void OneWayToSourceBinding(
            Windows.UI.Xaml.DependencyObject target,
            Windows.UI.Xaml.DependencyProperty targetProp,
            System.Windows.DependencyObject source,
            System.Windows.DependencyProperty sourceProp)
        {
            source.SetValue(sourceProp, target.GetValue(targetProp));
            var dispose = CreateOneWayChannel(source, sourceProp, target, targetProp);
            AddBindingDisposeAction(target, targetProp, dispose);
        }

        ~PropertyBridge()
        {
            foreach (var action in disposeActions.Values)
                action();
        }
    }
}
