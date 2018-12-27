using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.ORM
{
    public class TableDefination
    {
        public TableDefination(JObject obj) {
            var tb = obj["$table"];
            if (tb != null) this.Table = tb.ToString().Trim();
            this._Properties = new Dictionary<string, FieldDefination>();
            foreach (var pair in obj) {
                if (pair.Key.Length == 0 || pair.Key[0]=='@') continue;
                var field = (pair.Value as JObject)?.ToObject<FieldDefination>();
                if (field == null) continue;
                if (field.Field == "#Ignore" || field.NotField == true) continue;
                this._Properties.Add(pair.Key,field);
            }
        }
        public string Table { get; set; }



        Dictionary<string, FieldDefination> _Properties;
        public IReadOnlyDictionary<string, FieldDefination> Properties { get { return _Properties; } }
    }
}
