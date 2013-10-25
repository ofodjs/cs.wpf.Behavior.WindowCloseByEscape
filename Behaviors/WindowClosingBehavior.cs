/*
The MIT License (MIT)
Copyright (c) 2013 Sergey Serdyuk
*/
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace WpfClosingByEscape.Behaviors
{

    public class WindowClosingBehavior
    {

        public static readonly ICommand WindowCloseCommand = new DelegateCommand<Window>(win => win.Close());

        public static bool GetEnabled(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnabledProperty);
        }

        public static void SetEnabled(DependencyObject obj, bool value)
        {
            obj.SetValue(EnabledProperty, value);
        }

        public static readonly DependencyProperty EnabledProperty =
            DependencyProperty.RegisterAttached("Enabled", typeof(bool),
            typeof(WindowClosingBehavior),
            new UIPropertyMetadata(false, new PropertyChangedCallback(OnValueChange)));

        private static void OnValueChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Window))
            {
                return;
            }
            var window = d as Window;
            if ((bool)e.NewValue)
            {
                InputBinding escapeBinding = new InputBinding(WindowCloseCommand, new KeyGesture(Key.Escape));
                escapeBinding.CommandParameter = window;
                window.InputBindings.Add(escapeBinding);
                window.Closing += Window_Closing;
            }
            else
            {
                window.Closing -= Window_Closing;
            }
        }

        /// <summary>
        /// Confirm exit
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = !ShowExitDialog(sender as Window);
        }

        static bool ShowExitDialog(Window window)
        {
            MessageBoxResult result = MessageBox.Show(window, "Are you really want to exit?", "Exit",
                MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            return result == MessageBoxResult.Yes;
        }
    }

}
