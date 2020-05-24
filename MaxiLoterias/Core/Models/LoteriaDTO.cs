using System;
using System.Collections.Generic;

namespace MaxiLoterias.Core.Models
{
    public class LoteriaDTO
    {
        public string Fecha { get; set; }
        public List<Loteria> Loterias { get; set; }
    }
}
