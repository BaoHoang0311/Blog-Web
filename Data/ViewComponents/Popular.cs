using blog_web.Data.Enums;
using blog_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Data.ViewComponents
{
    public class Popular : ViewComponent
    {
        private readonly blogdbContext _context;
        private IMemoryCache memoryCache;
        public Popular(blogdbContext context, IMemoryCache _memoryCache)
        {
            _context = context;
            memoryCache = _memoryCache;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Post[] list_popular_post = await memoryCache.GetOrCreate(CacheKey.Popular, cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
                return GetlsPopular();
            });
            return View(list_popular_post);
        }
        public async Task<Post[]> GetlsPopular()
        {
            Post[] list_popular_post = await _context.Posts
                                    .Include(m=>m.Cat)
                                    .Include(m => m.Account)
                                    .AsNoTracking()
                                    .Where(x => x.IsHot == true)
                                    .OrderByDescending(x => x.CreatedAt)
                                    .ToArrayAsync();
            return list_popular_post;
        }

    }
}
