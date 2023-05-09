using Microsoft.EntityFrameworkCore;
using TgBotAcc.Entities;

namespace TgBotAcc.DataBase
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<AppUser> Users { get; set; }
        public DbSet<SpendingRecords> SRecords { get; set; }
    }
}
