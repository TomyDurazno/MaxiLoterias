using MaxiLoterias.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
        public string _Nombre;
        public string _SubCodigo;
        public bool Construct;
        public LoteriaState _Estado;

        public List<NumeroLoteria> Numeros { get; set; }

        public Loteria(LoteriaState estado)
        {
            _Estado = estado;
        }

        public Loteria(LoteriaState estado, string nombre, string subCodigo): this(estado)
        {
            _Nombre = nombre;
            _SubCodigo = subCodigo;
            Construct = true;
        }

        public string Nombre { get => _Estado != LoteriaState.Error ? _Nombre : null; }
        public string SubCodigo { get => _SubCodigo; }

        public string Estado 
        { 
            get 
            {
                if (Numeros?.Any() == true)
                    return null;

                return _Estado != LoteriaState.Jugado ? _Estado.GetDescription() : null; 
            } 
        }
    }
}
