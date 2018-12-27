using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public class ConvertEnumerator<TDest, TSrc> : IEnumerator<TDest>
    {
        public ConvertEnumerator(IEnumerator<TSrc> src, Func<TSrc, TDest> convertor)
        {
            this.SourceEnumerator = src;
            this.Convertor = convertor;
        }

        public ConvertEnumerator(IEnumerable<TSrc> src, Func<TSrc, TDest> convertor)
        {
            this.SourceEnumerator = src.GetEnumerator();
            this.Convertor = convertor;
        }


        public Func<TSrc, TDest> Convertor { get; private set; }

        public IEnumerator<TSrc> SourceEnumerator { get; private set; }
        public TDest Current => Convertor(SourceEnumerator.Current);

        object IEnumerator.Current => SourceEnumerator.Current;

        public void Dispose()
        {
            SourceEnumerator.Dispose();
        }

        public bool MoveNext()
        {
            return SourceEnumerator.MoveNext();
        }

        public void Reset()
        {
            SourceEnumerator.Reset();
        }
    }
}
