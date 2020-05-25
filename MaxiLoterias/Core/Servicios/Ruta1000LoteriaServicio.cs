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
        string MakeUrl (string dateAsString) => $"https://m.ruta1000.com.ar/index2008.php?FechaAlMinuto={dateAsString}#Sorteos";

        string DateToString(DateTime f) => $"{f.Year}_{f.Month.AddZeroFront()}_{f.Day.AddZeroFront()}";

        /*
        Esta es la función que parsea el string de input, fijarse que hay casos en los que no está pudiendo parsear bien.  
        */
        IEnumerable<string> ParseInput(string input)
        {
            return input.SplitBy("  ").Where(s => !string.IsNullOrWhiteSpace(s));
        }

        public async Task<IEnumerable<string>> GetRaw(DateTime fecha)
        {
            var config = Configuration.Default.WithDefaultLoader();
            var address = MakeUrl(DateToString(fecha));
            var context = BrowsingContext.New(config);
            var document = await context.OpenAsync(address);
            var cellSelector = "td[valign|='upper']";
            var cells = document.QuerySelectorAll(cellSelector);
            
            return cells.Select(m => m.TextContent);
        }

        IEnumerable<T> GetRawGroups<T>(IEnumerable<string> content, Func<IGrouping<string, string>, T> func)
        {
            //Cada tabla son grupos de 8
            var state = new StateInternal(8);

            return content.GroupBy(t => state.Letter())
                          .Select(g => func(g));
        }

        public async Task<IEnumerable<IEnumerable<string>>> GetRawInputs(DateTime fecha)
        {
            return GetRawGroups(await GetRaw(fecha), g => g.Select(s => s));
        }

        public async Task<LoteriaDTO> GoGet(DateTime fecha)
        {
            var bloques = GetRawGroups(await GetRaw(fecha), g => new Bloque(g.Select(s => ParseInput(s))));

            var dto = new LoteriaDTO()
            {
                Fecha = fecha.ToShortDateString(),
                Loterias = bloques.SelectMany(b => b.Loterias).ToList()
            };

            return dto;            
        }
    }
}
