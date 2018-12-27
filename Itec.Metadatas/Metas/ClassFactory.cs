using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Itec.Metas
{
    public class ClassFactory
    {
        public static ClassFactory Default = new ClassFactory();
        ConcurrentDictionary<Guid, Class> _Classes;
        public ClassFactory() {
            this._Classes = new ConcurrentDictionary<Guid, Class>();
        }
        public Class GetClass(Type type) {
            return _Classes.GetOrAdd(type.GUID,(k)=> {
                var t = typeof(Class<>).MakeGenericType(type);
                return Activator.CreateInstance(t, type) as Class;
            });
        }
    }
}
