using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core.Servicios
{
    public interface ITokenizerService<T>
    {
        public List<IValueMatcher<T>> By(T rawInput, params IValueMatcher<T>[] knownMatchers);
    }

    public class TokenizerService: ITokenizerService<string>
    {
        /// <summary>
        /// Returns an stream of known tokenizers
        /// </summary>
        /// <param name="rawInput"></param>
        /// <param name="knownMatchers"></param>
        /// <returns></returns>
        public List<IValueMatcher<string>> By(string rawInput, params IValueMatcher<string>[] knownMatchers)
        {
            var unknownMatcher = new AnyValueMatcher();

            var list = new List<IValueMatcher<string>>();

            foreach (var c in rawInput.ToCharArray().Select((x, i) => new { Value = x, Index = i }))
            {
                var known = knownMatchers.Select(m => { m.Next(c.Value, c.Index); return m; })
                                          .Where(m => m.Started());

                if (known.Any())
                {
                    known.Where(k => k.Matches()).Select(m =>
                    {
                        list.Add(m.New());
                        m.Reset();
                        return m;
                    })
                    .ToList();
                }
                else
                {
                    unknownMatcher.SetValue(c.Value.ToString());
                    unknownMatcher.Next(c.Value, c.Index);
                    list.Add(unknownMatcher.New());
                    unknownMatcher.Reset();
                }
            }

            return list;
        }
    }
}
