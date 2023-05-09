using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TgBotAcc;
using TgBotAcc.DataBase;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DataContext>(opt=>opt.UseSqlServer(connectionString));
builder.Services.AddSingleton<TelegramBot>();
var app = builder.Build();
app.Services.GetRequiredService<TelegramBot>().GetBot().Wait();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.MapControllers();
app.UseAuthorization();
app.Run();
