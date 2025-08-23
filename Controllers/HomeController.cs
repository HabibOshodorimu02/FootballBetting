using Microsoft.AspNetCore.Mvc;
using FootballBetting.Models;
using FootballBetting.Data;
using Microsoft.EntityFrameworkCore;
using FootballBetting.Models.ViewModels;

namespace FootballBetting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Show all matches
        public async Task<IActionResult> Index()
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Predictions)
                .AsNoTracking()
                .OrderBy(m => m.MatchDate)
                .ToListAsync();

            return View(matches);
        }

        // Show leaderboard
        public async Task<IActionResult> Leaderboard()
        {
            var users = await _context.Users
                .AsNoTracking()
                .OrderByDescending(u => u.Points)
                .ThenBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ToListAsync();

            var leaderboard = users.Select((u, index) => new LeaderboardViewModel
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Points = u.Points,
                NumberOfPredictions = u.NumberOfPredictions,
                Position = index + 1
            }).ToList();

            return View(leaderboard);
        }

        // ✅ Prediction entry (GET)
        [HttpGet]
        public async Task<IActionResult> Predict(int matchId)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null) return NotFound();

            var vm = new PredictionViewModel
            {
                MatchId = match.Id,
                HomeTeam = match.HomeTeam.Name,
                AwayTeam = match.AwayTeam.Name,
                MatchDate = match.MatchDate
            };

            return View(vm);
        }

        // ✅ Prediction submission (POST)
        [HttpPost]
        public async Task<IActionResult> Predict(PredictionViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);

            // TODO: Replace with logged-in user’s ID
            int userId = 1;

            // Check if user already predicted this match
            var existing = await _context.Predictions
                .FirstOrDefaultAsync(p => p.UserId == userId && p.MatchId == vm.MatchId);

            if (existing != null)
            {
                ModelState.AddModelError("", "You already made a prediction for this match.");
                return View(vm);
            }

            var prediction = new Prediction
            {
                UserId = userId,
                MatchId = vm.MatchId,
                PredictedOutcome = vm.PredictedOutcome,
                PredictedHomeScore = vm.PredictedHomeScore,
                PredictedAwayScore = vm.PredictedAwayScore,
                PredictionDate = DateTime.Now
            };

            _context.Predictions.Add(prediction);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
