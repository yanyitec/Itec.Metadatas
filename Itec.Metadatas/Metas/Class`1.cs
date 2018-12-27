using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Itec.Metas
{
    public class Class<T> : Class,IEnumerable<Property<T>>
        //where T : class
    {
        public Class() : base(typeof(T)) {
            
        }
        
        public new Property<T> this[string name] {
            get {
                return base[name] as Property<T>;
            }
        }

        public TDest CopyTo<TDest>( T src, TDest dest = default(TDest), string fieldnames = null)
        {
            var copier = this.GetCopier(typeof(TDest), fieldnames) as Copier<T,TDest>;
            return copier.Copy(src, dest);
        }

        

        T Clone(T src)
        {
            return this.CopyTo<T>(src);
        }

        

        protected override Property CreateProperty(MemberInfo memberInfo)
        {
            return new Property<T>(memberInfo,this);
        }
        protected override Method CreateMethod(MethodInfo methodInfo)
        {
            return new Method<T>(methodInfo, this);
        }
        IEnumerator<Property<T>> IEnumerable<Property<T>>.GetEnumerator()
        {
            return new Itec.ConvertEnumerator<Property<T>,Property>(this.Props.Values.GetEnumerator(),(src)=>(Property<T>)src);
        }
    }
}
