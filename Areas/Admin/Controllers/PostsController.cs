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
        public IActionResult Filter(int? catID, string keyword, string CurrentCat_ID)
        {
            var url = $"/Admin/Posts/Index?catID={catID}&keyword={keyword}";
            if (catID == null && keyword == null  )
            {
                url = $"/Admin/Posts/Index";
            }

            //else // catID == null || keyword == null
            //{
            //    if(catID != null)url = $"/Admin/Posts/Index?catID={catID}";
            //    if(keyword != null)url = $"/Admin/Posts/Index?keyword={keyword}";
            //}
            var zzz = Json(new { status = "success", redirectUrl = url });
            return zzz;
        }
        // GET: Admin/Posts
        public async Task<IActionResult> Index(int? page, int? catID , string keyword)
        {
            var ID_user = User.GetSpecificClaim("Account_Id");
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(ID_user));

            if (taikhoan == null) return NotFound();


             var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 1;

           
            IQueryable<Post> qr_post = _context.Posts
                                            .Include(p => p.Account)
                                            .Include(p => p.Cat)
                                            .OrderByDescending(x => x.PostId)
                                            .AsNoTracking();

            if (!User.IsInRole("Admin"))
            {
                qr_post = qr_post.Where(p => p.AccountId == taikhoan.AccountId);
            }

            if (catID != null && catID != 0  )
            {
                qr_post = qr_post.Where(x => x.CatId == catID);
            }

            if (keyword != null)
            {
                qr_post = qr_post.Where(x => x.Title.Contains(keyword) || x.Contents.Contains(keyword))
                           .OrderByDescending(x => x.CreatedAt);
            }

            List<Post> lsPost = new List<Post>();
            lsPost = qr_post.ToList();

            PagedList<Post> models = new PagedList<Post>(lsPost.AsQueryable(), pageNumber, pageSize);

            var cate = _context.Categories.ToList();
            List<Category> cate1 = new();

            cate1.Add(new Category()
            {
                CatId = 0,
                CatName = "ALL"
            });

            cate1.AddRange(_context.Categories);

            ViewBag.DanhMuc = new SelectList(cate1, "CatId", "CatName");
            
            ViewBag.DanhMuc_ID = catID;
            
            ViewBag.keyword = keyword;

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
            try
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
            catch
            {
                return View("NOTFOUND");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PostId,Title,ShortContent,Contents," +
            "Thumb,Published,Alias,CreatedAt,Author,IsHot,IsNewFeed,AccountId,CatId")]
        Post post, IFormFile fThumb)
        {
            //if (id != post.PostId)
            //{
            //    return NotFound();
            //}
            var taikhoan = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(User.GetAccountID()));
            if (taikhoan == null) return NotFound();
            
            post.AccountId = int.Parse(User.GetAccountID());
            // chỉ sửa bài của mình khác id ko cho sửa,admin sửa dc hết
            if (post.AccountId != taikhoan.AccountId && User.FindFirstValue(ClaimTypes.Role) != "Admin")
                return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                try
                {
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
