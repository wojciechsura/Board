using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Board.Controls
{
    /// <summary>
    /// Logika interakcji dla klasy SmallTag.xaml
    /// </summary>
    public partial class SmallTag : Border
    {
        private void UpdateDetails()
        {
            switch (ShowDetails)
            {
                case true:
                    {
                        ClearValue(WidthProperty);
                        CornerRadius = new CornerRadius(8);
                        Height = 16;
                        tbDescription.Visibility = Visibility.Visible;

                        break;
                    }
                case false:
                    {
                        Width = 40;
                        CornerRadius = new CornerRadius(4);
                        Height = 8;
                        tbDescription.Visibility = Visibility.Collapsed;

                        break;
                    }
            }
        }

        public SmallTag()
        {
            InitializeComponent();
            HandleShowDetailsChanged();
        }

        #region ShowDetails dependency property

        public bool ShowDetails
        {
            get { return (bool)GetValue(ShowDetailsProperty); }
            set { SetValue(ShowDetailsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowDetails.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowDetailsProperty =
            DependencyProperty.Register("ShowDetails", typeof(bool), typeof(SmallTag), new PropertyMetadata(false, HandleShowDetailsPropertyChanged));

        private static void HandleShowDetailsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SmallTag smallTag)
            {
                smallTag.HandleShowDetailsChanged();
            }
        }

        private void HandleShowDetailsChanged()
        {
            UpdateDetails();
        }

        #endregion
    }
}
