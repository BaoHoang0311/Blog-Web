using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using blog_web.Models;
using PagedList.Core;
using Microsoft.AspNetCore.Http;
using blog_web.Extension;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using blog_web.Data.Extension;
using Microsoft.AspNetCore.Authorization;

namespace blog_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class CategoriesController : Controller
    {
        private readonly blogdbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriesController(blogdbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin/Categories
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 3;
            var lsCategories = _context.Categories.OrderByDescending(x => x.CatId);
            PagedList<Category> models = new PagedList<Category>(lsCategories, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Parent = new SelectList(_context.Categories.Where(x => x.Levels == 1), "CatId", "CatName");
            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {
            ViewBag.Parent = new SelectList(_context.Categories.Where(x => x.Levels == 1), "CatId", "CatName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CatId,CatName,Title,Alias,MetaDesc,MetaKey,Thumb,Published,Ordering,Parent,Levels,Icon,Cover,Description")]
            Category category
            , IFormFile fIcon, IFormFile fCover, IFormFile fThumb)
        {
            if (ModelState.IsValid)
            {
                var rs = await _context.Categories.FirstOrDefaultAsync(m => m.CatName == category.CatName);
                if (rs == null)
                {
                    category.Alias = category.CatName.ToUrlFriendly();

                    if (category.Parent == null) category.Levels = 1;
                    else category.Levels = category.Parent == 0 ? 1 : 2;

                    if (fThumb != null) category.Thumb = await UploadImage(@"images/categories/", fThumb, category.CatName);
                    if (fIcon != null) category.Icon = await UploadImage(@"images/categories/", fIcon, category.CatName + "icon_");
                    if (fCover != null) category.Cover = await UploadImage(@"images/categories/", fCover, category.CatName + "cover_");

                    _context.Add(category);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(category);
            }
            return View(category);
        }
        // tạo đường dẫn lưu vào wwwroot
        private async Task<string> UploadImage(string folderPath, IFormFile file, string fileName)
        {
            string extension = Path.GetExtension(file.FileName);

            folderPath += Utilities.SEOUrl(fileName) + "_preview_" +Guid.NewGuid() + extension;

            string serverFolder = Path.Combine(_webHostEnvironment.WebRootPath, folderPath);

            await file.CopyToAsync(new FileStream(serverFolder, FileMode.Create));

            return "/" + folderPath;
        }
        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Parent = new SelectList(_context.Categories.Where(x => x.Levels == 1), "CatId", "CatName");
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [BindProperty]
        public Category category { get; set; }
        [HttpPost]
        [ValidateAntiForgeryToken]
        
        public async Task<IActionResult> Edit(int id, /*[Bind("CatId,CatName,Title,Alias,MetaDesc,MetaKey,Thumb,Published,Ordering,Parent,Levels,Icon,Cover,Description")]*/
        Category category, IFormFile fIcon, IFormFile fCover, IFormFile fThumb)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var rs = _context.Categories.FirstOrDefault(x => x.CatId == id);
                    rs.CatName = category.CatName;
                    rs.Title = category.Title;
                    rs.Alias = category.CatName.ToUrlFriendly();
                    rs.MetaDesc = category.MetaDesc;
                    rs.MetaKey = category.MetaKey;
                    rs.Published = category.Published;
                    rs.Ordering = category.Ordering;
                    rs.Description = category.Description;
                    rs.Parent = category.Parent;
                    rs.Levels = category.Parent == 0 ? 1 : 2;

                    if (fThumb != null) rs.Thumb = await UploadImage(@"images/categories/", fThumb, category.CatName);
                    if (fIcon != null) rs.Icon = await UploadImage(@"images/categories/", fIcon, category.CatName + "icon_");
                    if (fCover != null) rs.Cover = await UploadImage(@"images/categories/", fCover, category.CatName + "cover_");

                    _context.Update(rs);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CatId))
                    {
                        return View("Not Found");
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ViewBag.Parent = new SelectList(_context.Categories.Where(x => x.Levels == 1), "CatId", "CatName");
            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CatId == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CatId == id);
        }
    }
}
