using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using Autofac;
using ProjectCohesion.Core.Models.UIModels;
using ProjectCohesion.Core.ViewModels;
using ProjectCohesion.Utilities;

namespace ProjectCohesion.Resources
{
    public class ThemeResourceDictionary : ResourceDictionary
    {
        private Uri lightSource;
        private Uri darkSource;

        public Uri LightSource
        {
            get { return lightSource; }
            set
            {
                lightSource = value;
                UpdateSource();
            }
        }

        public Uri DarkSource
        {
            get { return darkSource; }
            set
            {
                darkSource = value;
                UpdateSource();
            }
        }

        public ThemeResourceDictionary()
        {
            ThemeListener.ThemeChanged += ThemeChanged;
        }

        private void ThemeChanged()
        {
            if (darkSource != null && lightSource != null)
            {
                var uiViewModel = Core.Autofac.Container.Resolve<UIViewModel>();
                if (uiViewModel.Theme == Themes.Default)
                {
                    Source = ThemeListener.IsDarkMode ? darkSource : lightSource;
                }
            }
        }

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
