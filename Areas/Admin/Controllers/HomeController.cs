﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace blog_web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize()]
    public class HomeController : Controller
    {
        [Route("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
