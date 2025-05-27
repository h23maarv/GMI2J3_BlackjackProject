using Microsoft.AspNetCore.Mvc;
using Blackjack.Core.Interfaces;
using Blackjack.Core.Models;
using Blackjack.Web.Models;
using Blackjack.Core.Services;
using Microsoft.AspNetCore.Http;

namespace Blackjack.Web.Controllers
{
    public class BlackjackController : Controller
    {
        private readonly IGameService _gameService;
        private readonly IBettingService _bettingService;
        private readonly IHandService _handService;

        public BlackjackController(
            IGameService gameService,
            IBettingService bettingService,
            IHandService handService)
        {
            _gameService = gameService;
            _bettingService = bettingService;
            _handService = handService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Hämta senaste insatsen från sessionen, default 10
            decimal lastBet = 10;
            var lastBetStr = HttpContext.Session.GetString("LastBet");
            if (!string.IsNullOrEmpty(lastBetStr))
            {
                decimal.TryParse(lastBetStr, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out lastBet);
            }

            TempData["BetAmount"] = null;

            var model = new BlackjackViewModel
            {
                PlayerHand = new List<Card>(),
                DealerHand = new List<Card>(),
                IsGameOver = false,
                BetAmount = lastBet,
                SessionVinst = GetSessionVinst()
            };
            return View(model);
        }


        [HttpPost]
        public IActionResult Deal(decimal bet)
        {
            _bettingService.PlaceBet(bet);
            _gameService.DealInitialHands();
            TempData["BetAmount"] = bet.ToString(System.Globalization.CultureInfo.InvariantCulture);
            HttpContext.Session.SetString("LastBet", bet.ToString(System.Globalization.CultureInfo.InvariantCulture)); // <--- Lägg till denna rad
            return RedirectToAction(nameof(Play));
        }

        [HttpGet]
        public IActionResult Play()
        {
            decimal betAmount = GetBetAmountFromTempData();

            var model = new BlackjackViewModel
            {
                PlayerHand = _gameService.PlayerHand ?? new List<Card>(),
                DealerHand = _gameService.DealerHand?.Take(1).ToList() ?? new List<Card>(),
                IsGameOver = false,
                BetAmount = betAmount,
                SessionVinst = GetSessionVinst()
            };
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult Hit()
        {
            _gameService.PlayerHit();

            int playerValue = _handService.CalculateValue(_gameService.PlayerHand);

            decimal betAmount = GetBetAmountFromTempData();

            if (playerValue > 21)
            {
                _gameService.DealerPlay();
                var result = _gameService.EvaluateOutcome();
                var payout = _bettingService.Payout(result);

                decimal vinst = CalculateVinst(result, payout, betAmount);

                UpdateSessionVinst(vinst);

                var model = new BlackjackViewModel
                {
                    PlayerHand = _gameService.PlayerHand ?? new List<Card>(),
                    DealerHand = _gameService.DealerHand ?? new List<Card>(),
                    IsGameOver = true,
                    Result = result,
                    BetAmount = betAmount,
                    Payout = payout,
                    SessionVinst = GetSessionVinst()
                };
                return View("Index", model);
            }

            return RedirectToAction(nameof(Play));
        }

        [HttpPost]
        public IActionResult Stand()
        {
            _gameService.DealerPlay();
            var result = _gameService.EvaluateOutcome();
            var payout = _bettingService.Payout(result);

            decimal betAmount = GetBetAmountFromTempData();
            decimal vinst = CalculateVinst(result, payout, betAmount);

            UpdateSessionVinst(vinst);

            var model = new BlackjackViewModel
            {
                PlayerHand = _gameService.PlayerHand ?? new List<Card>(),
                DealerHand = _gameService.DealerHand ?? new List<Card>(),
                IsGameOver = true,
                Result = result,
                BetAmount = betAmount,
                Payout = payout,
                SessionVinst = GetSessionVinst()
            };
            return View("Index", model);
        }

        // ======== Hjälpmetoder nedan =========

        private decimal GetBetAmountFromTempData()
        {
            decimal betAmount = 0;
            if (TempData["BetAmount"] != null)
            {
                decimal.TryParse(TempData["BetAmount"].ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out betAmount);
                TempData.Keep("BetAmount");
            }
            return betAmount;
        }

        private decimal GetSessionVinst()
        {
            decimal sessionVinst = 0;
            if (HttpContext.Session.GetString("Saldo") != null)
            {
                decimal.TryParse(HttpContext.Session.GetString("Saldo"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out sessionVinst);
            }
            return sessionVinst;
        }

        private void UpdateSessionVinst(decimal vinst)
        {
            decimal sessionVinst = GetSessionVinst();
            sessionVinst += vinst;
            HttpContext.Session.SetString("Saldo", sessionVinst.ToString(System.Globalization.CultureInfo.InvariantCulture));
        }

        private decimal CalculateVinst(GameResult result, decimal payout, decimal betAmount)
        {
            // Förlust: payout = 0, result = DealerWin ⇒ vinst = -betAmount
            if (payout == 0 && result == GameResult.DealerWin)
                return -betAmount;
            else
                return payout - betAmount;
        }
    }
}