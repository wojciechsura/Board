using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Board.Common.Wpf.Behaviors
{
    public class ClickBehavior : Behavior<FrameworkElement>
    {
        // Private methods ----------------------------------------------------

        private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Loaded -= AssociatedObject_Loaded;
            AssociatedObject.PreviewMouseLeftButtonUp += AssociatedObject_PreviewMouseLeftButtonUp;
        }

        private void AssociatedObject_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (Command != null && Command.CanExecute(null))
            {
                Command.Execute(null);
            }
        }

        private void AssociatedObject_Unloaded(object sender, RoutedEventArgs e)
        {
            AssociatedObject.Unloaded -= AssociatedObject_Unloaded;
            AssociatedObject.PreviewMouseLeftButtonUp -= AssociatedObject_PreviewMouseLeftButtonUp;
        }

        // Protected methods --------------------------------------------------

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Loaded += AssociatedObject_Loaded;
            AssociatedObject.Unloaded += AssociatedObject_Unloaded;
        }

        // Public methods -----------------------------------------------------
        
        public ClickBehavior()
        {

        }

        // Dependency properties ----------------------------------------------

        #region CommandProperty

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandPropery);
            set => SetValue(CommandPropery, value);
        }

        public static readonly DependencyProperty CommandPropery =
                 DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(ClickBehavior));

        #endregion
    }
}
