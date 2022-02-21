using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Board.Controls
{
    public class DateRange : Control
    {
        #region StartDate dependency property

        public DateTime? StartDate
        {
            get { return (DateTime?)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for StartDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime?), typeof(DateRange), new PropertyMetadata(null));

        #endregion

        #region EndDate dependency property

        public DateTime? EndDate
        {
            get { return (DateTime?)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EndDate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime?), typeof(DateRange), new PropertyMetadata(null));

        #endregion

        #region IsDone dependency property

        public bool IsDone
        {
            get { return (bool)GetValue(IsDoneProperty); }
            set { SetValue(IsDoneProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsDone.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsDoneProperty =
            DependencyProperty.Register("IsDone", typeof(bool), typeof(DateRange), new PropertyMetadata(false));

        #endregion

        #region IsOverdue dependency property

        public bool IsOverdue
        {
            get { return (bool)GetValue(IsOverdueProperty); }
            set { SetValue(IsOverdueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsOverdue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsOverdueProperty =
            DependencyProperty.Register("IsOverdue", typeof(bool), typeof(DateRange), new PropertyMetadata(false));

        #endregion
    }
}
