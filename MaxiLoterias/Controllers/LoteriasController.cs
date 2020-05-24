using System;
using System.Linq;
using System.Threading.Tasks;
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
        [Route("loterias/ultimosdias/{dias}/desde/{fecha}")]
        [Route("loterias/ultimosdias/{dias}")]
        public async Task<IActionResult> UltimosDias(int? dias, string fecha)
        {
            if(dias.HasValue)
            {
                var date = DateTime.Now;

                if (DateTime.TryParse(fecha, out DateTime d))
                    date = d;

                var tasks = Enumerable.Range(0, dias.Value)
                                      .Select(n => loteriaServicio.GoGet(date.AddDays(- n)));

                var results = await Task.WhenAll(tasks);

                return Ok(results);
            }
            else
            {
                return NotFound("Dia no especificado");
            }

        }


        #region Raws

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

        #endregion
    }
}