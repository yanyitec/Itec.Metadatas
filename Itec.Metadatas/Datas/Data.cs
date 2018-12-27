using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Itec.Datas
{
    public class Data :   ReadOnlyData, IData
    {
        
        public Data(JObject data = null):base(data)
        {
        }

        public Data(string json):base(json)
        {
        }



        public IData Remove(string key) {
            this.Internals.Remove(key);
            this.HasChanges = true;
            return this;
        }

        public new string this[string key] {
            get { return GetString(key); }
            set { SetString(key,value); }
        }

        

        public IData SetString(string key, string value) {
            Internals[key] = value;
            this.HasChanges = true;
            return this;
        }

        public IData SetJToken(string key, JToken value)
        {
            Internals[key] = value;
            this.HasChanges = true;
            return this;
        }



        public IData Set<T>(string key, T value)
        {
            var token = JToken.FromObject(value);
            this.Internals[key] = token;
            this.HasChanges = true;
            return this;
        }

        

        public IData Set(string key, object value)
        {
            var token = JToken.FromObject(value);
            this.Internals[key] = token;
            this.HasChanges = true;
            return this;
        }

        
        #region dynamic
        

        

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.Set(binder.Name,value);
            return true;
        }

        

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            var key = indexes[0];

            this.Set(key == null ? null : key.ToString(), binder.ReturnType);
            return true;
        }

        


        #endregion

    }
}
