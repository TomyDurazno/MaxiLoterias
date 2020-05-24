using MaxiLoterias.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Servicios
{
    public interface ILoteriaServicio
    {
        public Task<LoteriaDTO> GoGet(DateTime fecha);
        public Task<IEnumerable<T>> GetRaw<T>(DateTime fecha, Func<IGrouping<string, string>, T> func);
        public Task<IEnumerable<IEnumerable<string>>> GetRawInputs(DateTime fecha);
    }
}
