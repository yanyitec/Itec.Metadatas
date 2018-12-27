using Itec.Metadatas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.ORM
{
    public class SQL
    {
        public IDictionary<Guid, string> DataTypeMaps { get; set; }

        public EscapeCharacters EscapeCharacters { get; set; }

        public string PrimaryKeyword { get; set; }



        public string CreateTableSql(Class cls, string prefix)
        {
            var sb = new StringBuilder("CREATE TABLE ").Append(prefix);
            var tbAttr = cls.GetAttribute<TableAttribute>();
            var tbName =  tbAttr?.Name ?? cls.Name;
            if (EscapeCharacters != null && EscapeCharacters.NameStart != '\0') sb.Append(EscapeCharacters.NameStart);
            sb.Append(tbName);
            if (EscapeCharacters != null && EscapeCharacters.NameEnd != '\0') sb.Append(EscapeCharacters.NameEnd);

            sb.Append("(");
            bool hasId = false;
            foreach (var prop in cls)
            {
                if (prop.GetAttribute<NotFieldAttribute>() != null) continue;
                var fieldName = prop.GetAttribute<FieldAttribute>()?.Name ?? prop.Name;
                bool isPrimary = false;
                if (fieldName == "Id" || fieldName == "id" || fieldName == "ID" || fieldName == cls.Name + "Id" || fieldName == cls.Name + "ID") {
                    hasId = true;
                    isPrimary = true;
                }
            }

            return sb.ToString();

        }
    }
}
