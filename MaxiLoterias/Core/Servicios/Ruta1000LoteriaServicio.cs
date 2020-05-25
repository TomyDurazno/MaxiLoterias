using AngleSharp;
using MaxiLoterias.Core.Extensions;
using MaxiLoterias.Core.Models;
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

        #endregion

        #region Constructor

        public Ruta1000LoteriaServicio(ITokenizerService<string> _tokenizerService)
        {
            tokenizerService = _tokenizerService;
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
        };

        #endregion

        #region Interface Implementation

        public async Task<LoteriaResult> GoGet(DateTime fecha)
        {
            var bloques = GetRawGroups(await GetRaw(fecha), MakeBloque);

            return new LoteriaResult()
            {
                Fecha = fecha.ToShortDateString(),
                Loterias = bloques.SelectMany(b => b.Loterias).ToList()
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

        Bloque MakeBloque(IGrouping<string, string> g)
        {
            //Esta es la función que parsea el string del input, fijarse que hay casos en los que no está pudiendo parsear bien.  
            IEnumerable<string> ParseInput(string input)
            {
                return input.SplitBy("  ").Where(s => !string.IsNullOrWhiteSpace(s));
            }

            var arr = g.Select(s => ParseInput(s)).ToArray().Select(rv => rv.ToList());

            int count = 0;
            var _Lists = new List<List<string>>();

            foreach (var element in arr)
            {
                if (count == 0)
                {
                    var r = arr.ElementAtOrDefault(4);

                    if (r != null)
                        element.AddRange(r);

                    _Lists.Add(element);
                }

                if (count == 1)
                {
                    var r = arr.ElementAtOrDefault(5);

                    if (r != null)
                        element.AddRange(r);

                    _Lists.Add(element);
                }

                if (count == 2)
                {
                    var r = arr.ElementAtOrDefault(6);

                    if (r != null)
                        element.AddRange(r);

                    _Lists.Add(element);
                }

                if (count == 3)
                {
                    var r = arr.ElementAtOrDefault(7);

                    if (r != null)
                        element.AddRange(r);

                    _Lists.Add(element);
                }

                count++;
            }

            return new Bloque()
            {
                Loterias = _Lists.Take(4).Select(MakeLoteria).ToList()
            };
        }

        Loteria MakeLoteria(IEnumerable<string> rawValue)
        {
            string makeNombre()
            {
                var fix = rawValue.ElementAt(0)?.FixS();

                return Nombres.TryGetValue(fix, out string value) ? value : fix;
            }

            string makeSubCodigo()
            {
                return rawValue.ElementAt(1)?.FixS();
            }

            try
            {
                int cursor = 0;
                string _subCodigo = null;

                if (rawValue.ElementAt(1).Contains("1º"))
                {
                    cursor = 1;
                }

                if (rawValue.ElementAt(2).Contains("1º"))
                {
                    _subCodigo = makeSubCodigo();
                    cursor = 2;
                }

                if (rawValue.ElementAt(3).Contains("1º"))
                {
                    _subCodigo = makeSubCodigo();
                    cursor = 3;
                }

                var state = new StateInternal(2);
                var groups = rawValue.Skip(cursor).GroupBy(r => state.Letter());

                var numeros = groups.Select(g => new NumeroLoteria(g.ToArray())).ToList();

                if (numeros.All(n => n.HasValue()))
                {
                    return new Loteria(LoteriaState.Jugado, makeNombre(), _subCodigo)
                    {
                        Numeros = numeros
                    };
                }
                else
                {
                    return new Loteria(LoteriaState.PendienteDeJuego, makeNombre(), _subCodigo);
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
