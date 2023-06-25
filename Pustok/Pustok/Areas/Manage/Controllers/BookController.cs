using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pustok.Areas.Manage.ViewModels;
using Pustok.DAL;
using Pustok.Helpers;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BookController(PustokDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page = 1)
        {
            var query = _context.Books.Include(x => x.Author).Include(x => x.Genre).Include(x => x.BookImages.Where(bi => bi.PosterStatus == true)).Where(x=>!x.IsDeleted);
            return View(PaginatedList<Book>.Create(query, page, 4));
        }

        public IActionResult Create()
        {
            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();


            return View();
        }

        [HttpPost]
        public IActionResult Create(Book book)
        {
            if (book.PosterFile == null)
                ModelState.AddModelError("PosterFile", "PosterFile is required");

            if (book.HoverPosterFile == null)
                ModelState.AddModelError("HoverPosterFile", "HoverPosterFile is required");

            if (!ModelState.IsValid)
            {
                ViewBag.Authors = _context.Authors.ToList();
                ViewBag.Genres = _context.Genres.ToList();
                return View();
            }

            if (!_context.Authors.Any(x => x.Id == book.AuthorId))
                return View("error");

            if (!_context.Genres.Any(x => x.Id == book.GenreId))
                return View("error");


            BookImage poster = new BookImage
            {
                PosterStatus = true,
                ImageName = FileManager.Save(book.PosterFile, _env.WebRootPath, "manage/uploads/books"),
            };
            book.BookImages.Add(poster);

            BookImage hoverPoster = new BookImage
            {
                PosterStatus = false,
                ImageName = FileManager.Save(book.HoverPosterFile, _env.WebRootPath, "manage/uploads/books"),
            };
            book.BookImages.Add(hoverPoster);


            foreach (var file in book.ImageFiles)
            {
                BookImage bookImage = new BookImage
                {
                    PosterStatus = null,
                    ImageName = FileManager.Save(file, _env.WebRootPath, "manage/uploads/books"),
                };
                book.BookImages.Add(bookImage);
            }

            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId))
                    return View("error");

                BookTag tag = new BookTag
                {
                    TagId = tagId
                };

                book.BookTags.Add(tag);
            }

            _context.Books.Add(book);

            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {


            Book book = _context.Books.Include(x => x.BookImages).Include(x => x.BookTags).FirstOrDefault(x => x.Id == id);

            if (book == null) return View("error");

            book.TagIds = book.BookTags.Select(x => x.TagId).ToList();

            ViewBag.Authors = _context.Authors.ToList();
            ViewBag.Genres = _context.Genres.ToList();
            ViewBag.Tags = _context.Tags.ToList();

            return View(book);
        }

        [HttpPost]
        public IActionResult Edit(Book book)
        {
            Book existBook = _context.Books.Include(x => x.BookImages).Include(x => x.BookTags).FirstOrDefault(x => x.Id == book.Id);
            if (existBook == null) return View("Error");

            if (!_context.Authors.Any(x => x.Id == book.AuthorId)) return View("Error");

            if (!_context.Genres.Any(x => x.Id == book.GenreId)) return View("Error");

            existBook.BookTags = new List<BookTag>();

            foreach (var tagId in book.TagIds)
            {
                if (!_context.Tags.Any(x => x.Id == tagId))
                    return View("error");

                existBook.BookTags.Add(new BookTag { TagId = tagId });
            }

           

            existBook.Name = book.Name;
            existBook.Desc = book.Desc;
            existBook.SalePrice = book.SalePrice;
            existBook.CostPrice = book.CostPrice;
            existBook.DiscountPercent = book.DiscountPercent;
            existBook.IsFeatured = book.IsFeatured;
            existBook.IsNew = book.IsNew;
            existBook.StockStatus = book.StockStatus;
            existBook.AuthorId = book.AuthorId;
            existBook.GenreId = book.GenreId;

            List<string> removableImageNames = new List<string>();

            if (book.PosterFile != null)
            {
                BookImage poster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == true);
                removableImageNames.Add(poster.ImageName);
                poster.ImageName = FileManager.Save(book.PosterFile, _env.WebRootPath, "manage/uploads/books");
            }

            if (book.HoverPosterFile != null)
            {
                BookImage hoverPoster = existBook.BookImages.FirstOrDefault(x => x.PosterStatus == false);
                removableImageNames.Add(hoverPoster.ImageName);
                hoverPoster.ImageName = FileManager.Save(book.HoverPosterFile, _env.WebRootPath, "manage/uploads/books");
            }

            var removableImages = existBook.BookImages.FindAll(x =>x.PosterStatus==null && !book.BookImageIds.Contains(x.Id));
            _context.BookImages.RemoveRange(removableImages);

            //foreach (var rf in removableImages) removableImageNames.Add(rf.ImageName);
            removableImageNames.AddRange(removableImages.Select(x => x.ImageName).ToList());


            foreach (var file in book.ImageFiles)
            {
                BookImage image = new BookImage
                {
                    PosterStatus = null,
                    ImageName = FileManager.Save(file, _env.WebRootPath, "manage/uploads/books")
                };

                existBook.BookImages.Add(image);
            }

            _context.SaveChanges();
            FileManager.DeleteAll(_env.WebRootPath, "manage/uploads/books", removableImageNames);


            return RedirectToAction("index");
        }

        public IActionResult Delete(int id)
        {
            Book book = _context.Books.Find(id);

            if (book == null) return NotFound();

            book.IsDeleted = true;
            _context.SaveChanges();

            return Ok();
        }
    }
}
