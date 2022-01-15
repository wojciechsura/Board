using Board.BusinessLogic.Types.Attributes;
using Board.BusinessLogic.ViewModels.Base;
using Spooksoft.VisualStateManager.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Board.BusinessLogic.ViewModels.Main
{
    public class EntryDateEditorViewModel : BaseViewModel
    {
        // Private constants --------------------------------------------------

        private static Regex timeRegex = new Regex("^([01]?[0-9]|2[0-3]):([0-5][0-9])$");

        private const string DATES_PROPERTY_GROUP = "Dates";

        // Private fields -----------------------------------------------------

        private readonly IEntryDateEditorHandler handler;

        private bool startDateSet;
        private DateTime startDate;
        private string startTime;
        private bool endDateSet;
        private DateTime endDate;
        private string endTime;
        private bool isEditingDates;

        // Private methods ----------------------------------------------------

        private void HandleDatesChanged()
        {
            IsEditingDates = true;
        }

        private void InitDates(DateTime? startDate, DateTime? endDate)
        {
            this.startDateSet = startDate != null;
            this.startDate = startDate ?? DateTime.Now;
            this.startTime = startDate?.ToString("HH:mm") ?? DateTime.Now.ToString("HH:mm");

            this.endDateSet = endDate != null;
            this.endDate = endDate ?? DateTime.Now;
            this.endTime = endDate?.ToString("HH.mm") ?? DateTime.Now.ToString("HH:mm");
        }

        private (int h, int m) SanitizeTime(string startTime)
        {
            int h = 0, m = 0;
            if (timeRegex.IsMatch(startTime))
            {
                string[] parts = startTime.Split(':');
                h = int.Parse(parts[0]);
                m = int.Parse(parts[1]);
            }

            return (h, m);
        }

        // Public methods -----------------------------------------------------

        public EntryDateEditorViewModel(DateTime? startDate, DateTime? endDate, IEntryDateEditorHandler handler)
        {
            this.handler = handler;

            InitDates(startDate, endDate);            
        }

        public void Cancel()
        {
            (DateTime? startDate, DateTime? endDate) = handler.GetCurrentDates();

            InitDates(startDate, endDate);
            this.PropertyGroupChanged(DATES_PROPERTY_GROUP);

            IsEditingDates = false;
        }

        public void Save()
        {
            DateTime? newStartDate = null, newEndDate = null;

            if (startDateSet)
            {
                (int h, int m) = SanitizeTime(startTime);

                newStartDate = new DateTime(StartDate.Year, StartDate.Month, StartDate.Day, h, m, 0);
            }

            if (endDateSet)
            {
                (int h, int m) = SanitizeTime(endTime);

                newEndDate = new DateTime(EndDate.Year, EndDate.Month, EndDate.Day, h, m, 0);
                if (newStartDate != null && newEndDate < newStartDate)
                    newEndDate = newStartDate.Value;
            }

            handler.SetCurrentDates(newStartDate, newEndDate);

            IsEditingDates = false;
        }


        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public bool IsEditingDates
        {
            get => isEditingDates;
            set => Set(ref isEditingDates, value);
        }

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public bool StartDateSet
        {
            get => startDateSet;
            set => Set(ref startDateSet, value, changeHandler: HandleDatesChanged);
        }

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public DateTime StartDate
        {
            get => startDate;
            set => Set(ref startDate, value, changeHandler: HandleDatesChanged);
        }

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public string StartTime
        {
            get => startTime;
            set => Set(ref startTime, value, changeHandler: HandleDatesChanged);
        }

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public bool EndDateSet
        {
            get => endDateSet;
            set => Set(ref endDateSet, value, changeHandler: HandleDatesChanged);
        }

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public DateTime EndDate
        {
            get => endDate;
            set => Set(ref endDate, value, changeHandler: HandleDatesChanged);
        }

        [PropertyNotificationGroup(DATES_PROPERTY_GROUP)]
        public string EndTime
        {
            get => endTime;
            set => Set(ref endTime, value, changeHandler: HandleDatesChanged);
        }
    }
}
