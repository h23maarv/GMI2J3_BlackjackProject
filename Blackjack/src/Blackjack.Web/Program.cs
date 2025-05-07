using Blackjack.Core.Interfaces;
using Blackjack.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register application services
builder.Services.AddSingleton<IRandomProvider, SystemRandomProvider>();
builder.Services.AddSingleton<IDeckService, DeckService>();
builder.Services.AddSingleton<IHandService, HandService>();
builder.Services.AddSingleton<IGameService, GameService>();
builder.Services.AddSingleton<IBettingService, BettingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Blackjack}/{action=Index}/{id?}");

app.Run();
