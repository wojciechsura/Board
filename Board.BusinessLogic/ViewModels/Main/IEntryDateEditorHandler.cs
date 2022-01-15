using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.ViewModels.Main
{
    public interface IEntryDateEditorHandler
    {
        (DateTime? startDate, DateTime? endDate) GetCurrentDates();
        void SetCurrentDates(DateTime? newStartDate, DateTime? newEndDate);
    }
}
