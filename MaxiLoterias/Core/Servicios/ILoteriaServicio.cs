using MaxiLoterias.Core.Models;
using System;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Servicios
{
    public interface ILoteriaServicio
    {
        public Task<LoteriaResult> GoGet(DateTime fecha);

        public string RutaHoy { get; }
    }
}
