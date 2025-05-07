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

        public BlackjackController(
            IGameService gameService,
            IBettingService bettingService)
        {
            _gameService = gameService;
            _bettingService = bettingService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new BlackjackViewModel
            {
                PlayerHand = _gameService.PlayerHand,
                DealerHand = new[] { _gameService.DealerHand.FirstOrDefault() },
                IsGameOver = false
            };
            return View(model);
        }

        [HttpPost]
        public IActionResult Deal(decimal bet)
        {
            _bettingService.PlaceBet(bet);
            _gameService.DealInitialHands();
            return RedirectToAction(nameof(Play));
        }

        [HttpGet]
        public IActionResult Play()
        {
            var model = new BlackjackViewModel
            {
                PlayerHand = _gameService.PlayerHand,
                DealerHand = new[] { _gameService.DealerHand.First() },
                IsGameOver = false,
                BetAmount = 0
            };
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Hit()
        {
            _gameService.PlayerHit();
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
                PlayerHand = _gameService.PlayerHand,
                DealerHand = _gameService.DealerHand,
                IsGameOver = true,
                Result = result,
                BetAmount = 0,
                Payout = payout
            };
            return View("Index", model);
        }
    }
}