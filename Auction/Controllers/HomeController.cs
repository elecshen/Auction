using Auction.Models;
using Auction.Models.Home;
using Auction.Models.MSSQLModels;
using Auction.Models.MSSQLModels.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Cryptography;
using static Auction.Models.ConstModels.CoockieEnums;

namespace Auction.Controllers
{
    public class HomeController(ILogger<HomeController> logger, LocalDBContext context) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly LocalDBContext _context = context;

        public IActionResult Index(string? search, int[] statusFilter, int[] categoryFilter)
        {
            // Подготавливаем запрос на выборку данных о лотах
            IQueryable<Lot> lotsFilter = _context.Lots.AsQueryable();
            // Получаем поисковую строку и фильтруем по ней
            ViewBag.Search = search;
            bool isDefault = search is null;
            if (!isDefault)
            {
                lotsFilter = lotsFilter.Where(l => EF.Functions.Like(l.Title, string.Format("%{0}%", search!)));
            }
            // Получение списка статусов и фильтрация по выбранным
            List<Status> statuses = _context.Statuses.ToList();
            statuses.ForEach(s =>
            {
                if (!isDefault)
                    s.IsSetByDefault = statusFilter.Contains(s.Id);
            });
            ViewBag.StatusFilterList = new
            {
                ParamName = "StatusFilter",
                List = statuses.Select(s => new SelectListItem(s.Name, s.Id.ToString(), s.IsSetByDefault)),
            };
            if (!(statusFilter.Length == 0 || statusFilter.Length == statuses.Count) || isDefault)
            {
                var activeStatuses = statuses.Where(s => s.IsSetByDefault).Select(s => s.Id).ToArray();
                lotsFilter = lotsFilter.Where(l => activeStatuses.Contains(l.StatusId));
            }
            // Получение списка категорий и фильтрация по выбранной
            List<Category> categories = _context.Categories.ToList();
            var categoriesSelectList = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString(), categoryFilter.Contains(c.Id)));
            ViewBag.CategoryFilterList = new
            {
                ParamName = "CategoryFilter",
                List = categoriesSelectList,
            };
            if (!(categoryFilter.Length == 0 || categoryFilter.Length == categories.Count))
            {
                var activeCategories = categories.Where(c => categoryFilter.Contains(c.Id)).Select(c => c.Id);
                lotsFilter = lotsFilter.Where(l => activeCategories.Contains(l.CategoryId));
            }

            var filtredLots = lotsFilter.Select(l => new LotCardVM()
            {
                PublicId = l.PublicId,
                StartPrice = l.StartPrice,
                LastBid = l.LastBid,
                BlitzPrice = l.BlitzPrice,
                Title = l.Title,
                StatusName = l.Status.Name,
                StartDate = l.StartDate,
                ExpiresOn = l.ExpiresOn,
                CategoryName = l.Category.Name,
            }).OrderBy(l => l.StartDate);

            ViewData["Theme"] = Theme.Light;

            // Исполняем запрос со всеми применёнными фильтрациями и передаём его как модель
            return View(filtredLots.ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
