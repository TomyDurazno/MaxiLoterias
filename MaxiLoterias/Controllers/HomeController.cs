using MaxiLoterias.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MaxiLoterias.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(IWebHostEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;
            
            var JSON = System.IO.File.ReadAllText(contentRootPath + "/Data/Index.json");

            var model = JToken.Parse(JSON).ToObject<List<IndexViewModel>>();

            return View(model);
        }
    }
}