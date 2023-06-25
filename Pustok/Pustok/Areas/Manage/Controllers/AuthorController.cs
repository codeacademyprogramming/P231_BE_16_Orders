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
    public class AuthorController : Controller
    {
        private readonly PustokDbContext _context;

        public AuthorController(PustokDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(int page=1)
        {
            var query = _context.Authors.Include(x => x.Books);

            return View(PaginatedList<Author>.Create(query,page,2));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Author author)
        {
            if(!ModelState.IsValid) return View();

            _context.Authors.Add(author);
            _context.SaveChanges();

            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.Find(id);

            if (author == null) return View("error");

            return View(author);
        }

        [HttpPost]
        public IActionResult Edit(Author author)
        {
            if(!ModelState.IsValid) return View();

            Author existAuthor = _context.Authors.Find(author.Id);

            if (author == null) return View("error");

            existAuthor.FullName = author.FullName;
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Author author = _context.Authors.Include(x => x.Books).FirstOrDefault(x => x.Id == id);
            if (author == null || _context.Books.Any(x => x.AuthorId == author.Id)) return View("Error");
            return View(author);
        }

        [HttpPost]
        public IActionResult Delete(Author author)
        {
            Author existAuthor = _context.Authors.Find(author.Id);
            if (existAuthor == null || _context.Books.Any(x=>x.AuthorId==author.Id)) return View("Error");

            _context.Authors.Remove(existAuthor);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

    }
}
