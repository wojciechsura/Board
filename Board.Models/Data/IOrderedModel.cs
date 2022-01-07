using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Models.Data
{
    public interface IOrderedModel
    {
        int Id { get; set; }
        long Order { get; set; }
    }
}
