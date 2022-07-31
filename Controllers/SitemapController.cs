﻿using blog_web.Data.Extension.Sitemap;
using blog_web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Controllers
{
    public class SitemapController : Controller
    {
        private readonly blogdbContext _context;
        public SitemapController(blogdbContext context)
        {
            _context = context;
        }

        [Route("sitemap")]
        public async Task<ActionResult> SitemapAsync()
        {
            string baseUrl = "https://localhost:44359/";

            // get a list of published articles
            var posts = await _context.Posts.ToListAsync();

            // get last modified date of the home page
            var siteMapBuilder = new Sitemapbuilder();

            // add the home page to the sitemap
            siteMapBuilder.AddUrl(baseUrl, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 1.0);

            // add the blog posts to the sitemap
            foreach (var post in posts)
            {
                siteMapBuilder.AddUrl(baseUrl + post.Alias, modified: post.CreatedAt, changeFrequency: null, priority: 0.9);
            }

            // generate the sitemap xml
            string xml = siteMapBuilder.ToString();
            return Content(xml, "text/xml");
        }
    }
}