namespace TgBotAcc.Entities
{
    public class SpendingRecords:BaseEntity
    {
        public long ChatId { get; set; }
        public string Spending { get; set; }
        public double Spent { get; set; }

    }
}
