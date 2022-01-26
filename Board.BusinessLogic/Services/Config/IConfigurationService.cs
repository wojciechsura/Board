using Board.Models.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Services.Config
{
    public interface IConfigurationService
    {
        Configuration Configuration { get; }
        bool Load();
        bool Save();
        void NotifyConfigurationChanged();
    }
}
