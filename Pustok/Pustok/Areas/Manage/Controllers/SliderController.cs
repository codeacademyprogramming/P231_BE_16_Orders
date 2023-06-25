using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pustok.Areas.Manage.ViewModels;
using Pustok.DAL;
using Pustok.Helpers;
using Pustok.Models;

namespace Pustok.Areas.Manage.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    [Area("manage")]
    public class SliderController : Controller
    {
        private readonly PustokDbContext _context;
        private readonly IWebHostEnvironment _env;

        public SliderController(PustokDbContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {
            var query = _context.Sliders.AsQueryable();
            return View(PaginatedList<Slider>.Create(query,page,2));
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Slider slider)
        {
            if (!ModelState.IsValid) return View();

            if (slider.ImageFile == null)
            {
                ModelState.AddModelError("ImageFile", "ImageFile is required");
                return View();
            }

            if(slider.ImageFile.ContentType!="image/jpeg" && slider.ImageFile.ContentType != "image/png")
            {
                ModelState.AddModelError("ImageFile", "ImageFile must be .jpg,.jpeg or .png");
                return View();
            }
          
            slider.ImageName = FileManager.Save(slider.ImageFile,_env.WebRootPath,"manage/uploads/sliders");

            _context.Sliders.Add(slider);
            _context.SaveChanges();

            return RedirectToAction("index");
        }

        public IActionResult Edit(int id)
        {
            Slider slider = _context.Sliders.Find(id);

            if (slider == null) return View("Error");

            return View(slider);
        }

        [HttpPost]
        public IActionResult Edit(Slider slider)
        {
            if (!ModelState.IsValid) { return View(slider); }

            Slider existSlider = _context.Sliders.Find(slider.Id);

            if (existSlider == null) return View("Error");


            string removableImageName = null;

            if (slider.ImageFile != null)
            {
                if (slider.ImageFile.ContentType != "image/jpeg" && slider.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "ImageFile must be .jpg,.jpeg or .png");
                    return View(slider);
                }

                removableImageName = existSlider.ImageName;
                existSlider.ImageName = FileManager.Save(slider.ImageFile, _env.WebRootPath, "manage/uploads/sliders");
            }

            existSlider.Title1 = slider.Title1;
            existSlider.Title2 = slider.Title2;
            existSlider.Desc = slider.Desc;
            existSlider.BtnText = slider.BtnText;
            existSlider.BtnUrl = slider.BtnUrl;
            existSlider.Order = slider.Order;

            _context.SaveChanges();

            if(removableImageName!=null)FileManager.Delete(_env.WebRootPath, "manage/uploads/sliders", removableImageName);

            return RedirectToAction("index");
        }
    }
}
