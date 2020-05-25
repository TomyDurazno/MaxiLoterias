﻿using System;
using System.Linq;
using System.Threading.Tasks;
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
        [Route("loterias/fecha/{fecha}/{command?}")]
        public async Task<IActionResult> PorFecha(string fecha, string command = null)
        {            
            if(DateTime.TryParse(fecha, out DateTime date))
            {
                var bloques = await loteriaServicio.GoGet(date);

                return Ok(ExecuteCommand(bloques, command));
            }
            else
            {
                return NotFound("La fecha es errónea o no existen campos para la misma");
            }
        }

        [HttpGet]
        [Route("loterias/hoy/{command?}")]
        public async Task<IActionResult> Hoy(string command = null)
        {
            var bloques = await loteriaServicio.GoGet(DateTime.Now);

            return Ok(ExecuteCommand(bloques, command));
        }

        [HttpGet]
        [Route("loterias/ultimosdias/{dias}/{command?}")]
        [Route("loterias/ultimosdias/{dias}/hasta/{fecha}/{command?}")]
        public async Task<IActionResult> UltimosDias(int? dias, string fecha, string command = null)
        {
            if(dias.HasValue)
            {
                var date = DateTime.Now;

                if (DateTime.TryParse(fecha, out DateTime d))
                    date = d;

                var tasks = Enumerable.Range(0, dias.Value)
                                      .Select(n => loteriaServicio.GoGet(date.AddDays(- n)));

                var results = await Task.WhenAll(tasks);

                return Ok(results.Select(l => ExecuteCommand(l, command)));
            }
            else
            {
                return NotFound("Dia no especificado");
            }
        }

        #region ExecuteCommand

        object ExecuteCommand(LoteriaResult result, string command)
        {
            switch (command)
            {
                case "pretty":
                    return result; //Custom JSON Converter

                default:
                    return result.ToDTO();
            }
        }

        #endregion
    }
}