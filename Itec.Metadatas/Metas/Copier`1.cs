using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace Itec
{
    public class Copier<TSrc,TDest>:Copier
       
    {
        public Copier(string fieldnames):base(typeof(TSrc),typeof(TDest),fieldnames) {

        }

        
        Func<TSrc, TDest, TDest> _TypeCopy;

        public TDest Copy(TSrc src, TDest dest = default(TDest))
        {
            if (_TypeCopy == null)
            {
                lock (this)
                {
                    if (_TypeCopy == null)
                    {
                        var srcExpr = Expression.Parameter(this.SrcType, "paramSrc");
                        var destExpr = Expression.Parameter(this.DestType, "destSrc");
                        var body = this.MakeObjectCopyExpression(srcExpr, destExpr);
                        var lamda = Expression.Lambda<Func<TSrc, TDest, TDest>>(body, srcExpr, destExpr);
                        this._TypeCopy = lamda.Compile();
                    }
                }
            }
            return _TypeCopy(src, dest);
        }
        
    }
}
