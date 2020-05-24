using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AngleSharp;
using MaxiLoterias.Core;
using MaxiLoterias.Core.Extensions;
using MaxiLoterias.Core.Models;
using MaxiLoterias.Core.Servicios;
using Microsoft.AspNetCore.Mvc;

namespace MaxiLoterias.Controllers
{
    public class LoteriasController : Controller
    {
        ILoteriaServicio loteriaServicio;

        public LoteriasController(ILoteriaServicio _loteriaServicio)
        {
            loteriaServicio = _loteriaServicio;
        }


        [HttpGet]
        [Route("loterias/fecha/{fecha}")]
        public async Task<IActionResult> PorFecha(string fecha)
        {            
            if(DateTime.TryParse(fecha, out DateTime date))
            {
                var bloques = await loteriaServicio.GoGet(date);

                return Ok(bloques);
            }
            else
            {
                return NotFound("La fecha es errónea o no existen campos para la misma");
            }
        }

        [HttpGet]
        [Route("loterias/hoy")]
        public async Task<IActionResult> Hoy()
        {
            var bloques = await loteriaServicio.GoGet(DateTime.Now);

            return Ok(bloques);
        }

        [HttpGet]
        [Route("loterias/raw/hoy")]
        public async Task<IActionResult> HoyRaw()
        {
            var obj = await loteriaServicio.GetRawInputs(DateTime.Now);

            return Ok(obj);
        }

        [HttpGet]
        [Route("loterias/raw/fecha/{fecha}")]
        public async Task<IActionResult> PorFechaRaw(string fecha)
        {
            if (DateTime.TryParse(fecha, out DateTime date))
            {
                var bloques = await loteriaServicio.GetRawInputs(DateTime.Now);

                return Ok(bloques);
            }
            else
            {
                return NotFound("La fecha es errónea o no existen campos para la misma");
            }
        }
    }
}