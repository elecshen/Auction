using Auction.Models;
using Auction.Models.Home;
using Auction.Models.MSSQLModels;
using Auction.Models.MSSQLModels.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static Auction.Models.ConstModels.CoockieEnums;

namespace Auction.Controllers
{
    public class HomeController(ILogger<HomeController> logger, LocalDBContext context) : Controller
    {
        private readonly ILogger<HomeController> _logger = logger;
        private readonly LocalDBContext _context = context;

        public IActionResult Index(string? search, int? categoryFilter, bool? SearchAll)
        {
            // Подготавливаем запрос на выборку данных о лотах
            IQueryable<Lot> lotsFilter = _context.Lots.AsNoTracking();
            // Получаем поисковую строку и фильтруем по ней
            ViewBag.Search = search;
            bool isDefault = search is null;
            if (!isDefault)
            {
                lotsFilter = lotsFilter.Where(l => EF.Functions.Like(l.Title, string.Format("%{0}%", search!)));
            }
            // Фильтрация по завершённости
            if (SearchAll is null || !SearchAll.Value)
            {
                lotsFilter = lotsFilter.Where(l => !l.IsCompleted);
                lotsFilter = lotsFilter.Where(l => l.StartDate.AddSeconds(l.Interval) > DateTime.Now);
            }
            // Получение списка категорий и фильтрация по выбранной
            List<Category> categories = _context.Categories.ToList();
            categories.Insert(0, new() { Id = 0, Name = "Все" });
            var categoriesSelectList = categories.Select(c => new SelectListItem(c.Name, c.Id.ToString(), categoryFilter == c.Id)).ToList();
            ViewBag.CategoryFilterList = new
            {
                PublicName = "Категории",
                ParamName = "CategoryFilter",
                List = categoriesSelectList,
            };
            if (categoryFilter != 0 && categoryFilter is not null)
            {
                lotsFilter = lotsFilter.Where(l => categoryFilter ==  l.CategoryId);
            }
            else
            {
                categoriesSelectList[0].Selected = true;
            }

            lotsFilter = lotsFilter.OrderBy(l => l.StartDate);
            lotsFilter = lotsFilter.Include(l => l.LastBid)
                .Include(l => l.Category);
            lotsFilter = lotsFilter.Skip(0).Take(30);
            // Исполняем запрос со всеми применёнными фильтрациями 
            var filtredLots = lotsFilter.ToList();

            ViewData["Theme"] = Theme.Light;
            return View(filtredLots.Select(l => new LotCardVM(l)).ToList());
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
