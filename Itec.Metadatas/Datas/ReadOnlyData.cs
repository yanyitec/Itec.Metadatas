using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Itec.Datas
{
    public class ReadOnlyData : System.Dynamic.DynamicObject, IReadOnlyData
    {
        
        public ReadOnlyData(JObject data = null)
        {
            this._Internals = data;
        }

        public ReadOnlyData(string json)
        {
            this._Internals = json == null ? new JObject() : JObject.Parse(json);
        }

        
        /// <summary>
        /// 状态是否有变更
        /// </summary>
        public bool HasChanges { get; protected internal set; }


        JObject _Internals;
        protected internal JObject Internals
        {
            get {
                
                return _Internals==null?(_Internals = new JObject()):_Internals;
            }
            set
            {
                _Internals = value;
                if (_Internals == null) _Internals = new JObject();
                HasChanges = true;
            }
        }


        
        public string this[string key] {
            get { return this.GetString(key); }
        }



        public string GetString(string key)
        {
            var token = Internals[key];
            if (token == null || token.Type == JTokenType.Undefined || token.Type == JTokenType.Null) return null;
            return token.ToString();
        }



        public JToken GetJToken(string key) {
            return this.Internals[key];
        }





        public T Get<T>(string key)
        {
            var token = Internals[key];
            if (token == null || token.Type == JTokenType.Undefined || token.Type == JTokenType.Null) return default(T);
            return token.ToObject<T>();
        }


        public object Get(string key, Type type)
        {
            var token = Internals[key];
            if (token == null || token.Type == JTokenType.Undefined || token.Type == JTokenType.Null) return null;

            return token.ToObject(type);
        }

        public string ToJSON()
        {
            return this.Internals.ToString();
        }

        public static implicit  operator JObject(ReadOnlyData me){
            return me._Internals;
        }

        #region dynamic
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var names = new List<string>();
            foreach (var pair in this._Internals) names.Add(pair.Key);
            return names;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = this.Get(binder.Name, binder.ReturnType);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            var key = indexes[0];

            result = this.Get(key == null ? null : key.ToString(), binder.ReturnType);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return false;
        }

        public override bool TryConvert(ConvertBinder binder, out object result)
        {
            result = _Internals.ToObject(binder.ReturnType);
            return true;
        }

        public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
        {
            throw new InvalidOperationException("不能当作函数使用");
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            throw new InvalidOperationException("成员不能当作函数使用，成员不能被调用");
        }


        #region enumeratable
        public class Enumerator:IEnumerator<KeyValuePair<string, object>> {
            public Enumerator(ReadOnlyData state) {
                this.InnerEnumerator = state._Internals?.GetEnumerator();
            }

            public IEnumerator<KeyValuePair<string, JToken>> InnerEnumerator { get; private set; }

            public KeyValuePair<string, object> Current => new KeyValuePair<string,object>(this.InnerEnumerator.Current.Key,this.InnerEnumerator.Current.Value);

            object IEnumerator.Current => this.InnerEnumerator.Current;

            public void Dispose()
            {
                this.InnerEnumerator?.Dispose();
            }

            public bool MoveNext()
            {
                return this.InnerEnumerator==null?false:this.InnerEnumerator.MoveNext();
            }

            public void Reset()
            {
                this.InnerEnumerator?.Reset();
            }
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator(this);
        }
        #endregion
        #endregion

    }
}
