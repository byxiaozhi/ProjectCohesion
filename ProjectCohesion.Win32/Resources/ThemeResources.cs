using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Autofac;
using ProjectCohesion.Core.Models.UIModels;
using ProjectCohesion.Core.ViewModels;
using ProjectCohesion.Win32.Utilities;

namespace ProjectCohesion.Win32.Resources
{
    public class ThemeResources : ResourceDictionary
    {
        private readonly Uri lightSource = new("/ProjectCohesion.Win32;component/Resources/Styles/Theme.Light.xaml", UriKind.Relative);
        private readonly Uri darkSource = new("/ProjectCohesion.Win32;component/Resources/Styles/Theme.Dark.xaml", UriKind.Relative);
        private readonly UIViewModel uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();

        public ThemeResources()
        {
            Source = lightSource;
            uiViewModel.PropertyChanged += UiViewModel_PropertyChanged;
            ThemeListener.ThemeChanged += ThemeChanged;
        }

        ~ThemeResources()
        {
            uiViewModel.PropertyChanged -= UiViewModel_PropertyChanged;
            ThemeListener.ThemeChanged -= ThemeChanged;
        }

        private void UiViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // 当软件主题设置发生变化时更新主题
            if (e.PropertyName == nameof(UIViewModel.Theme))
            {
                UpdateSource();
            }
        }

        private void ThemeChanged()
        {
            // 当系统主题发生变化且软件主题设置为默认时更新主题
            if(uiViewModel.Theme == Themes.Default)
            {
                UpdateSource();
            }
        }

        /// <summary>
        /// 更新主题
        /// </summary>
        private void UpdateSource()
        {
            if (darkSource != null && lightSource != null)
            {
                var uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();
                switch (uiViewModel.Theme)
                {
                    case Themes.Light:
                        Source = lightSource;
                        break;
                    case Themes.Dark:
                        Source = darkSource;
                        break;
                    case Themes.Default:
                        Source = ThemeListener.IsDarkMode ? darkSource : lightSource;
                        break;
                }
            }
        }
    }
}
