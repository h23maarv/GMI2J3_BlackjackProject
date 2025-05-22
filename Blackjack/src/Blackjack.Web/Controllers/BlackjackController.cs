using Microsoft.AspNetCore.Mvc;
using Blackjack.Core.Interfaces;
using Blackjack.Core.Models;
using Blackjack.Web.Models;

namespace Blackjack.Web.Controllers
{
    public class BlackjackController : Controller
    {
        private readonly IGameService _gameService;
        private readonly IBettingService _bettingService;

        public BlackjackController(IGameService gameService, IBettingService bettingService)
        {
            _gameService = gameService;
            _bettingService = bettingService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Visar bara startsida med insats och Deal-knapp
            var model = new BlackjackViewModel
            {
                PlayerHand = new List<Card>(),
                DealerHand = new List<Card>(),
                IsGameOver = false
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Deal(decimal bet)
        {
            _bettingService.PlaceBet(bet);
            _gameService.DealInitialHands();
            // Visa Play (inte Index) efter Deal!
            return RedirectToAction(nameof(Play));
        }

        [HttpGet]
        public IActionResult Play()
        {
            // Pågående spel, visa Hit och Stand
            var model = new BlackjackViewModel
            {
                PlayerHand = _gameService.PlayerHand ?? new List<Card>(),
                DealerHand = _gameService.DealerHand?.Take(1).ToList() ?? new List<Card>(), // Bara första kortet
                IsGameOver = false
            };
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Hit()
        {
            _gameService.PlayerHit();
            // Visa Play (inte Index) efter Hit!
            return RedirectToAction(nameof(Play));
        }

        [HttpPost]
        public IActionResult Stand()
        {
            _gameService.DealerPlay();
            var result = _gameService.EvaluateOutcome();
            var payout = _bettingService.Payout(result);

            var model = new BlackjackViewModel
            {
                PlayerHand = _gameService.PlayerHand ?? new List<Card>(),
                DealerHand = _gameService.DealerHand ?? new List<Card>(), // Visa ALLA kort nu
                IsGameOver = true,
                Result = result,
                BetAmount = 0,
                Payout = payout
            };
            return View("Index", model);
        }
    }
}