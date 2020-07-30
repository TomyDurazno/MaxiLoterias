using AngleSharp.Common;
using MaxiLoterias.Core.Extensions;
using MaxiLoterias.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Servicios
{
    #region Machinery

    public interface ICondicionDeJuego
    {
        public CondiciondeJuegoResult Fits(Loteria loteria);

        public string Nombre { get; }
    }

    public class CondiciondeJuegoResult
    {
        public string Condicion { get; set; }
        public List<NumeroLoteria> Matches { get; set; }
        public bool HasValue() => Matches.Any();
    }

    public interface IMultipleCondicionDeJuegoServicio
    {
        public List<CondiciondeJuegoResult> Matches(Loteria loteria, params ICondicionDeJuego[] condiciones);
    }

    public class MultipleCondicionDeJuegoMatcher : IMultipleCondicionDeJuegoServicio
    {
        public List<CondiciondeJuegoResult> Matches(Loteria loteria, params ICondicionDeJuego[] condiciones)
        {
            var matches = new List<CondiciondeJuegoResult>();

            foreach (var condition in condiciones)
            {
                var result = condition.Fits(loteria);
                if(result?.HasValue() == true)
                {
                    matches.Add(result);
                }
            }
            
            return matches;
        }
    }

    #endregion

    #region Condiciones

    public class TieneCapicuas : ICondicionDeJuego
    {
        public string Nombre { get => "Tiene Capicúas";  }

        public bool Condition(NumeroLoteria numero)
        {
            var value = numero.Value();

            var reversed = numero.Value().ToString().Reverse();

            var reversedNum = reversed.Pipe(c => string.Concat(c));

            var hasFour = reversed.ToArray().Count() == 4;

            return value == Convert.ToInt32(reversedNum) && hasFour;
        }

        public CondiciondeJuegoResult Fits(Loteria loteria)
        {
            if (!loteria.IsOk())
                return null;

            var matches = new List<NumeroLoteria>();

            foreach (var numero in loteria.Numeros)
            {
                if (numero.HasValue() && Condition(numero))
                    matches.Add(numero);
            }

            return new CondiciondeJuegoResult()
            {
                Condicion = Nombre,
                Matches = matches
            };
        }
    }

    public class Pares : ICondicionDeJuego
    {
        public string Nombre { get => "Pares";  }

        public bool Condition(NumeroLoteria numero)
        {
            return numero.Value() % 2 == 0;
        }

        public CondiciondeJuegoResult Fits(Loteria loteria)
        {
            if (!loteria.IsOk())
                return null;

            var matches = new List<NumeroLoteria>();

            foreach (var numero in loteria.Numeros)
            {
                if (numero.HasValue() && Condition(numero))
                    matches.Add(numero);
            }

            return new CondiciondeJuegoResult()
            {
                Condicion = Nombre,
                Matches = matches
            };
        }
    }

    public class NumerosRepetidos : ICondicionDeJuego
    {
        public string Nombre { get => "Numeros Repetidos"; }

        public CondiciondeJuegoResult Fits(Loteria loteria)
        {
            if (!loteria.IsOk())
                return null;

            var matches = new List<NumeroLoteria>();

            foreach (var numero in loteria.Numeros)
            {
                if (numero.HasValue() && loteria.Numeros.Any(ln => ln.Name() != numero.Name() && numero.Value() == ln.Value()))
                    matches.Add(numero);
            }

            return new CondiciondeJuegoResult()
            {
                Condicion = Nombre,
                Matches = matches,
            };
        }
    }

    public class TieneDosProximosIguales : ICondicionDeJuego
    {
        public string Nombre { get => "TieneDosProximosIguales"; }

        bool Criteria(NumeroLoteria numero)
        {
            var arr = numero.Value().ToString().ToCharArray().Select(c => Convert.ToInt32(c)).ToArray();

            var adjacentDuplicate = arr.Skip(1).Where((value, index) => value == arr[index]).Distinct();

            if (adjacentDuplicate.Any())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public CondiciondeJuegoResult Fits(Loteria loteria)
        {
            if (!loteria.IsOk())
                return null;

            var matches = new List<NumeroLoteria>();

            foreach (var numero in loteria.Numeros)
            {
                if (numero.HasValue() && Criteria(numero))
                    matches.Add(numero);
            }

            return new CondiciondeJuegoResult()
            {
                Condicion = Nombre,
                Matches = matches
            };
        }
    }

    public class TresIgualesUnoDistinto : ICondicionDeJuego
    {
        public string Nombre { get => "TresIgualesUnoDistinto"; }

        bool Criteria(NumeroLoteria numero)
        {
            var arr = numero.Value().ToString().ToCharArray().Select(c => Convert.ToInt32(c)).ToArray();

            return arr.GroupBy(r => r).Any(g => g.ToList().Count() == 3);
        }

        public CondiciondeJuegoResult Fits(Loteria loteria)
        {
            if (!loteria.IsOk())
                return null;

            var matches = new List<NumeroLoteria>();

            foreach (var numero in loteria.Numeros)
            {
                if (numero.HasValue() && Criteria(numero))
                    matches.Add(numero);
            }

            return new CondiciondeJuegoResult()
            {
                Condicion = Nombre,
                Matches = matches
            };
        }
    }

    public class NumerosSeRepitenEnLasUltimasDos : ICondicionDeJuego
    {
        public string Nombre { get => "NumerosSeRepitenEnLasUltimasDos"; }

        public CondiciondeJuegoResult Fits(Loteria loteria)
        {
            if (!loteria.IsOk())
                return null;

            var has20 = loteria.Numeros.Count() == 20;

            if (!has20)
                return null;

            var matches = new List<NumeroLoteria>();

            var reversed = loteria.Numeros.AsEnumerable().Reverse();

            var last = reversed.Take(1).FirstOrDefault();

            var ante = reversed.Skip(1).Take(1).FirstOrDefault();

            var anteAsIntArray = ante.AsIntArray();

            foreach (var inLast in last.AsIntArray())
            {
                if(anteAsIntArray.Any(n => n == inLast))
                {
                    return new CondiciondeJuegoResult()
                    {
                        Condicion = Nombre,
                        Matches = matches
                    };
                }
            }

            return null;
        }
    }


    #endregion
}
