using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Manage.ViewModels;
using Pustok.DAL;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class GenreController : Controller
    {
        private readonly PustokDbContext _context;

        public GenreController(PustokDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1,string search=null)
        {
            ViewBag.Search = search;

            var query = _context.Genres.Include(x => x.Books).AsQueryable();

            if(search!=null) query = query.Where(x=>x.Name.Contains(search));

            return View(PaginatedList<Genre>.Create(query, page, 2));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Genre genre)
        {
            if(!ModelState.IsValid)
                return View();

            if(_context.Genres.Any(x=>x.Name == genre.Name))
            {
                ModelState.AddModelError("Name", "Name is already taken");
                return View();
            }

            _context.Genres.Add(genre);

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Genre genre = _context.Genres.FirstOrDefault(x=>x.Id == id);

            if (genre == null) return View("error");


            return View(genre);
        }

        [HttpPost]
        public IActionResult Edit(Genre genre)
        {
            if(!ModelState.IsValid)
                return View();

            Genre existGenre = _context.Genres.FirstOrDefault(x => x.Id == genre.Id);

            if (existGenre == null) return View("error");

            if (genre.Name!=existGenre.Name &&  _context.Genres.Any(x=> x.Name==genre.Name))
            {
                ModelState.AddModelError("Name", "Name is already taken");
                return View();
            }


            existGenre.Name = genre.Name;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Genre genre = _context.Genres.Find(id);

            if (genre == null) return StatusCode(404);

            if (_context.Books.Any(x => x.GenreId == id)) return StatusCode(400);

            _context.Genres.Remove(genre);
            _context.SaveChanges();

            return StatusCode(200);
        }
    }
}
