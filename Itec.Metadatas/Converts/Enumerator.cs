using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Itec
{
    public class Enumerator<TDest, TSrc> : IEnumerator<TDest>
        where TSrc:TDest
    {
        public Enumerator(IEnumerator<TSrc> src) {
            this.SourceEnumerator = src;
            //this.Convertor = convertor;
        }

        public Enumerator(IEnumerable<TSrc> src)
        {
            this.SourceEnumerator = src.GetEnumerator();
            //this.Convertor = convertor;
        }

        //public Func<TSrc, TDest> Convertor { get; private set; }

        public IEnumerator<TSrc> SourceEnumerator { get; private set; }
        public TDest Current => (TDest)SourceEnumerator.Current;

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
