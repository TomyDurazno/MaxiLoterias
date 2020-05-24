using MaxiLoterias.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Models
{
    public class NumeroLoteria
    {
        string[] _RawValue;
        public int Puesto { get; set; }
        public int? Numero { get; set; }

        public NumeroLoteria(string[] RawValue)
        {
            _RawValue = RawValue;
            Puesto = _RawValue[0].Replace("º", string.Empty).Pipe(Convert.ToInt32);
            Numero = _RawValue[1].Pipe(n => int.TryParse(n, out var val) ? val : (int?)null);
        }
    }
}
