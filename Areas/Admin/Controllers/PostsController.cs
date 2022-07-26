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
    [Authorize()]
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
        public async Task<IActionResult> Index(int? page, int catID = 0)
        {
            var ID_user = User.GetSpecificClaim("Account_Id");
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(ID_user));
            if (taikhoan == null) return NotFound();


            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 20;

            List<Post> lsPost = new List<Post>();

            if (User.IsInRole("Admin"))
            {
                lsPost = _context.Posts.Include(p => p.Account)
                    .Include(p => p.Cat)
                    .OrderByDescending(x => x.PostId).ToList();
            }
            else
            {
                lsPost = _context.Posts.Include(p => p.Account)
                           .Include(p => p.Cat)
                           .Where(p => p.AccountId == taikhoan.AccountId)
                           .OrderByDescending(x => x.PostId).ToList();
            }

            if (catID != 0)
            {
                lsPost = _context.Posts
                            .AsNoTracking().Where(x => x.CatId == catID)
                            .Include(x => x.Cat)
                            .OrderByDescending(x => x.PostId).ToList();
            }
            else
            {
                lsPost = _context.Posts
                            .AsNoTracking()
                            .Include(x => x.Cat)
                            .OrderByDescending(x => x.PostId).ToList();
            }

            PagedList<Post> models = new PagedList<Post>(lsPost.AsQueryable(), pageNumber, pageSize);

            var cate = _context.Categories.ToList();
            List<Category> cate1 = new();
            cate1.Add(new Category()
            {
                CatId = 0,
                CatName = "ALL"
            }); ;
            cate1.AddRange(_context.Categories);
            ViewBag.DanhMuc = new SelectList(cate1, "CatId", "CatName");


            return View(models);
        }
        public IActionResult Filter(int catID = 0)
        {
            var url = $"/Admin/Posts/Index?catID={catID}";
            if (catID == 0)
            {
                url = $"/Admin/Posts/Index";
            }
            else
            {
                url = $"/Admin/Posts/Index?catID={catID}";
            }
            var zzz = Json(new { status = "success", redirectUrl = url });
            return zzz;
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
        public async Task<IActionResult> Create()
        {
            // phải đăng nhập để đăng bài
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(User.GetAccountID()));
            if (taikhoan == null) return RedirectToAction("Login", "Accounts", new { Areas = "Admin" });

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
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(User.GetAccountID()));
            if (taikhoan == null) return NotFound();

            if (ModelState.IsValid)
            {
                post.AccountId = int.Parse(User.GetAccountID());
                post.Author = User.FindFirstValue(ClaimTypes.Name);
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

            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(User.GetAccountID()));
            if (taikhoan == null) return NotFound();

            var post = await _context.Posts.FindAsync(id);

            // chỉ sửa bài của mình khác id ko cho sửa,admin sửa dc hết
            if (post.AccountId != taikhoan.AccountId && User.FindFirstValue(ClaimTypes.Role) != "Admin")
                return RedirectToAction(nameof(Index));

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
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(User.GetAccountID()));
            if (taikhoan == null) return NotFound();

            // chỉ sửa bài của mình khác id ko cho sửa,admin sửa dc hết
            if (post.AccountId != taikhoan.AccountId && User.FindFirstValue(ClaimTypes.Role) != "Admin")
                return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                try
                {
                    post.AccountId = int.Parse(User.GetAccountID());
                    post.Author = User.FindFirstValue(ClaimTypes.Name);
                    post.Alias = post.Title.ToUrlFriendly();

                    if (fThumb != null) post.Thumb = await save.UploadImage(@"images/Post/Thumb/", fThumb, post.Title);


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

            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(User.GetAccountID()));
            if (taikhoan == null) return NotFound();

            Post post = new Post();
            if (User.IsInRole("Admin"))
            {
                post = await _context.Posts
                                .Include(p => p.Account)
                                .Include(p => p.Cat)
                                .FirstOrDefaultAsync(m => m.PostId == id);
            }
            else // ko phải admin đòi xóa
            {
                post = await _context.Posts
                            .Include(p => p.Account)
                           .Include(p => p.Cat)
                           .Where(p => p.AccountId == int.Parse(User.GetAccountID()))
                           .FirstOrDefaultAsync(m => m.PostId == id);

            }
            if (post == null) return NotFound();

            ViewBag.Categories = new SelectList(_context.Categories, "CatId", "CatName", post.CatId);

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
