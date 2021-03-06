﻿using AngleSharp;
using MaxiLoterias.Core.Extensions;
using MaxiLoterias.Core.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Servicios
{
    public class Ruta1000LoteriaServicio : ILoteriaServicio
    {
        #region Fields

        ITokenizerService<string> tokenizerService;
        ILogger<Ruta1000LoteriaServicio> logger;

        #endregion

        #region Constructor

        public Ruta1000LoteriaServicio(ILogger<Ruta1000LoteriaServicio> _logger)
        {
            logger = _logger;
        }

        #endregion

        #region Private Properties

        string MakeUrl (string dateAsString) => $"https://m.ruta1000.com.ar/index2008.php?FechaAlMinuto={dateAsString}#Sorteos";

        string DateToString(DateTime f) => $"{f.Year}_{f.Month.AddZeroFront()}_{f.Day.AddZeroFront()}";        

        Dictionary<string, string> Nombres = new Dictionary<string, string>()
        {
            { "QUINIELADE LA CIUDAD(Ex-Nacional)PRIMERA", "QUINIELA DE LA CIUDAD (Ex-Nacional) PRIMERA" },
            { "QUINIELADE LA CIUDAD(Ex-Nacional)MATUTINA", "QUINIELA DE LA CIUDAD (Ex-Nacional) MATUTINA" },
            { "QUINIELADE LA CIUDAD(Ex-Nacional)VESPERTINA", "QUINIELA DE LA CIUDAD (Ex-Nacional) VESPERTINA" },
            { "QUINIELADE LA CIUDAD(Ex-Nacional)NOCTURNA", "QUINIELA DE LA CIUDAD (Ex-Nacional) NOCTURNA" },
            { "QUINIELABUENOS AIRESPRIMERA", "QUINIELA BUENOS AIRES PRIMERA" },
            { "QUINIELABUENOS AIRESMATUTINA", "QUINIELA BUENOS AIRES MATUTINA" },
            { "QUINIELABUENOS AIRESVESPERTINA", "QUINIELA BUENOS AIRES VESPERTINA" },
            { "QUINIELABUENOS AIRESNOCTURNA", "QUINIELA BUENOS AIRES NOCTURNA" },
            { "QUINIELACORDOBAPRIMERA", "QUINIELA CORDOBA PRIMERA" },
            { "QUINIELACORDOBAMATUTINA", "QUINIELA CORDOBA MATUTINA" },
            { "QUINIELACORDOBAVESPERTINA", "QUINIELA CORDOBA VESPERTINA" },
            { "QUINIELACORDOBANOCTURNA", "QUINIELA CORDOBA NOCTURNA" },
            { "QUINIELASANTA FEPRIMERA", "QUINIELA SANTA FE PRIMERA" },
            { "QUINIELASANTA FEMATUTINA", "QUINIELA SANTA FE MATUTINA" },
            { "QUINIELASANTA FEVESPERTINA", "QUINIELA SANTA FE VESPERTINA" },
            { "QUINIELASANTA FENOCTURNA", "QUINIELA SANTA FE NOCTURNA" },
            { "QUINIELAENTRE RIOSPRIMERA", "QUINIELA ENTRE RIOS PRIMERA" },
            { "QUINIELAENTRE RIOSMATUTINA", "QUINIELA ENTRE RIOS MATUTINA" },
            { "QUINIELAENTRE RIOSVESPERTINA", "QUINIELA ENTRE RIOS VESPERTINA" },
            { "QUINIELAENTRE RIOSNOCTURNA", "QUINIELA ENTRE RIOS NOCTURNA" },
            { "QUINIELAMONTEVIDEOMATUTINA", "QUINIELA MONTEVIDEO MATUTINA" },
            { "QUINIELAMONTEVIDEONOCTURNA", "QUINIELA MONTEVIDEO NOCTURNA" },
            {"QUINIELACORRIENTESMATUTINA", "QUINIELA CORRIENTES MATUTINA" },
            {"QUINIELACORRIENTESVESPERTINA", "QUINIELA CORRIENTES VESPERTINA" },
            {"QUINIELACORRIENTESNOCTURNA", "QUINIELA CORRIENTES NOCTURNA" },
            {"QUINIELACORRIENTESPRIMERA" , "QUINIELA CORRIENTES PRIMERA" }
        };

        #endregion

        #region Interface Implementation

        public string RutaHoy => MakeUrl(DateToString(DateTime.Now));

        public async Task<LoteriaResult> GoGet(DateTime fecha)
        {
            var bloques = GetRawGroups(await GetRaw(fecha), MakeBloque);

            var loterias = bloques.SelectMany(b => b.Loterias).ToList();

            loterias.ForEach(l => l.Fecha = fecha);

            return new LoteriaResult()
            {
                Fecha = fecha.ToShortDateString(),
                Loterias = loterias
            };         
        }        
        #endregion

        #region Private Methods

        async Task<IEnumerable<string>> GetRaw(DateTime fecha)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = MakeUrl(DateToString(fecha));

            var document = await BrowsingContext.New(config).OpenAsync(address);

            return document.QuerySelectorAll("td[valign|='upper']")
                           .Select(m => m.TextContent);
        }

        IEnumerable<T> GetRawGroups<T>(IEnumerable<string> content, Func<IGrouping<string, string>, T> func)
        {
            //Cada tabla son grupos de 8
            var state = new StateInternal(8);

            return content.GroupBy(t => state.Letter())
                          .Select(g => func(g));
        }

        //Esta es la función que parsea el string del input, fijarse que hay casos en los que no está pudiendo parsear bien.  
        //TO FIX: esta función es en general la responsable de que algo falle. Refactorizarla
        IEnumerable<string> ParseInput(string input)
        {
            return input.SplitBy("\n").Where(s => !string.IsNullOrWhiteSpace(s));
        }

        Bloque MakeBloque(IGrouping<string, string> g)
        {
            logger.Log(LogLevel.Information, $"Before Make Bloque: {JsonConvert.SerializeObject(g)}");

            var arr = g.Select(s => ParseInput(s)).ToArray().Select(rv => rv.ToList());

            logger.Log(LogLevel.Information, $"After Parse Input: {JsonConvert.SerializeObject(arr)}");

            int count = 0;
            var lists = new List<List<string>>();

            foreach (var element in arr)
            {
                if (count == 0)
                {
                    var r = arr.ElementAtOrDefault(4);

                    if (r != null)
                        element.AddRange(r);

                    lists.Add(element);
                }

                if (count == 1)
                {
                    var r = arr.ElementAtOrDefault(5);

                    if (r != null)
                        element.AddRange(r);

                    lists.Add(element);
                }

                if (count == 2)
                {
                    var r = arr.ElementAtOrDefault(6);

                    if (r != null)
                        element.AddRange(r);

                    lists.Add(element);
                }

                if (count == 3)
                {
                    var r = arr.ElementAtOrDefault(7);

                    if (r != null)
                        element.AddRange(r);

                    lists.Add(element);
                }

                count++;
            }

            var bloque = new Bloque()
            {
                //Take(4) porque hay duplicación en los siguientes elementos
                Loterias = lists.Take(4).Select(MakeLoteria).ToList()
            };

            logger.Log(LogLevel.Information, $"After Make Bloque: { JsonConvert.SerializeObject(bloque) }");

            return bloque;
        }

        Loteria MakeLoteria(IEnumerable<string> rawValue)
        {
            string makeNombre() => rawValue.ElementAt(0)?
                                           .FixS()
                                           .Pipe(key => Nombres.TryGetValue(key, out string value) ? value : key);

            int cursor = 0;

            string makeSubCodigo() => cursor != 1 ? rawValue.ElementAt(1)?.FixS() : null;

            bool containsTopNumber(int n) => rawValue.ElementAtOrDefault(n)?.Contains("1º") ?? false;

            try
            {
                if (containsTopNumber(1))
                    cursor = 1;

                if (containsTopNumber(2))
                    cursor = 2;

                if (containsTopNumber(3))
                    cursor = 3;

                var state = new StateInternal(2);
                var groups = rawValue.Skip(cursor).GroupBy(r => state.Letter());

                var numeros = groups.Select(g => new NumeroLoteria(g.ToArray())).ToList();

                if (numeros.All(n => n.HasValue()))
                {
                    return new Loteria(LoteriaState.Jugado, makeNombre(), makeSubCodigo())
                    {
                        Numeros = numeros
                    };
                }
                else
                {
                    return new Loteria(LoteriaState.PendienteDeJuego, makeNombre(), makeSubCodigo());
                }                
            }
            catch
            {
                return new Loteria(LoteriaState.Error);
            }
        }

        #endregion
    }
}
