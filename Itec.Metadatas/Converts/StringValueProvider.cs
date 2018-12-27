using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Metadatas
{
    public class StringValueProvider : IValueProvider
    {
        public StringValueProvider(IStringCollection collection) {
            this.StringCollection = collection;
        }

        public IStringCollection StringCollection { get; private set; }
        public Noneable<T> Get<T>(string name = null)
        {
            
            var t = typeof(T);
            if (t.IsClass) {
                var cls = ClassFactory.Default.GetClass(typeof(T));
                var inst = (T)cls.CreateInstance();
                var hasSetted = false;
                foreach (var prop in cls) {
                    var propValue = this.StringCollection[prop.Name];
                    if (propValue == null) continue;
                    prop.SetValue(inst,propValue);
                    hasSetted = true;

                }
                if (hasSetted) return new Noneable<T>(inst);
                else return new Noneable<T>();
            } else {
                var text = this.StringCollection[name];
                if (text == null) return new Noneable<T>();
                return ValConvert.ConvertTo<T>(text);
            }
        }
        public object GetRawValue(string name=null) {
            if (name == null) return this.StringCollection;
            return this.StringCollection[name];
        }
    }
}
