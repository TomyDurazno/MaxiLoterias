using MaxiLoterias.Core.Extensions;
using MaxiLoterias.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MaxiLoterias.Core.Servicios
{
    #region Machinery

    public interface ICondicionDeJuego
    {
        public CondicionDeJuegoResult Fits(Loteria loteria);

        public string Nombre { get; }
    }

    public class CondicionDeJuegoResult
    {
        public string Condicion { get; set; }        
        public List<NumeroLoteria> Matches { get; set; }
        public bool HasValue() => Matches.Any();
    }

    public class MultipleCondicionResult
    {
        public string Nombre { get; set; }
        public List<CondicionDeJuegoResult> Condiciones { get; set; }
    }

    public class JuegosResult
    {
        public List<string> Condiciones { get; set; }
        public List<MultipleCondicionResult> Resultados { get; set; }
    }

    public interface IMultipleCondicionDeJuegoServicio
    {
        public MultipleCondicionResult Matches(Loteria loteria, params ICondicionDeJuego[] condiciones);

        public JuegosResult Matches(LoteriaResult loterias, params ICondicionDeJuego[] condiciones);
    }

    public class MultipleCondicionDeJuegoMatcherServicio : IMultipleCondicionDeJuegoServicio
    {
        public MultipleCondicionResult Matches(Loteria loteria, params ICondicionDeJuego[] condiciones)
        {
            var matches = new List<CondicionDeJuegoResult>();

            foreach (var condition in condiciones)
            {
                var result = condition.Fits(loteria);
                if(result?.HasValue() == true)
                {
                    matches.Add(result);
                }
            }

            var multiple = new MultipleCondicionResult();

            multiple.Nombre = loteria.Nombre;
            multiple.Condiciones = matches;

            return multiple;
        }

        public JuegosResult Matches(LoteriaResult result, params ICondicionDeJuego[] condiciones)
        {
            var matches = new List<MultipleCondicionResult>();

            foreach (var loteria in result.Loterias)
            {
                matches.Add(Matches(loteria, condiciones));
            }

            return new JuegosResult
            {
                Condiciones = condiciones.Select(m => m.Nombre).ToList(),
                Resultados = matches
            };
        }
    }

    #endregion

    #region Condiciones De Juego

    public class Condiciones
    {
        public static ICondicionDeJuego[] DeJuego => typeof(Condiciones).GetNestedTypes().Select(t => (ICondicionDeJuego)Activator.CreateInstance(t)).ToArray();

        public class Capicuas : ICondicionDeJuego
        {
            public string Nombre { get => "Capicúas"; }

            public bool Condition(NumeroLoteria numero)
            {
                var value = numero.Value();

                var reversed = numero.Value().ToString().Reverse();

                var reversedNum = reversed.Pipe(string.Concat, Convert.ToInt32);

                return reversed.Count() >= 3 && (value == reversedNum);
            }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (numero.HasValue() && Condition(numero))
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Condicion = Nombre,
                    Matches = matches
                };
            }
        }

        public class Pares : ICondicionDeJuego
        {
            public string Nombre { get => "Pares"; }

            public bool Condition(NumeroLoteria numero)
            {
                return numero.Value() % 2 == 0;
            }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (numero.HasValue() && Condition(numero))
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Condicion = Nombre,
                    Matches = matches
                };
            }
        }

        public class NumerosRepetidos : ICondicionDeJuego
        {
            public string Nombre { get => "Números Repetidos"; }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (numero.HasValue() && loteria.Numeros.Any(ln => ln.Position() != numero.Position() && numero.Value() == ln.Value()))
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Condicion = Nombre,
                    Matches = matches,
                };
            }
        }

        public class DosProximosIguales : ICondicionDeJuego
        {
            public string Nombre { get => "Dos Números Próximos Iguales"; }

            bool Criteria(NumeroLoteria numero)
            {
                var arr = numero.Value().ToString().ToCharArray().Select(c => Convert.ToInt32(c)).ToArray();

                var adjacentDuplicate = arr.Skip(1).Where((value, index) => value == arr[index]).Distinct();

                return adjacentDuplicate.Any();
            }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (numero.HasValue() && Criteria(numero))
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Condicion = Nombre,
                    Matches = matches
                };
            }
        }

        public class TresIgualesUnoDistinto : ICondicionDeJuego
        {
            public string Nombre { get => "Tres Iguales Uno Distinto"; }

            bool Criteria(NumeroLoteria numero) => numero.AsIntArray()
                                                         .GroupBy(r => r)
                                                         .Any(g => g.ToList().Count() == 3);

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (numero.HasValue() && Criteria(numero))
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Condicion = Nombre,
                    Matches = matches
                };
            }
        }

        public class NumerosSeRepitenEnLasUltimasDos : ICondicionDeJuego
        {
            public string Nombre { get => "Números Se Repiten En Las Ultimas Dos"; }

            public CondicionDeJuegoResult Fits(Loteria loteria)
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
                    if (anteAsIntArray.Any(n => n == inLast))
                    {
                        return new CondicionDeJuegoResult()
                        {
                            Condicion = Nombre,
                            Matches = matches
                        };
                    }
                }

                return null;
            }
        }

        public class EsNumeroPrimo : ICondicionDeJuego
        {
            public string Nombre { get => "Es Número Primo"; }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (numero.HasValue() && numero.Value().IsPrime())
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }

        public class RepetidosEnFila : ICondicionDeJuego
        {
            public string Nombre => "Repetidos en Fila";

            bool MatchesCriteria(NumeroLoteria first, NumeroLoteria last)
            {
                var firstArr = first.AsIntArray();
                var secondArr = last.AsIntArray();

                foreach (var i in Enumerable.Range(0, 4))
                {
                    if ((secondArr.ElementAtOrDefault(i) != default && firstArr.ElementAtOrDefault(i) != default) &&
                        (secondArr.ElementAtOrDefault(i) == firstArr.ElementAtOrDefault(i)))
                        return true;
                }

                return false;
            }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                var nums = loteria.Numeros.Select((l, i) => new { l, i }).ToArray();

                foreach (var num in nums)
                {
                    if(num.i != 0)
                    {
                        var first = nums[num.i - 1];

                        if (MatchesCriteria(first.l, num.l))
                            matches.AddRange(new[] { first.l, num.l });
                    }
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }

        public class TodosLosNúmerosPares : ICondicionDeJuego
        {
            public string Nombre { get => "Todos Los Números Pares"; }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (numero.HasValue() && numero.AsIntArray().All(n => n % 2 == 0))
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }

        public class Escalera : ICondicionDeJuego
        {
            public string Nombre => "Escalera";

            bool Ordered(int[] arr, Func<int, int, bool> predicate)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i != 0)
                        if (!predicate(arr[i - 1], arr[i]))
                            return false;
                }
                return true;
            }

            bool MatchesCriteria(NumeroLoteria num)
            {                
                var ordered = num.AsIntArray();

                if (ordered.Count() < 3)
                    return false;

                var reversed = num.AsIntArray().Reverse().ToArray();

                if (Ordered(ordered, (f, s) => (f - 1) == s))
                    return true;

                if (Ordered(reversed, (f, s) => (f + 1) == s))
                    return true;

                return false;
            }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var num in loteria.Numeros)
                {
                    if (MatchesCriteria(num))
                        matches.Add(num);
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }

        public class NumeroDeLost : ICondicionDeJuego
        {
            public string Nombre => "Numero de Lost (4, 8, 15, 16, 23, 42)";

            bool Ordered(int[] arr, Func<int, int, bool> predicate)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                    if (i != 0)
                        if (predicate(arr[i - 1], arr[i]))
                            return true;
                }
                return false;
            }

            bool IsNumber(int[] arr, Func<int, bool> predicate)
            {
                for (int i = 0; i < arr.Length; i++)
                {
                        if (predicate(arr[i]))
                            return true;
                }
                return false;
            }

            bool MatchesCriteria(NumeroLoteria num)
            {
                var nums = new [] { 4, 8, 15, 16, 23, 42};

                var ordered = num.AsIntArray();

                if (Ordered(ordered, (f, s) => $"{f}{s}".Pipe(Convert.ToInt32, n => nums.Any(n1 => n1 == n))))
                    return true;

                if (IsNumber(ordered, n => nums.Any(n1 => n1 == n)))
                    return true;

                return false;
            }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var num in loteria.Numeros)
                {
                    if (MatchesCriteria(num))
                        matches.Add(num);
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }

        public class CifrasSumanTrece : ICondicionDeJuego
        {
            public string Nombre => "Cifras Suman Trece";

            bool MatchesCriteria(NumeroLoteria num) => num.AsIntArray().Sum() == 13;

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var num in loteria.Numeros)
                {
                    if (MatchesCriteria(num))
                        matches.Add(num);
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }

        public class SaleElDiaDeJuego : ICondicionDeJuego
        {
            public string Nombre => "Sale El Dia De Juego";

            string AsZeroEd(int n) => n > 10 ? n.ToString() : $"0{n}";

            bool MatchesCriteria(NumeroLoteria num, DateTime date) 
            {
                return num.Value().ToString() == $"{date.Day.Pipe(AsZeroEd)}{date.Month.Pipe(AsZeroEd)}";
            }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var num in loteria.Numeros)
                {
                    if (MatchesCriteria(num, loteria.Fecha))
                        matches.Add(num);
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }

        public class NumerosClones : ICondicionDeJuego
        {
            public string Nombre { get => "Numeros Clones"; }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (loteria.Numeros.Any(nl => nl.Position() != numero.Position() && nl.Value() == numero.Value())) 
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }

        public class EmpiezaYTerminaConPosición : ICondicionDeJuego
        {
            public string Nombre { get => "Empieza y termina con la posición"; }

            bool Matches(NumeroLoteria numlot)
            {
                var arr = numlot.AsIntArray();

                if (arr.Count() < 3)
                    return false;

                var first = arr.First();
                var last = arr.Last();

                return numlot.Position() == first && (first == last);
            }

            public CondicionDeJuegoResult Fits(Loteria loteria)
            {
                if (!loteria.IsOk())
                    return null;

                var matches = new List<NumeroLoteria>();

                foreach (var numero in loteria.Numeros)
                {
                    if (Matches(numero))
                        matches.Add(numero);
                }

                return new CondicionDeJuegoResult()
                {
                    Matches = matches,
                    Condicion = Nombre
                };
            }
        }
    }

    #endregion
}
