using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.DAL;
using Pustok.Models;
using Pustok.ViewModels;
using System.Diagnostics;

namespace Pustok.Controllers
{
    public class HomeController : Controller
    {
        private readonly PustokDbContext _context;

        public HomeController(PustokDbContext context)
        {
            _context = context;
        }

      
        public IActionResult Index()
        {
            HomeViewModel vm = new HomeViewModel
            {
                Sliders = _context.Sliders.OrderBy(x=>x.Order).ToList(),
                Features = _context.Features.Take(4).ToList(),
                FeaturedBooks = _context.Books
                                    .Include(x=>x.Author).Include(x=>x.BookImages.Where(x=>x.PosterStatus!=null))
                                    .Where(x=>x.IsFeatured).Take(20).ToList(),
                 NewBooks = _context.Books
                                    .Include(x => x.Author).Include(x => x.BookImages.Where(x => x.PosterStatus != null))
                                    .Where(x => x.IsNew).Take(20).ToList(),
                DiscountedBooks = _context.Books
                                    .Include(x => x.Author).Include(x => x.BookImages.Where(x => x.PosterStatus != null))
                                    .Where(x => x.DiscountPercent>0).Take(20).ToList()
            };
            return View(vm);
        }

        public IActionResult SetSession(string name)
        {
            HttpContext.Session.SetString("name", name);
            return Content("");
        }

        public IActionResult GetSession(string key)
        {
            var data = HttpContext.Session.GetString(key);
            return Content(data);
        }

        public IActionResult SetCookie(string name)
        {
            HttpContext.Response.Cookies.Append("name", name);
            return Content("");
        }

        public IActionResult GetCookie(string key)
        {
            var data = HttpContext.Request.Cookies[key];
            return Content(data);
        }
        public IActionResult RemoveCookie(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
            return Content("");
        }
    }
}