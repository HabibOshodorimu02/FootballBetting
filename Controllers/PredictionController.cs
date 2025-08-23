using Microsoft.AspNetCore.Mvc;
using FootballBetting.Models;
using FootballBetting.Data;
using Microsoft.EntityFrameworkCore;
using FootballBetting.Models.ViewModels;

namespace FootballBetting.Controllers
{
    public class PredictionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PredictionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => !m.IsCompleted)
                .OrderBy(m => m.MatchDate)
                .ToListAsync();

            var userPredictions = await _context.Predictions
                .Where(p => p.UserId == userId)
                .ToListAsync();

            ViewBag.UserPredictions = userPredictions;

            return View(matches);
        }

        public async Task<IActionResult> Create(int matchId)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null || match.IsCompleted)
            {
                return NotFound();
            }

            // Check if user already made prediction for this match
            var existingPrediction = await _context.Predictions
                .FirstOrDefaultAsync(p => p.UserId == userId && p.MatchId == matchId);

            if (existingPrediction != null)
            {
                TempData["Error"] = "You have already made a prediction for this match.";
                return RedirectToAction("Index");
            }

            var viewModel = new PredictionViewModel
            {
                MatchId = match.Id,
                HomeTeam = match.HomeTeam.Name,
                AwayTeam = match.AwayTeam.Name,
                MatchDate = match.MatchDate
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PredictionViewModel model)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                var prediction = new Prediction
                {
                    UserId = userId.Value,
                    MatchId = model.MatchId,
                    PredictedOutcome = model.PredictedOutcome,
                    PredictedHomeScore = model.PredictedHomeScore,
                    PredictedAwayScore = model.PredictedAwayScore,
                    PredictionDate = DateTime.Now
                };

                _context.Predictions.Add(prediction);

                // Update user prediction count
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    user.NumberOfPredictions++;
                    _context.Update(user);
                }

                await _context.SaveChangesAsync();

                TempData["Success"] = "Prediction saved successfully!";
                return RedirectToAction("Index");
            }

            // Reload match data for view
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == model.MatchId);

            if (match != null)
            {
                model.HomeTeam = match.HomeTeam.Name;
                model.AwayTeam = match.AwayTeam.Name;
                model.MatchDate = match.MatchDate;
            }

            return View(model);
        }

        public async Task<IActionResult> MyPredictions()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
            {
                return RedirectToAction("Login", "User");
            }

            var predictions = await _context.Predictions
                .Include(p => p.Match)
                .ThenInclude(m => m.HomeTeam)
                .Include(p => p.Match)
                .ThenInclude(m => m.AwayTeam)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PredictionDate)
                .ToListAsync();

            return View(predictions);
        }
    }
}
