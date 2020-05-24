using MaxiLoterias.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Models
{
    public enum LoteriaState
    {
        [Description("Error al tratar de leer los datos")]
        Error,
        [Description("Pendiente de Juego")]
        PendienteDeJuego,
        [Description("Jugado")]
        Jugado
    }

    public class Loteria
    {
        List<string> _RawValue;
        public string _Nombre;
        public string _SubCodigo;
        public bool Construct;
        public LoteriaState _Estado;

        public string Nombre { get => _Estado != LoteriaState.Error ? _Nombre?.FixS() : null; }
        public string SubCodigo { get => _SubCodigo?.FixS(); }

        public string Estado { get => _Estado != LoteriaState.Jugado ? _Estado.GetDescription() : null; }
        public List<NumeroLoteria> Numeros { get; set; }

        public Loteria(List<string> RawValue)
        {
            try
            {
                _RawValue = RawValue;
                int cursor = 0;                

                if (RawValue.ElementAt(1).Contains("1º"))
                {
                    _Nombre = RawValue.ElementAt(0);
                    cursor = 1;
                }

                if (RawValue.ElementAt(2).Contains("1º"))
                {
                    _Nombre = RawValue.ElementAt(0);
                    _SubCodigo = RawValue.ElementAt(1);
                    cursor = 2;
                }

                if (RawValue.ElementAt(3).Contains("1º"))
                {
                    _Nombre = RawValue.ElementAt(0);
                    _SubCodigo = RawValue.ElementAt(1);
                    cursor = 3;
                }

                var state = new StateInternal(2);
                var groups = _RawValue.Skip(cursor).GroupBy(r => state.Letter());

                var numeros = groups.Select(g => new NumeroLoteria(g.ToArray())).ToList();

                if (numeros.All(n => n.Numero.HasValue))
                {
                    Numeros = numeros;
                    _Estado = LoteriaState.Jugado;
                }
                else
                {
                    _Estado = LoteriaState.PendienteDeJuego;
                }

                Construct = true;
            }
            catch
            {
                _Estado = LoteriaState.Error;
            }
        }
    }
}
