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

        public DateTime Fecha { get; set; }

        public string Nombre { get => _Estado != LoteriaState.Error ? _Nombre : null; }

        public string FullNombre { get => $"{Nombre}_{SubCodigo}"; }
        public string SubCodigo { get => _SubCodigo; }

        public string Estado
        {
            get => _Estado.GetDescription();
        }

        public bool IsOk() => _Estado == LoteriaState.Jugado;

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

        public LoteriaDTO ToDTO() => new LoteriaDTO(Nombre, SubCodigo, Estado, Numeros);
    }   

    public class LoteriaDTO
    {
        public string Nombre { get; set; }
        public string SubCodigo { get; set; }
        public string Estado { get; set; }

        public List<NumeroLoteriaDTO> Numeros { get; set; }

        public LoteriaDTO(string nombre, string subCodigo, string estado, List<NumeroLoteria> numeros)
        {
            Nombre = nombre;
            SubCodigo = subCodigo;
            Estado = estado;
            Numeros = numeros?.Select(n => n?.ToDTO()).ToList();
        }
    }
}
