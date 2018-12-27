using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Itec.Metas
{
    public class Method<T>:Method
    {
        public Method(MethodInfo method, Class<T> cls) : base(method, cls) {

        }

        Func<T,IValueProvider, object> _DoCall;

        public object Call(T instance,IValueProvider valueProvider)
        {
            if (_DoCall == null)
            {
                lock (this)
                {
                    if (_DoCall == null) _DoCall = MakeCall<T>(this);
                }

            }
            return _DoCall(instance,valueProvider);
        }

        
    }
}
