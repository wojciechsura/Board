using Board.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Board.Data.SQLite
{
    public class TableContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbPath}");
        }
        
        public TableContext()
        {
            DbPath = String.Empty;
        }

        public TableContext(string path)
        {
            DbPath = path;
        }

        public string DbPath { get; private set; }
        public DbSet<Table>? Tables { get; set; }
    }
}