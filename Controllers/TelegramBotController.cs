using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Types;
using TgBotAcc.DataBase;
using TgBotAcc.DbCommands;
using TgBotAcc.Entities;

namespace TgBotAcc.Controllers
{
    [ApiController]
    [Route("/api/message/update")]
    public class TelegramBotController : ControllerBase
    {

        private readonly DataContext dbcontext;
        private readonly TelegramBotClient tgrBotClient;
        private readonly List<string> commands = new List<string>() {"/createRecord", "/deleteRecord", "/listRecords" };
        public TelegramBotController(TelegramBot telegramBot, DataContext context)
        {
            tgrBotClient = telegramBot.GetBot().Result;
            dbcontext = context;
        }

        [HttpPost]
        public async Task<IActionResult> Update([FromBody]object update)
        {
            var tempUdd = JsonConvert.DeserializeObject<Update>(update.ToString());
            var chat = tempUdd.Message?.Chat;
            if (chat == null)
            {
                return Ok();
            }
            //cleaning updates
            //await tgrBotClient.SendTextMessageAsync(chat.Id, "Stop");
            //return Ok();
            var appUser = new AppUser
            {
                UserName = chat?.Username,
                ChatId = chat.Id
            };
            var result = await DbOperator.GetOrAddUser(dbcontext, appUser);
            string msg = tempUdd.Message.Text.Split("\n")[0];
            switch (msg)
            {
                case "/start":
                    {
                        await tgrBotClient.SendTextMessageAsync(chat.Id, String.Format
                            ("Welcome {0}, please input next commands \n {1} \n [Spending name;16,2 (its a value example]", chat.Username, String.Join(";", commands)));
                        break;
                    }
                case "/createRecord":
                    {
                        string spendingName = tempUdd.Message.Text.Split("\n")[1].Replace("[", "").Replace("]", "").Split(";")[0];
                        string test = tempUdd.Message.Text.Split("\n")[1].Replace("[", "").Replace("]", "").Split(";")[1].Trim();
                        //double spentM = Double.Parse(tempUdd.Message.Text.Split("\n")[1].Replace("[", "").Replace("]", "").Split(",")[1].Trim());
                        double spentM = Double.Parse(test);
                        var record = new SpendingRecords()
                        {
                            ChatId = chat.Id,
                            Spending = spendingName,
                            Spent = spentM
                        };
                        DbOperator.AddSpendingRecord(dbcontext, record);
                        await tgrBotClient.SendTextMessageAsync(chat.Id, String.Format("Record has been added"));
                        break;
                    }
                case "/deleteRecord":
                    {
                        string recordId = tempUdd.Message.Text.Split("\n")[1].Replace("[", "").Replace("]", "").Split(";")[0];
                        var record = new SpendingRecords()
                        {
                            Id = Int32.Parse(recordId)
                        };
                        DbOperator.DeleteSpendingRecord(dbcontext, record);
                        await tgrBotClient.SendTextMessageAsync(chat.Id, "Record has been deleted");
                        break;
                    }
                case "/listRecords":
                    {
                        List<SpendingRecords> recList = DbOperator.GetRecords(dbcontext, result);
                        string responseR = "";
                        foreach (var i in recList)
                        {
                            responseR += i.Id + " ---- " + i.Spending + " = "+ i.Spent + "\n";
                        }
                        string records = responseR;
                        await tgrBotClient.SendTextMessageAsync(chat.Id, String.Format("Your recodrs: {0}", records));
                        break;
                    }

            }

            //var result = await dbcontext.Users.AddAsync(appUser);
            //await dbcontext.SaveChangesAsync();
            //await tgrBotClient.SendTextMessageAsync(chat.Id, "Your spending book has been created");
            return Ok();
        }
    }
}
