using Auction.Models.ConstModels;
using Auction.Models.Lots;
using Auction.Models.MSSQLModels;
using Auction.Models.MSSQLModels.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Auction.Controllers
{
    public class LotController(LocalDBContext context) : Controller
    {
        private readonly LocalDBContext _context = context;

        private Lot? GetLot(int pid)
            => _context.Lots
                .Where(l => l.PublicId == pid)
                .Include(l => l.Bids)
                    .ThenInclude(b => b.Bidder)
                .Include(l => l.Category)
                .FirstOrDefault();

        private Guid? GetUserId()
        {
            var sid = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);
            if (User.Identity is not null && User.Identity.IsAuthenticated && sid is not null)
            {
                return new(sid.Value);
            }
            return null;
        }

        private LotVM MakeLotVM(Lot lot, Guid? userId)
        {
            LotVM lotVM = new()
            {
                PublicId = lot.PublicId,
                CategoryName = lot.Category.Name,
                Title = lot.Title,
                Description = lot.Description,
                StartDate = lot.StartDate,
                ExpireDate = lot.StartDate.AddSeconds(lot.Interval),
                BlitzPrice = lot.BlitzPrice,
                IsAuthenticated = userId is not null,
                IsOwner = lot.OwnerId == userId,
                Bids = lot.Bids.Select(b => new BidVM()
                {
                    BidderName = b.Bidder.Username,
                    Value = b.Value,
                    BidDate = b.BidDate,
                }).OrderBy(b => b.BidDate).ToList(),
            };
            CalcLotBidProperties(lot, lotVM);
            return lotVM;
        }

        private void CalcLotBidProperties(Lot lot, LotVM lotVM)
        {
            lotVM.IsCanBid = false;
            if (DateTime.Now < lotVM.ExpireDate && !lot.IsCompleted)
            {
                if (!lotVM.IsOwner)
                    lotVM.IsCanBid = true;
                lotVM.Status = LotStatus.InProgress;
            }
            else
                lotVM.Status = LotStatus.Completed;
            if (lot.LastBid is not null)
            {
                lotVM.CurrentPrice = lot.LastBid.Value;
                lotVM.NextMinPrice = lot.LastBid.Value + 1;
            }
            else
            {
                lotVM.CurrentPrice = lot.StartPrice;
                lotVM.NextMinPrice = lot.StartPrice;
            }
        }

        private IActionResult MakeIndexView(int pid, int? bid)
        {
            Lot? lot = GetLot(pid);
            if (lot is null)
            {
                return NotFound();
            }
            Guid? userId = GetUserId();
            LotVM lotVM = MakeLotVM(lot, userId);
            if (bid is null)
                return View(lotVM);

            // Проверка коректности ставки и её фиксация
            if (lotVM.IsCanBid && !(bid < lotVM.NextMinPrice || bid > lot.BlitzPrice))
            {
                Bid newBid = new()
                {
                    Value = bid.Value,
                    BidDate = DateTime.Now,
                    BidderId = userId!.Value,
                    LotId = lot.Id,
                };

                using var transaction = _context.Database.BeginTransaction();
                _context.Bids.Add(newBid);
                lot.LastBid = newBid;
                if (lot.BlitzPrice == bid)
                    lot.IsCompleted = true; // Статус: Завершён

                try
                {
                    _context.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                }

                lotVM.Bids.Add(new BidVM()
                {
                    BidderName = User.Claims.First(c => c.Type == ClaimTypes.Name).Value,
                    Value = newBid.Value,
                    BidDate = newBid.BidDate,
                });
                CalcLotBidProperties(lot, lotVM);

                ViewBag.AddBidError = "";
            }
            ViewBag.AddBidError ??= "Не удалось сделать ставку";

            return View(lotVM);
        }

        [HttpGet]
        public IActionResult Index(int pid)
        {
            return MakeIndexView(pid, null);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public IActionResult Index(int pid, int bid)
        {
            return MakeIndexView(pid, bid);
        }

        [Authorize]
        public IActionResult Create()
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
                if (lotVM.BlitzPrice <= lotVM.StartPrice)
                    ModelState.AddModelError(nameof(lotVM.BlitzPrice), "Цена \"Купить сейчас\" должна быть больше стартовой");
                if (ModelState.IsValid)
                {
                    Lot lot = new()
                    {
                        Title = lotVM.Title,
                        Description = lotVM.Description,
                        StartDate = DateTime.Now,
                        Interval = (int)new TimeSpan(lotVM.Days, lotVM.Hours, lotVM.Minutes, lotVM.Seconds).TotalSeconds,
                        BlitzPrice = lotVM.BlitzPrice,
                        StartPrice = lotVM.StartPrice,
                        OwnerId = new Guid(User.Claims.First(c => c.Type == ClaimTypes.Sid).Value),
                        CategoryId = lotVM.CategoryId,
                    };
                    _context.Add(lot);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), "Lot", new { pid = lot.PublicId });
                }
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View(lotVM);
        }

        [Authorize]
        public IActionResult Edit(int pid)
        {
            Guid userId = new(User.Claims.First(c => c.Type == ClaimTypes.Sid).Value);

            var query = _context.Lots.Where(l => l.PublicId == pid);
            if (!User.IsInRole("Admin")) // Редактировать может либо администратор
                query.Where(l => l.OwnerId == userId); // Либо владелец
            var lot = query.FirstOrDefault();
            if (lot is null)
            {
                return NotFound();
            }

            TimeSpan interval = TimeSpan.FromSeconds(lot.Interval);

            ViewBag.IsCanEdit = DateTime.Now < lot.StartDate + interval && !lot.IsCompleted;
            if (ViewBag.IsCanEdit)
            {
                EditLotVM lotVM = new()
                {
                    Pid = pid,
                    CategoryId = lot.CategoryId,
                    Title = lot.Title,
                    Description = lot.Description,
                    StartDate = lot.StartDate,
                    Days = interval.Days,
                    Hours = interval.Hours,
                    Minutes = interval.Minutes,
                    Seconds = interval.Seconds,
                    StartPrice = lot.StartPrice,
                    BlitzPrice = lot.BlitzPrice,
                };

                ViewBag.IsActive = !lot.IsCompleted;
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", lot.CategoryId);
                return View(lotVM);
            }
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAsync(int pid, EditLotVM lotVM)
        {
            Guid userId = new(User.Claims.First(c => c.Type == ClaimTypes.Sid).Value);

            var query = _context.Lots.Where(l => l.PublicId == pid);
            if (!User.IsInRole("Admin")) // Редактировать может либо администратор
                query.Where(l => l.OwnerId == userId); // Либо владелец
            var lot = query.FirstOrDefault();
            if (lot is null)
            {
                return NotFound();
            }

            TimeSpan interval = TimeSpan.FromSeconds(lot.Interval);
            TimeSpan newInterval = new(lotVM.Days, lotVM.Hours, lotVM.Minutes, lotVM.Seconds);

            ViewBag.IsCanEdit = DateTime.Now < lot.StartDate + interval && !lot.IsCompleted;
            ViewBag.IsActive = !lot.IsCompleted;

            if (ViewBag.IsCanEdit)
            {
                if (ModelState.IsValid)
                {
                    if (newInterval < interval)
                        ModelState.AddModelError(nameof(lotVM.Days), "Продолжительность торгов не может быть уменьшена");
                    if (lotVM.BlitzPrice < lot.BlitzPrice)
                        ModelState.AddModelError(nameof(lotVM.BlitzPrice), "Цена \"Купить сейчас\" не может быть уменьшена");
                    if (lotVM.BlitzPrice <= lotVM.StartPrice)
                        ModelState.AddModelError(nameof(lotVM.BlitzPrice), "Цена \"Купить сейчас\" должна быть больше стартовой");

                    if (ModelState.IsValid)
                    {
                        if (ViewBag.IsActive)
                        {
                            lot.Description = lotVM.Description;
                        }
                        lot.Interval = (int)newInterval.TotalSeconds;
                        lot.BlitzPrice = lotVM.BlitzPrice;

                        _context.Update(lot);
                        _context.Entry(lot).Property(l => l.PublicId).IsModified = false;
                        await _context.SaveChangesAsync();
                        return RedirectToAction(nameof(Index), "Lot", new { pid = lot.PublicId });
                    }
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", lot.CategoryId);
            }
            return View(lotVM);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAsync(int pid)
        {
            Guid userId = new(User.Claims.First(c => c.Type == ClaimTypes.Sid).Value);

            var query = _context.Lots.Where(l => l.PublicId == pid);
            if (!User.IsInRole("Admin")) // Удалять может либо администратор
                query.Where(l => l.OwnerId == userId); // Либо владелец
            var lot = query.FirstOrDefault();
            if (lot is null)
            {
                return NotFound();
            }

            _context.Remove(lot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), "Home");
        }
    }
}
