using System;
using System.Collections.Generic;
using System.Linq;

namespace MaxiLoterias.Core.Models
{
    public class LoteriaResult
    {
        public string Fecha { get; set; }
        public List<Loteria> Loterias { get; set; }

        public LoteriaResultDTO ToDTO()
        {
            return new LoteriaResultDTO()
            {
                Fecha = Fecha,
                Loterias = Loterias.Select(l => l.ToDTO()).ToList()
            };
        }
    }

    public class LoteriaResultDTO
    {
        public string Fecha { get; set; }
        public List<LoteriaDTO> Loterias { get; set; }
    }
}
