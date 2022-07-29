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
    public class Categories_Mobi : ViewComponent
    {
        private readonly blogdbContext _context;
        private readonly IMemoryCache memoryCache;
        public Categories_Mobi(blogdbContext context, IMemoryCache _memoryCache)
        {
            _context = context;
            memoryCache = _memoryCache;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {

            var categories = await memoryCache.GetOrCreate(CacheKey.Categories, cacheEntry =>
            {
                cacheEntry.SlidingExpiration = TimeSpan.FromSeconds(3);
                cacheEntry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20);
                return GetlsCategories();
            });
            return View(categories);
        }
        // số ở giỏ hàng
        public async Task<Category[]> GetlsCategories()
        {
            var cate = await _context.Categories
                    .AsNoTracking()
                                  .Where(x => x.Published == true)
                                  .ToArrayAsync();
            return cate;
        }
    }
}
