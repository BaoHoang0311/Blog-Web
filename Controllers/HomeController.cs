using blog_web.Data.ViewModel;
using blog_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Controllers
{
    public class HomeController : Controller
    {
        private readonly blogdbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, blogdbContext context)
        {
            _logger = logger;
            _context = context;
        }
       
        public async Task<IActionResult> Index()
        {
            var list = await _context.Posts
                                .Include(m=>m.Cat)
                                .Include(m=>m.Account)
                                .Where(x => x.Published == true)
                                .OrderByDescending(x => x.CreatedAt)
                                .ToListAsync();
            HomePageVM homepage = new HomePageVM()
            {
                Inspiration = list,
                Populars = list.Where(x=>x.IsHot == true).ToList(),
                Trendings = list.Where(x => x.IsHot == true).ToList(),
                LatestPosts = list,
                Recents = list,
                post =list.FirstOrDefault(),
            };
            return View(homepage);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
