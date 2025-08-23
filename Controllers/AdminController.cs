using Microsoft.AspNetCore.Mvc;
using FootballBetting.Models;
using FootballBetting.Data;
using Microsoft.EntityFrameworkCore;

namespace FootballBetting.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var matches = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Include(m => m.Predictions)
                .OrderBy(m => m.MatchDate)
                .ToListAsync();

            return View(matches);
        }

        public async Task<IActionResult> UpdateResult(int id)
        {
            var match = await _context.Matches
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (match == null)
            {
                return NotFound();
            }

            return View(match);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateResult(int id, int homeScore, int awayScore)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            match.HomeScore = homeScore;
            match.AwayScore = awayScore;
            match.IsCompleted = true;

            _context.Update(match);
            await _context.SaveChangesAsync();

            // Process predictions for this match
            await ProcessPredictions(id);

            TempData["Success"] = "Match result updated and predictions processed!";
            return RedirectToAction("Index");
        }

        private async Task ProcessPredictions(int matchId)
        {
            var match = await _context.Matches.FindAsync(matchId);
            if (match == null) return;

            var predictions = await _context.Predictions
                .Include(p => p.User)
                .Where(p => p.MatchId == matchId && !p.IsProcessed)
                .ToListAsync();

            foreach (var prediction in predictions)
            {
                int points = 0;

                // Determine actual result
                string actualResult;
                if (match.HomeScore > match.AwayScore)
                    actualResult = "Home Win";
                else if (match.AwayScore > match.HomeScore)
                    actualResult = "Away Win";
                else
                    actualResult = "Draw";

                // Check outcome prediction
                if (prediction.PredictedOutcome == actualResult)
                {
                    switch (actualResult)
                    {
                        case "Home Win":
                            points += 2;
                            break;
                        case "Away Win":
                            points += 3;
                            break;
                        case "Draw":
                            points += 5;
                            break;
                    }
                }

                // Check correct score (10 points)
                if (prediction.PredictedHomeScore == match.HomeScore &&
                    prediction.PredictedAwayScore == match.AwayScore)
                {
                    points += 10;
                }

                prediction.PointsEarned = points;
                prediction.IsProcessed = true;

                // Update user points
                prediction.User.Points += points;

                // Update fact table
                var factTable = await _context.FactTables.FirstOrDefaultAsync(f => f.UserId == prediction.UserId);
                if (factTable != null)
                {
                    factTable.TotalPredictions++;

                    if (points > 0)
                        factTable.Wins++;
                    else
                        factTable.Losses++;

                    if (actualResult == "Draw" && prediction.PredictedOutcome == "Draw")
                        factTable.Draws++;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}