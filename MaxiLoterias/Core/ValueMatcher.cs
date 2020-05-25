using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MaxiLoterias.Core
{
    #region ValueMatcher

    #endregion

    #region Core

    public interface IValueMatcher<T>
    {
        public IValueMatcher<T> New();

        public T Value { get; }
        public int Start { get; }
        public int End { get; }

        public void SetValue(T s);
        public bool Started();
        public bool Matches();
        public bool Next(char c, int i);
        public void Reset();
    }

    public class ValueMatcher : IValueMatcher<string>
    {
        string value;
        int index = 0;
        bool notStarted = true;
        int start;
        int end;

        public ValueMatcher(string s)
        {
            value = s;
        }

        public void SetValue(string s)
        {
            value = s;
        }

        ValueMatcher(string s, int _start, int _end)
        {
            value = s;
            start = _start;
            end = _end;
        }

        public IValueMatcher<string> New()
        {
            return new ValueMatcher(value, start, end);
        }

        public Type Typeof => this.GetType();

        public string Value { get => value; }
        public int Start { get => start; }
        public int End { get => end; }

        public bool Started() => !notStarted;

        public bool Matches() => value.Length == index;

        public bool Next(char c, int i)
        {
            var flag = value.ElementAtOrDefault(index) == c;

            if (flag)
            {
                if (notStarted)
                {
                    start = i;
                    notStarted = false;
                }

                index++;

                if (Matches())
                {
                    end = i;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            index = 0;
            notStarted = true;
            start = 0;
            end = 0;
        }
    }

    #endregion

    public class AnyValueMatcher : IValueMatcher<string>
    {
        string value;
        int index = 0;
        bool started = true;
        int start;
        int end;

        public AnyValueMatcher()
        {

        }

        public void SetValue(string s)
        {
            value = s;
        }

        AnyValueMatcher(string s, int _start, int _end)
        {
            value = s;
            start = _start;
            end = _end;
        }

        public Type Typeof => this.GetType();

        public string Value => value;

        public int Start => start;

        public int End => end;

        public bool Started() => true;

        public bool Matches() => value.Length == index;

        public IValueMatcher<string> New()
        {
            return new AnyValueMatcher(value, start, end);
        }

        public bool Next(char c, int i)
        {
            //var flag = value.ElementAtOrDefault(index) == c;

            var flag = true;

            if (flag)
            {
                if (started)
                {
                    start = i;
                    started = false;
                }

                index++;

                if (Matches())
                {
                    end = i;
                }

                return true;
            }
            else
            {
                return false;
            }
        }

        public void Reset()
        {
            index = 0;
            started = true;
            start = 0;
            end = 0;
        }
    }

    public class WhiteTriviaValueMatcher : ValueMatcher
    {
        public WhiteTriviaValueMatcher(string s = " ") : base(s)
        {

        }
    }
}
