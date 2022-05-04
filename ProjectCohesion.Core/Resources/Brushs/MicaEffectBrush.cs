using Microsoft.Graphics.Canvas.Effects;
using ProjectCohesion.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System.Power;
using Windows.UI;
using Windows.UI.Composition;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace ProjectCohesion.Core.Resources.Brushs
{
    public class MicaEffectBrush : XamlCompositionBrushBase
    {
        private static CompositionBrush BuildMicaEffectBrush(Compositor compositor, Color tintColor, float tintOpacity, float luminosityOpacity)
        {
            // Tint Color.
            var tintColorEffect = new ColorSourceEffect
            {
                Name = "TintColor",
                Color = tintColor
            };

            // OpacityEffect applied to Tint.
            var tintOpacityEffect = new OpacityEffect
            {
                Name = "TintOpacity",
                Opacity = tintOpacity,
                Source = tintColorEffect
            };

            // Apply Luminosity:

            // Luminosity Color.
            var luminosityColorEffect = new ColorSourceEffect
            {
                Color = tintColor
            };

            // OpacityEffect applied to Luminosity.
            var luminosityOpacityEffect = new OpacityEffect
            {
                Name = "LuminosityOpacity",
                Opacity = luminosityOpacity,
                Source = luminosityColorEffect
            };

            // Luminosity Blend.
            // NOTE: There is currently a bug where the names of BlendEffectMode::Luminosity and BlendEffectMode::Color are flipped.
            var luminosityBlendEffect = new BlendEffect
            {
                Mode = BlendEffectMode.Color,
                Background = new CompositionEffectSourceParameter("BlurredWallpaperBackdrop"),
                Foreground = luminosityOpacityEffect
            };

            // Apply Tint:

            // Color Blend.
            // NOTE: There is currently a bug where the names of BlendEffectMode::Luminosity and BlendEffectMode::Color are flipped.
            var colorBlendEffect = new BlendEffect
            {
                Mode = BlendEffectMode.Luminosity,
                Background = luminosityBlendEffect,
                Foreground = tintOpacityEffect
            };

            CompositionEffectBrush micaEffectBrush = compositor.CreateEffectFactory(colorBlendEffect).CreateBrush();
            micaEffectBrush.SetSourceParameter("BlurredWallpaperBackdrop", compositor.TryCreateBlurredWallpaperBackdropBrush());

            return micaEffectBrush;
        }

        private static CompositionBrush CreateCrossFadeEffectBrush(Compositor compositor, CompositionBrush from, CompositionBrush to)
        {
            var crossFadeEffect = new CrossFadeEffect
            {
                Name = "Crossfade", // Name to reference when starting the animation.
                Source1 = new CompositionEffectSourceParameter("source1"),
                Source2 = new CompositionEffectSourceParameter("source2"),
                CrossFade = 0
            };

            CompositionEffectBrush crossFadeEffectBrush = compositor.CreateEffectFactory(crossFadeEffect, new List<string>() { "Crossfade.CrossFade" }).CreateBrush();
            crossFadeEffectBrush.Comment = "CrossFade";
            // The inputs have to be swapped here to work correctly...
            crossFadeEffectBrush.SetSourceParameter("source1", to);
            crossFadeEffectBrush.SetSourceParameter("source2", from);
            return crossFadeEffectBrush;
        }

        private static ScalarKeyFrameAnimation CreateCrossFadeAnimation(Compositor compositor)
        {
            ScalarKeyFrameAnimation animation = compositor.CreateScalarKeyFrameAnimation();
            LinearEasingFunction linearEasing = compositor.CreateLinearEasingFunction();
            animation.InsertKeyFrame(0.0f, 0.0f, linearEasing);
            animation.InsertKeyFrame(1.0f, 1.0f, linearEasing);
            animation.Duration = TimeSpan.FromMilliseconds(250);
            return animation;
        }

        public MicaEffectBrush(FrameworkElement root, IWpfWindow window)
        {
            RootElement = root;
            WpfWindow = window;
        }

        public FrameworkElement RootElement { get; }

        public IWpfWindow WpfWindow { get; }

        protected override void OnConnected()
        {
            base.OnConnected();

            if (uiSettings == null)
                uiSettings = new UISettings();

            if (accessibilitySettings == null)
                accessibilitySettings = new AccessibilitySettings();

            if (isFastEffects == null)
                isFastEffects = CompositionCapabilities.GetForCurrentView().AreEffectsFast();

            if (isEnergySaver == null)
                isEnergySaver = PowerManager.EnergySaverStatus == EnergySaverStatus.On;

            UpdateBrush();

            WpfWindow.Activated += Window_GotFocus;
            WpfWindow.Deactivated += Window_LostFocus;
            uiSettings.ColorValuesChanged += Settings_ColorValuesChanged;
            accessibilitySettings.HighContrastChanged += AccessibilitySettings_HighContrastChanged;
            PowerManager.EnergySaverStatusChanged += PowerManager_EnergySaverStatusChanged;
            CompositionCapabilities.GetForCurrentView().Changed += CompositionCapabilities_Changed;
            RootElement.ActualThemeChanged += RootElement_ActualThemeChanged;
        }

        protected override void OnDisconnected()
        {
            base.OnDisconnected();

            WpfWindow.Activated -= Window_GotFocus;
            WpfWindow.Deactivated -= Window_LostFocus;

            if (uiSettings != null)
            {
                uiSettings.ColorValuesChanged -= Settings_ColorValuesChanged;
                uiSettings = null;
            }

            if (accessibilitySettings != null)
            {
                accessibilitySettings.HighContrastChanged -= AccessibilitySettings_HighContrastChanged;
                accessibilitySettings = null;
            }

            PowerManager.EnergySaverStatusChanged -= PowerManager_EnergySaverStatusChanged;
            CompositionCapabilities.GetForCurrentView().Changed -= CompositionCapabilities_Changed;
            RootElement.ActualThemeChanged -= RootElement_ActualThemeChanged;

            if (CompositionBrush != null)
            {
                CompositionBrush.Dispose();
                CompositionBrush = null;
            }
        }

        private void UpdateBrush()
        {
            if (uiSettings == null || accessibilitySettings == null)
                return;

            var currentTheme = RootElement.ActualTheme;
            var compositor = Window.Current.Compositor;

            var useSolidColorFallback =
                !uiSettings.AdvancedEffectsEnabled ||
                !WpfWindow.IsActive ||
                isFastEffects == false ||
                isEnergySaver == true;

            Color tintColor = currentTheme == ElementTheme.Light ?
                Color.FromArgb(255, 243, 243, 243) :
                Color.FromArgb(255, 32, 32, 32);
            float tintOpacity = currentTheme == ElementTheme.Light ? 0.5f : 0.8f;

            if (accessibilitySettings.HighContrast)
            {
                tintColor = uiSettings.GetColorValue(UIColorType.Background);
                useSolidColorFallback = true;
            }

            var newBrush = useSolidColorFallback ?
                compositor.CreateColorBrush(tintColor) :
                BuildMicaEffectBrush(compositor, tintColor, tintOpacity, 1.0f);

            var oldBrush = CompositionBrush;

            var doCrossFade = oldBrush != null &&
                CompositionBrush.Comment != "CrossFade" &&
                !(oldBrush is CompositionColorBrush && newBrush is CompositionColorBrush);

            if (doCrossFade)
            {
                var crossFadeBrush = CreateCrossFadeEffectBrush(compositor, oldBrush, newBrush);
                var animation = CreateCrossFadeAnimation(compositor);
                CompositionBrush = crossFadeBrush;

                var crossFadeAnimationBatch = compositor.CreateScopedBatch(CompositionBatchTypes.Animation);
                crossFadeBrush.StartAnimation("CrossFade.CrossFade", animation);
                crossFadeAnimationBatch.End();
                crossFadeAnimationBatch.Completed += (o, a) =>
                {
                    if (CompositionBrush?.Comment == "CrossFade")
                    {
                        oldBrush.Dispose();
                        CompositionBrush = newBrush;
                    }
                    crossFadeBrush.Dispose();
                };
            }
            else
            {
                CompositionBrush = newBrush;
            }
        }

        private async void Settings_ColorValuesChanged(UISettings sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateBrush();
            });
        }

        private async void AccessibilitySettings_HighContrastChanged(AccessibilitySettings sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                UpdateBrush();
            });
        }

        private async void CompositionCapabilities_Changed(CompositionCapabilities sender, object args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                isFastEffects = sender.AreEffectsFast();
                UpdateBrush();
            });
        }

        private async void PowerManager_EnergySaverStatusChanged(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                isEnergySaver = PowerManager.EnergySaverStatus == EnergySaverStatus.On;
                UpdateBrush();
            });
        }

        private void Window_GotFocus(object sender, EventArgs e)
        {
            UpdateBrush();
        }

        private void Window_LostFocus(object sender, EventArgs e)
        {
            UpdateBrush();
        }

        private void RootElement_ActualThemeChanged(FrameworkElement sender, object args)
        {
            UpdateBrush();
        }

        private UISettings uiSettings;
        private AccessibilitySettings accessibilitySettings;
        private bool? isFastEffects;
        private bool? isEnergySaver;
    }
}
