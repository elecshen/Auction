using Auction.Models;
using Auction.Models.MSSQLModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using static Auction.Models.ConstModels.CoockieEnums;

namespace Auction.Controllers
{
    public class HomeController(ILogger<HomeController> logger, LocalDBContext context) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly LocalDBContext _context = context;

        public IActionResult Index(string? search, int[] statusFilter, int? categoryFilter)
        {
            ViewBag.Search = search;
            bool isDefault = search is null;
            ViewBag.StatusFilterList = new
            {
                ParamName = "StatusFilter",
                List = _context.Statuses.Select(s => new SelectListItem(s.Name, s.Id.ToString(), isDefault ? s.SetDefault : statusFilter.Contains(s.Id))).ToList(),
            };
            categoryFilter ??= -1;
            ViewBag.CategoryFilterList = new
            {
                ParamName = "CategoryFilter",
                List = _context.Categories.Select(c => new SelectListItem(c.Name, c.Id.ToString(), c.Id == categoryFilter)).ToList(),
            };
            ViewData["Theme"] = Theme.Light;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
