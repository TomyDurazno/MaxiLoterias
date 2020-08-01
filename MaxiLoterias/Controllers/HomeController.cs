using MaxiLoterias.Core.Servicios;
using MaxiLoterias.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace MaxiLoterias.Controllers
{
    public class HomeController : Controller
    {
        #region Fields

        IWebHostEnvironment _hostingEnvironment;
        ILoteriaServicio _loteriaServicio;

        #endregion

        #region Constructor

        public HomeController(IWebHostEnvironment hostingEnvironment, ILoteriaServicio loteriaServicio)
        {
            _hostingEnvironment = hostingEnvironment;
            _loteriaServicio = loteriaServicio;
        }

        #endregion

        #region Endpoints

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            string contentRootPath = _hostingEnvironment.ContentRootPath;

            var JSON = System.IO.File.ReadAllText($"{contentRootPath}/Data/Index.json");

            var model = JToken.Parse(JSON).ToObject<List<IndexViewModel>>();

            ViewBag.RutaHoy = _loteriaServicio.RutaHoy;

            return View(model);
        }

        #endregion
    }
}