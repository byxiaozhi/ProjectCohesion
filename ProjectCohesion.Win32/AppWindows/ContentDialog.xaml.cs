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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectCohesion.Win32.AppWindows
{
    public partial class ContentDialog : WindowBase
    {
        public Controls.Button PrimaryButton => primaryButton;
        public Controls.Button SecondaryButton => secondaryButton;
        public Controls.Button CloseButton => closeButton;


        public static new readonly DependencyProperty ContentProperty = DependencyProperty.Register(nameof(Content), typeof(object), typeof(ContentDialog), null);
        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }

        public static readonly DependencyProperty PrimaryButtonTextProperty = DependencyProperty.Register(nameof(PrimaryButtonText), typeof(string), typeof(ContentDialog), null);
        public string PrimaryButtonText
        {
            get => (string)GetValue(PrimaryButtonTextProperty);
            set => SetValue(PrimaryButtonTextProperty, value);
        }

        public static readonly DependencyProperty SecondaryButtonTextProperty = DependencyProperty.Register(nameof(SecondaryButtonText), typeof(string), typeof(ContentDialog), null);
        public string SecondaryButtonText
        {
            get => (string)GetValue(SecondaryButtonTextProperty);
            set => SetValue(SecondaryButtonTextProperty, value);
        }

        public static readonly DependencyProperty CloseButtonTextTextProperty = DependencyProperty.Register(nameof(CloseButtonText), typeof(string), typeof(ContentDialog), null);
        public string CloseButtonText
        {
            get => (string)GetValue(CloseButtonTextTextProperty);
            set => SetValue(CloseButtonTextTextProperty, value);
        }

        public static readonly DependencyProperty PrimaryButtonStyleProperty = DependencyProperty.Register(nameof(PrimaryButtonStyle), typeof(string), typeof(ContentDialog), null);
        public string PrimaryButtonStyle
        {
            get => (string)GetValue(PrimaryButtonStyleProperty);
            set => SetValue(PrimaryButtonStyleProperty, value);
        }

        public static readonly DependencyProperty SecondaryButtonStyleProperty = DependencyProperty.Register(nameof(SecondaryButtonStyle), typeof(string), typeof(ContentDialog), null);
        public string SecondaryButtonStyle
        {
            get => (string)GetValue(SecondaryButtonStyleProperty);
            set => SetValue(SecondaryButtonStyleProperty, value);
        }

        public static readonly DependencyProperty CloseButtonTextStyleProperty = DependencyProperty.Register(nameof(CloseButtonStyle), typeof(string), typeof(ContentDialog), null);
        public string CloseButtonStyle
        {
            get => (string)GetValue(CloseButtonTextStyleProperty);
            set => SetValue(CloseButtonTextStyleProperty, value);
        }

        public event RoutedEventHandler PrimaryButtonClick;

        public event RoutedEventHandler SecondaryButtonClick;

        public event RoutedEventHandler CloseButtonClick;

        public enum Result
        {
            PrimaryButton, SecondaryButton, CloseButton
        }

        private Result result = Result.CloseButton;

        private void SetResultAndClose(Result result)
        {
            this.result = result;
            Close();
        }

        public new Result ShowDialog()
        {
            base.ShowDialog();
            return result;
        }

        public async Task<Result> ShowDialogAsync()
        {
            return await Dispatcher.InvokeAsync(ShowDialog);
        }

        public ContentDialog()
        {
            InitializeComponent();
        }

        private void primaryButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var routedEventArg = new RoutedEventArgs();
                PrimaryButtonClick?.Invoke(this, routedEventArg);
                if (!routedEventArg.Handled)
                    SetResultAndClose(Result.PrimaryButton);
            });
        }

        private void secondaryButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var routedEventArg = new RoutedEventArgs();
                SecondaryButtonClick?.Invoke(this, routedEventArg);
                if (!routedEventArg.Handled)
                    SetResultAndClose(Result.SecondaryButton);
            });
        }

        private void closeButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(() =>
            {
                var routedEventArg = new RoutedEventArgs();
                CloseButtonClick?.Invoke(this, routedEventArg);
                if (!routedEventArg.Handled)
                    SetResultAndClose(Result.CloseButton);
            });
        }
    }
}
