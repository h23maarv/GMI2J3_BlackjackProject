using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Blackjack.Web.Models;
using Blackjack.Core.Models;

namespace Blackjack.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        var model = new BlackjackViewModel
        {
            PlayerHand = new List<Card>(),
            DealerHand = new List<Card>(),
            IsGameOver = false,
            Result = null,
            BetAmount = 0,
            Payout = 0
        };
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
