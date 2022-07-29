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
using Microsoft.AspNetCore.Authorization;
using blog_web.Areas.Admin.Data.ViewModel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using blog_web.Extension;

namespace blog_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class AccountsController : Controller
    {
        private readonly blogdbContext _context;

        public AccountsController(blogdbContext context)
        {
            _context = context;
        }

        // GET: Admin/Login
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }

            return View("LogIn");
        }
        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html")]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    Account kh = _context.Accounts
                                    .Include(p => p.Role)
                                    .SingleOrDefault(p => p.Email.ToLower() == model.EmailAddress.ToLower().Trim());
                    if (kh == null)
                    {
                        ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
                        return View(model);
                    }
                    string pass = model.Password.Trim();
                    if (kh.Password.Trim() != pass)
                    {
                        ViewBag.Error = "Thông tin đăng nhập chưa chính xác";
                        return View(model);
                    }
                    // đăng nhập thành công
                    //Cập nhật ngày
                    kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();

                    //Identity
                    // Lưu session MaKH
                    HttpContext.Session.SetString("id_tai_khoan", kh.AccountId.ToString());
                    //Identity
                    var userClaims = new List<Claim>
                    {
                                new Claim(ClaimTypes.Name, kh.FullName),
                                new Claim(ClaimTypes.Email, kh.Email),
                                new Claim("Account_Id", kh.AccountId.ToString()),
                                new Claim("Role_Id", kh.RoleId.ToString()),
                                new Claim(ClaimTypes.Role, kh.Role.RoleName),
                    };
                    //c2
                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User Identity");
                    await HttpContext.SignInAsync(new ClaimsPrincipal(grandmaIdentity));

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home", new { Area = "Admin" });
                }
            }
            catch
            {
                return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            }
            return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
        }

        [Route("dang-xuat.html")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync("CookieAuthentication_zz");
                HttpContext.Session.Remove("id_tai_khoan");
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }
            catch
            {
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }
        }

        // GET: Admin/Accounts
        [Authorize(Roles = "Admin")]
        public IActionResult Index(int? page)
        {
            var pageNumber = page == null || page <= 0 ? 1 : page.Value;
            var pageSize = 3;

            var lsAccount = _context.Accounts
                                    .Include(a => a.Role)
                                    .OrderByDescending(x => x.CreatedAt);
            PagedList<Account> models = new PagedList<Account>(lsAccount, pageNumber, pageSize);

            ViewBag.CurrentPage = pageNumber;
            return View(models);
        }
        [Authorize(Roles = "Admin")]
        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Accounts/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,FullName,Email,Phone,Password," +
            "Active,CreatedAt,RoleId,LastLogin")] Account account)
        {
            if (ModelState.IsValid)
            {
                if (account.CreatedAt == null) account.CreatedAt = DateTime.Now;
                _context.Add(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,FullName,Email," +
            "Phone,Password,Active,CreatedAt,RoleId,LastLogin")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var account = await _context.Accounts.FindAsync(id);
            _context.Accounts.Remove(account);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
            return _context.Accounts.Any(e => e.AccountId == id);
        }

        [AllowAnonymous]
        [Route("not-found.html")]
        public IActionResult AccessDenied()
        {
            return View("NOTFOUND");
        }

        #region  User

        #region EditProfile
        [HttpGet]
        [Route("edit-profile.html")]
        public async Task<IActionResult> EditProfile()
        {
            var id = User.GetSpecificClaim("Account_Id");
            if (string.IsNullOrEmpty(id)) return View("NOTFOUND");
            Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(id));
            if (account == null) return View("NOTFOUND");
            return View(account);
        }

        [BindProperty]
        public Account account { get; set; }
        [HttpPost]
        [Route("edit-profile.html")]
        public async Task<IActionResult> EditProfile(Account account)
        {
            _context.Update(account);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home", new { Area = "Admin" });
        }
        #endregion



        #region ChangePassWord
        [Route("/doi-mat-khau.html")]
        [HttpGet]
        public IActionResult ChangePassWord()
        {
            return View("ChangePassWord");
        }
        [HttpPost]
        [Route("/doi-mat-khau.html")]
        public async Task<IActionResult> ChangePassWord(ChangePasswordViewModel changePassword)
        {
            if (ModelState.IsValid)
            {
                var id = User.GetSpecificClaim("Account_Id");
                if (string.IsNullOrEmpty(id)) return View("NOTFOUND");
                Account account = await _context.Accounts.FirstOrDefaultAsync(x => x.AccountId == int.Parse(id));
                if (account == null) return View("NOTFOUND");
                account.Password = changePassword.ConfirmPassword;
                _context.Update(account);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home", new { Area = "Admin" });
            }
            return View(changePassword);
        }
        #endregion
        #endregion


    }
}
