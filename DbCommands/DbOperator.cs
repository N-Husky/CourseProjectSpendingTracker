using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using TgBotAcc.DataBase;
using TgBotAcc.Entities;

namespace TgBotAcc.DbCommands
{
    public static class DbOperator
    {
        public static async Task<AppUser> GetOrAddUser(DataContext context, AppUser user)
        {
            var dbUser = await context.Users.FirstOrDefaultAsync(x => x.ChatId == user.ChatId);
            if (dbUser != null)
                return user;
            var result = await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            return result.Entity;
        }
        public static List<SpendingRecords> GetRecords(DataContext context, AppUser user)
        {
            //List<SpendingRecords> result = context.SRecords.Where(x=> x.ChatId == user.ChatId).ToList<SpendingRecords>();    
            List<SpendingRecords> result = context.SRecords.ToList<SpendingRecords>();
            return result;
        }
        public static async Task AddSpendingRecord(DataContext context, SpendingRecords record)
        {
            var result = await context.SRecords.AddAsync(record);
            await context.SaveChangesAsync();
        }
        public static async Task DeleteSpendingRecord(DataContext context, SpendingRecords record)
        {
            var dbRecord = await context.SRecords.FirstOrDefaultAsync(x => x.Id == record.Id);
            context.SRecords.Remove(dbRecord);
            await context.SaveChangesAsync();
        }
    }
}
