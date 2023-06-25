using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Packaging.Signing;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;
using System.Security.Claims;

namespace Pustok.Controllers
{
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;

        public BookController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult GetDetail(int id)
        {
            Book book = _context.Books.Include(x=>x.BookImages).FirstOrDefault(x => x.Id == id);
            //return Json(book);
            return PartialView("_BookModalPartial", book);
        }

        public IActionResult AddToBasket(int id)
        {
            BasketViewModel basketVM = new BasketViewModel();
            if (User.Identity.IsAuthenticated)
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var basketItems = _context.BasketItems.Where(x=>x.AppUserId== userId).ToList();

                var basketItem = basketItems.FirstOrDefault(x => x.BookId == id);

                if (basketItem == null)
                {
                    basketItem = new BasketItem
                    {
                        BookId = id,
                        Count = 1,
                        AppUserId = userId,
                    };
                    _context.BasketItems.Add(basketItem);
                }
                else
                    basketItem.Count++;

                _context.SaveChanges();

                var items = _context.BasketItems.Include(x=>x.Book).ThenInclude(x=>x.BookImages.Where(bi=>bi.PosterStatus==true)).Where(x => x.AppUserId == userId).ToList();

                foreach (var bi in items)
                {
                    BasketItemViewModel item = new BasketItemViewModel
                    {
                        Count = bi.Count,
                        Book = bi.Book,
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Book.DiscountPercent > 0 ? item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100 : item.Book.SalePrice) * item.Count;
                }
            }
            else
            {
                var basketStr = Request.Cookies["basket"];

                List<BasketCookieItemViewModel> cookieItems = null;

                if (basketStr == null)
                    cookieItems = new List<BasketCookieItemViewModel>();
                else
                    cookieItems = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(basketStr);


                BasketCookieItemViewModel cookieItem = cookieItems.FirstOrDefault(x => x.BookId == id);
                if (cookieItem == null)
                {
                    cookieItem = new BasketCookieItemViewModel
                    {
                        BookId = id,
                        Count = 1
                    };
                    cookieItems.Add(cookieItem);
                }
                else
                    cookieItem.Count++;

                HttpContext.Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

               
                foreach (var ci in cookieItems)
                {
                    BasketItemViewModel item = new BasketItemViewModel
                    {
                        Count = ci.Count,
                        Book = _context.Books.Include(x => x.BookImages.Where(x => x.PosterStatus == true)).FirstOrDefault(x => x.Id == ci.BookId)
                    };
                    basketVM.Items.Add(item);
                    basketVM.TotalAmount += (item.Book.DiscountPercent > 0 ? item.Book.SalePrice * (100 - item.Book.DiscountPercent) / 100 : item.Book.SalePrice) * item.Count;
                }
            }
          


            return PartialView("_BasketPartial", basketVM);
        }

        public IActionResult ShowBasket()
        {
            var dataStr = HttpContext.Request.Cookies["basket"];
            var data = JsonConvert.DeserializeObject<List<BasketCookieItemViewModel>>(dataStr);
            return Json(data);
        }
    }
}
