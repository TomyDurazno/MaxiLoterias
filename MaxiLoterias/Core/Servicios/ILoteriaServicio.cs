using MaxiLoterias.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Servicios
{
    public interface ILoteriaServicio
    {
        public Task<LoteriaResult> GoGet(DateTime fecha);
        public Task<IEnumerable<string>> GetRaw(DateTime fecha);
        public Task<IEnumerable<IEnumerable<string>>> GetRawInputs(DateTime fecha);
    }
}
