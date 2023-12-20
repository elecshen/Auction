using Auction.Models.Lots;
using Auction.Models.MSSQLModels;
using Auction.Models.MSSQLModels.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Auction.Controllers
{
    public class LotController(LocalDBContext context) : Controller
    {
        private readonly LocalDBContext _context = context;

        private Lot? CheckLotRequest(int pid, out Guid? userId, out bool isAllowBid)
        {// Проверка существования лота и установка вспомогательных значений для представления
            userId = null;
            isAllowBid = false;
            Lot? lot = _context.Lots.Where(l => l.PublicId == pid).FirstOrDefault();
            if (lot is null)
            {
                return null;
            }
            ViewBag.IsOwner = false;
            var sid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
            if (User.Identity is not null && User.Identity.IsAuthenticated && sid is not null)
            {
                userId = new(sid.Value);
                if (userId == lot.OwnerId)
                    ViewBag.IsOwner = true;
                else
                    isAllowBid = true;
            }
            return lot;
        }

        private LotVM GetLotVM(int pid)
        {
            IQueryable<Lot> lotsFilter = _context.Lots.AsQueryable();
            lotsFilter = lotsFilter.Where(l => l.PublicId == pid);
            LotVM filtredLot = lotsFilter.Select(l => new LotVM()
            {
                PublicId = l.PublicId,
                CategoryName = l.Category.Name,
                Title = l.Title,
                StartPrice = l.StartPrice,
                LastBid = l.LastBid,
                PriceStep = l.PriceStep,
                BlitzPrice = l.BlitzPrice,
                StatusName = l.Status.Name,
                IsClosed = l.IsClosed,
                StartDate = l.StartDate,
                ExpiresOn = l.ExpiresOn,
                Description = l.Description,
                Bids = l.Bids.Select(b => new BidVM()
                {
                    BidderName = b.Bidder.Username,
                    Value = b.Value,
                    BidDate = b.BidDate,
                }).OrderBy(b => b.BidDate).ToList(),
            }).First();
            return filtredLot;
        }

        [HttpGet]
        public ActionResult Index(int pid)
        {
            Lot? lot = CheckLotRequest(pid, out Guid? userId, out bool isAllowBid);
            if (lot is null)
            {
                return NotFound();
            }
            LotVM filtredLot = GetLotVM(pid);
            return View(filtredLot);
        }

        private string? IsBidCorrect(Lot lot, DateTime bidDate, int bid)
        {
            if (lot.StartDate >= bidDate || bidDate >= lot.ExpiresOn)
                return "Время вышло";
            int currentPrice = lot.LastBid is not null ? lot.LastBid.Value + lot.PriceStep : lot.StartPrice;
            if (bid >= currentPrice && bid <= lot.BlitzPrice && (bid - lot.StartPrice) % lot.PriceStep == 0)
                return null;
            return "Неверное значение ставки";
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Index(int pid, int bid)
        {
            DateTime bidDate = DateTime.Now;

            Lot? lot = CheckLotRequest(pid, out Guid? userId, out bool isAllowBid);
            if (lot is null)
            {
                return NotFound();
            }

            // Проверка коректности ставки и её фиксация
            ViewBag.AddBidError = IsBidCorrect(lot, bidDate, bid);
            if (isAllowBid && !lot.IsClosed && ViewBag.AddBidError is null)
            {
                Bid newBid = new()
                {
                    Value = bid,
                    BidDate = bidDate,
                    BidderId = userId!.Value,
                    LotId = lot.Id,
                };
                _context.Bids.Add(newBid);
                lot.LastBid = newBid;
                _context.SaveChanges();
                // CheckstatusService
                ViewBag.AddBidError = "";
            }
            ViewBag.AddBidError ??= "Не удалось сделать ставку";


            LotVM filtredLot = GetLotVM(pid);
            return View(filtredLot);
        }

        [Authorize]
        public ActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAsync(CreateLotVM lotVM)
        {
            if (ModelState.IsValid)
            {
                if (lotVM.StartDate <= DateTime.Now)
                    ModelState.AddModelError(nameof(lotVM.StartDate), "Начало ставок не может быть установлено задним числом");
                if (lotVM.ExpiresOn <= DateTime.Now)
                    ModelState.AddModelError(nameof(lotVM.ExpiresOn), "Окончание ставок не может быть установлено задним числом");
                if (lotVM.StartDate >= lotVM.ExpiresOn)
                    ModelState.AddModelError(nameof(lotVM.ExpiresOn), "Окончание ставок не может быть раньше даты начала");
                if (lotVM.StartPrice <= 0)
                    ModelState.AddModelError(nameof(lotVM.StartPrice), "Стартовая цена должна быть положительной");
                if (lotVM.BlitzPrice <= 0)
                    ModelState.AddModelError(nameof(lotVM.BlitzPrice), "Цена \"Купить сейчас\" должна быть положительной");
                if (lotVM.BlitzPrice <= lotVM.StartPrice)
                    ModelState.AddModelError(nameof(lotVM.BlitzPrice), "Цена \"Купить сейчас\" не может быть меньше стартовой");
                if (lotVM.PriceStep < 1)
                    ModelState.AddModelError(nameof(lotVM.PriceStep), "Шаг цены не может быть меньше 1");
                if (lotVM.StartPrice + lotVM.PriceStep > lotVM.BlitzPrice)
                    ModelState.AddModelError(nameof(lotVM.PriceStep), "Шаг цены слишком большой");
                if (ModelState.IsValid)
                {
                    Lot lot = new()
                    {
                        Title = lotVM.Title,
                        Description = lotVM.Description,
                        StartDate = lotVM.StartDate,
                        ExpiresOn = lotVM.ExpiresOn,
                        BlitzPrice = lotVM.BlitzPrice,
                        StartPrice = lotVM.StartPrice,
                        PriceStep = lotVM.PriceStep,
                        CategoryId = lotVM.CategoryId,
                        OwnerId = new Guid(User.Claims.First(c => c.Type == ClaimTypes.Sid).Value),
                    };
                    _context.Add(lot);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), "Lot", new { pid = lot.PublicId });
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View(lotVM);
        }

        // GET: LotController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: LotController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: LotController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: LotController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
