using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Models
{
    public class Bloque
    {
        IEnumerable<IEnumerable<string>> _RawValue;

        public List<Loteria> Loterias { get; set; }
        DateTime Fecha { get; set; }

        public Bloque(IEnumerable<IEnumerable<string>> RawValue)
        {
            _RawValue = RawValue;
            Loterias = GetList().Select(l => new Loteria(l)).ToList();
        }

        public List<List<string>> GetList()
        {
            var arr = _RawValue.ToArray().Select(rv => rv.ToList());           

            int count = 0;
            List<List<string>> _Lists = new List<List<string>>();
            foreach (var element in arr)
            {
                if (count == 0)
                {
                    var r = arr.ElementAtOrDefault(4);

                    if (r != null)
                        element.AddRange(r);

                    _Lists.Add(element);
                }

                if (count == 1)
                {
                    var r = arr.ElementAtOrDefault(5);

                    if(r != null)
                        element.AddRange(r);

                    _Lists.Add(element);
                }

                if (count == 2)
                {
                    var r = arr.ElementAtOrDefault(6);

                    if (r != null)
                        element.AddRange(r);

                    _Lists.Add(element);
                }

                if (count == 3)
                {
                    var r = arr.ElementAtOrDefault(7);

                    if (r != null)
                        element.AddRange(r);

                    _Lists.Add(element);
                }

                count++;
            }

            return _Lists.Take(4).ToList();
        }
    }
}
