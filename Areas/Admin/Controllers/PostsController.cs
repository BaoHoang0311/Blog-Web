using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using blog_web.Models;
using PagedList.Core;
using blog_web.Extension;
using Microsoft.AspNetCore.Http;
using blog_web.Data.Extension;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace blog_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class PostsController : Controller
    {
        private readonly blogdbContext _context;
        private readonly Saveimage save;

        public PostsController(blogdbContext context, Saveimage _save)
        {
            _context = context;
            save = _save;
        }
        // GET: Admin/Posts
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 3;
            var lsPost = _context.Posts.Include(p => p.Account)
                .Include(p => p.Cat)
                .OrderByDescending(x => x.PostId);
            PagedList<Post> models = new PagedList<Post>(lsPost, pageNumber, pageSize);
            return View(models);
        }

        // GET: Admin/Posts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // GET: Admin/Posts/Create
        public IActionResult Create()
        {
            // phải đăng nhập để đăng bài
            if (!User.Identity.IsAuthenticated) 
                RedirectToAction("Login", "Accounts", new { Areas = "Admin"});
            var taikhoanID = HttpContext.Session.GetString("id_tai_khoan");
            if(taikhoanID ==null) return RedirectToAction("Login", "Accounts", new { Areas = "Admin" });
            ViewBag.Categories = new SelectList(_context.Categories, "CatId", "CatName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("PostId,Title,ShortContent,Contents,Thumb,Published,Alias" +
            ",CreatedAt,Author,Tags,IsHot,IsNewFeed,AccountId,CatId")]
            Post post
            , IFormFile fThumb)
        {
            // phải đăng nhập để đăng bài
            if (!User.Identity.IsAuthenticated) RedirectToAction("Login", "Accounts", new { Areas = "Admin" });
            var taikhoan_id = HttpContext.Session.GetString("id_tai_khoan");
            if (taikhoan_id == null) return RedirectToAction("Login", "Accounts", new { Areas = "Admin" });
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(taikhoan_id));
            if (taikhoan == null) return NotFound();

            if (ModelState.IsValid)
            {
                post.AccountId = int.Parse(taikhoan_id);
                post.Author = taikhoan.FullName;
                post.Alias = post.Title.ToUrlFriendly();
                post.Thumb = await save.UploadImage(@"images/Post/Thumb/", fThumb, post.Title);
                if (post.CreatedAt == null) post.CreatedAt = DateTime.Now;
                _context.Add(post);
                await _context.SaveChangesAsync();
      
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Acount = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
            ViewBag.Categories = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            // phải đăng nhập mới chỉnh sửa
            if (!User.Identity.IsAuthenticated) RedirectToAction("Login", "Accounts", new { Areas = "Admin" });

            var taikhoan_id = HttpContext.Session.GetString("id_tai_khoan");
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(taikhoan_id));

            if (taikhoan == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);

            // chỉ sửa bài của mình khác id ko cho sửa,admin sửa dc hết
            if (post.AccountId != taikhoan.AccountId && User.FindFirstValue(ClaimTypes.Role) != "Admin") return RedirectToAction(nameof(Index));

            if (post == null)
            {
                return NotFound();
            }
            ViewBag.Acount = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
            ViewBag.Categories = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,ShortContent,Contents," +
            "Thumb,Published,Alias,CreatedAt,Author,IsHot,IsNewFeed,AccountId,CatId")]
        Post post, IFormFile fThumb)
        {
            if (id != post.PostId)
            {
                return NotFound();
            }

            if (!User.Identity.IsAuthenticated) RedirectToAction("Login", "Accounts", new { Areas = "Admin" });

            var taikhoan_id = HttpContext.Session.GetString("id_tai_khoan");
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(taikhoan_id));

            if (taikhoan == null) return NotFound();

            // chỉ sửa bài của mình khác id ko cho sửa,admin sửa dc hết
            if (post.AccountId != taikhoan.AccountId && User.FindFirstValue(ClaimTypes.Role) != "Admin") return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                try
                {
                    post.AccountId = int.Parse(taikhoan_id);
                    post.Author = taikhoan.FullName;
                    post.Alias = post.Title.ToUrlFriendly();
                    if(fThumb != null) post.Thumb = await save.UploadImage(@"images/Post/Thumb/", fThumb, post.Title);
                    if (post.CreatedAt == null) post.CreatedAt = DateTime.Now;

                    _context.Update(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PostExists(post.PostId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            ViewBag.Acount = new SelectList(_context.Accounts, "AccountId", "FullName", post.AccountId);
            ViewBag.Categories = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            return View(post);
        }

        // GET: Admin/Posts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var post = await _context.Posts
                .Include(p => p.Account)
                .Include(p => p.Cat)
                .FirstOrDefaultAsync(m => m.PostId == id);
            ViewBag.Categories = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);
            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: Admin/Posts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var post = await _context.Posts.FindAsync(id);

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PostExists(int id)
        {
            return _context.Posts.Any(e => e.PostId == id);
        }
    }
}
