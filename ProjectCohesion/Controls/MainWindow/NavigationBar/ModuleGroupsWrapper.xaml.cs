using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProjectCohesion.Controls.MainWindow
{
    public partial class ModuleGroupsWrapper : UserControl
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(object), typeof(ModuleGroupsWrapper));
        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }

        public static new readonly DependencyProperty BackgroundProperty = DependencyProperty.Register(nameof(Background), typeof(SolidColorBrush), typeof(ModuleGroupsWrapper));
        public new SolidColorBrush Background
        {
            get => (SolidColorBrush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }

        public ModuleGroupsWrapper()
        {
            InitializeComponent();
        }

        protected override async void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == ItemsSourceProperty)
            {
                var nextItem = new ModuleGroups() { ItemsSource = e.NewValue };
                if (itemsControl.Items.Count == 0)
                {
                    itemsControl.Items.Add(nextItem);
                }
                else
                {
                    // 过渡动画
                    var prevItem = itemsControl.Items[^1] as UserControl;
                    itemsControl.Items.Add(nextItem);
                    var duration = 300;
                    var ease = new CircleEase() { EasingMode = EasingMode.EaseInOut };
                    nextItem.BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(duration)) { EasingFunction = ease });
                    nextItem.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(-8, 0, 0, 0), new Thickness(0), TimeSpan.FromMilliseconds(duration)) { EasingFunction = ease });
                    prevItem.BeginAnimation(OpacityProperty, new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(duration)) { EasingFunction = ease });
                    prevItem.BeginAnimation(MarginProperty, new ThicknessAnimation(new Thickness(0), new Thickness(-8, 0, 0, 0), TimeSpan.FromMilliseconds(duration)) { EasingFunction = ease });
                    await Task.Delay(duration);
                    itemsControl.Items.Remove(prevItem);
                }
            }
        }
    }
}
