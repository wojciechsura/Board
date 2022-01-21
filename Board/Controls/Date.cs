using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Board.Controls
{
    public class Date : Control
    {
        #region DisplayDate dependency property

        public DateTime? DisplayDate
        {
            get { return (DateTime?)GetValue(DisplayDateProperty); }
            set { SetValue(DisplayDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Date.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DisplayDateProperty =
            DependencyProperty.Register("DisplayDate", typeof(DateTime?), typeof(Date), new PropertyMetadata(null));

        #endregion

        #region IsDone dependency property

        public bool IsDone
        {
            get { return (bool)GetValue(IsDoneProperty); }
            set { SetValue(IsDoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDoneProperty =
            DependencyProperty.Register("IsDone", typeof(bool), typeof(Date), new PropertyMetadata(false));

        #endregion
    }
}
