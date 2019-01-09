using Itec.Metas;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Text;

namespace Itec.Datas
{
    public class StringDataSource : System.Dynamic.DynamicObject, IDataSource
    {
        public StringDataSource(IStringCollection collection) {
            this.StringCollection = collection;
        }

        public string this[string key] => this.StringCollection[key];

        public IStringCollection StringCollection { get; private set; }

        public bool HasChanges => false;

        public T Get<T>(string key = null)
        {
            var nullable = this.GetNoneable<T>(key);
            return nullable.GetValue();
        }

        public object Get(Type objectType, string key = null)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            //return this.StringCollection.GetEnumerator();
            return new ConvertEnumerator<KeyValuePair<string,object>, KeyValuePair<string,string>>(this.StringCollection,(src)=>new KeyValuePair<string, object>(src.Key,src.Value));
        }

        
        public Noneable<T> GetNoneable<T>(string name = null)
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
        public object GetRaw(string name=null) {
            if (name == null) return this.StringCollection;
            return this.StringCollection[name];
        }

        public string GetString(string key = null)
        {
            if (key == null) return this.StringCollection.ToString();
            else return this.StringCollection[key];
        }

        public string ToJSON()
        {
            return this.StringCollection.ToJSON();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.StringCollection.GetEnumerator();
        }

        
    }
}
