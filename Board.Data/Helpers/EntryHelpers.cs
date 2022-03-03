using Board.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.Data.Helpers
{
    public static class EntryHelpers
    {
        public static IQueryable<Entry> ApplyFilter(this IQueryable<Entry> query, string filter)
        {
            var words = (filter?.Split(' ')
                            .Select(w => w.Trim().ToLower())
                            .Where(w => !string.IsNullOrEmpty(w))
                            .ToList());

            if (words?.Any() ?? false)
            {
                foreach (var word in words)
                    query = query.Where(e => e.Title.ToLower().Contains(word) || e.Description.ToLower().Contains(word));
            }

            return query;
        }
    }
}
