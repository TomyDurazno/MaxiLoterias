using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core
{
    public class StateInternal
    {
        public static IEnumerable<string> Numbers()
        {
            int cont = 65;
            while (cont < 91)
            {
                yield return ((char)cont).ToString();
                cont++;
            }
        }

        int counter;
        bool hasChanged;
        IEnumerator<string> enumerator;
        bool firstTime;
        int _top;

        public StateInternal(int top)
        {
            counter = 0;
            _top = top;
            enumerator = Numbers().GetEnumerator();
            firstTime = true;
        }

        public bool HasStateChanged()
        {
            hasChanged = false;

            if (counter >= _top)
            {
                hasChanged = true;
                counter = 0;
            }

            counter++;
            return hasChanged;
        }

        public string Letter()
        {
            if (HasStateChanged() || firstTime)
            {
                enumerator.MoveNext();
                firstTime = false;
            }

            return enumerator.Current;
        }
    }
}
