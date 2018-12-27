using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;

namespace Itec
{
    public abstract class Copier
    {
        private Func<object, object, object> _ObjectCopy;
        public Copier(Type srcType, Type destType, string fieldnames) {
            this.SrcType = srcType;
            this.DestType = destType;
            this.Fieldnames = fieldnames;
        }
        public Type SrcType { get; private set; }
        public Type DestType { get; private set; }

        public string Fieldnames { get;private set; }

        

        public object Copy(object src, object dest = null)
        {
            if (_ObjectCopy == null)
            {
                lock (this)
                {
                    if (_ObjectCopy == null)
                    {
                        var srcExpr = Expression.Parameter(typeof(object), "paramSrc");
                        var destExpr = Expression.Parameter(typeof(object), "destSrc");
                        var body = this.MakeObjectCopyExpression(srcExpr, destExpr);
                        var lamda = Expression.Lambda<Func<object, object, object>>(body, srcExpr, destExpr);
                        this._ObjectCopy = lamda.Compile();
                    }
                }
            }
            return _ObjectCopy(src, dest);
        }


        protected List<MemberInfo> GetMembers()
        {
            var all = new List<MemberInfo>();
            var selected = new List<MemberInfo>();
            var fields = string.IsNullOrWhiteSpace(this.Fieldnames) ? new string[] { } : this.Fieldnames.Split(',');
            var members = this.SrcType.GetMembers();
            foreach (var member in members)
            {
                if (member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property)
                {
                    var prop = member as PropertyInfo;
                    if (prop != null && (!prop.CanRead || prop.GetMethod == null)) continue;
                    all.Add(member);
                    if (fields.Contains(member.Name))
                    {
                        selected.Add(member);
                    }
                }
            }
            if (fields.Length == 0) return all;
            if (selected.Count == 0) throw new InvalidProgramException("指定的拷贝字段不在源对象中");
            return selected;
        }

        //PropertyInfo NullableHasValueInfo = typeof(System.Nullable<>).GetProperty("HasValue");
        //PropertyInfo NullableValueInfo = typeof(System.Nullable<>).GetProperty("Value");

        protected Expression MakeObjectCopyExpression(ParameterExpression paramSrcExpr, ParameterExpression paramDestExpr)
        {
            var locals = new List<ParameterExpression>();
            var codes = new List<Expression>();
            ParameterExpression srcExpr = null;
            if (paramSrcExpr.Type == this.SrcType) srcExpr = paramSrcExpr;
            else
            {
                srcExpr = Expression.Parameter(this.SrcType, "src");
                locals.Add(srcExpr);
                codes.Add(Expression.Assign(srcExpr, Expression.Convert(paramSrcExpr, this.SrcType)));
            }


            ParameterExpression destExpr = null;
            if (paramDestExpr.Type == this.DestType) destExpr = paramDestExpr;
            else
            {
                destExpr = Expression.Parameter(this.DestType, "dest");
                locals.Add(destExpr);
                codes.Add(Expression.Assign(destExpr, Expression.Convert(paramDestExpr, this.DestType)));
            }

            var nullCheckExpr = Expression.Equal(destExpr, Expression.Constant(null, this.DestType));
            var ctor = this.DestType.GetConstructor(Type.EmptyTypes);
            codes.Add(Expression.IfThen(nullCheckExpr, Expression.Assign(destExpr, Expression.New(ctor))));
            var srcMembers = GetMembers();
            var destMembers = this.DestType.GetMembers();
            foreach (var srcMember in srcMembers)
            {
                var destMember = destMembers.FirstOrDefault(p => p.Name == srcMember.Name && (p.MemberType == MemberTypes.Field || p.MemberType == MemberTypes.Property));
                if (destMember == null) continue;
                var prop = destMember as PropertyInfo;
                if (prop != null && (!prop.CanWrite || prop.SetMethod == null)) continue;
                var pname = srcMember.Name;
                var srcValueType = srcMember.MemberType == MemberTypes.Field ? (srcMember as FieldInfo).FieldType : (srcMember as PropertyInfo).PropertyType;
                var destValueType = destMember.MemberType == MemberTypes.Field ? (destMember as FieldInfo).FieldType : (destMember as PropertyInfo).PropertyType;
                if (srcValueType == destValueType)
                {
                    codes.Add(Expression.Assign(Expression.PropertyOrField(destExpr, pname), Expression.PropertyOrField(srcExpr, pname)));
                    continue;
                }

                var srcValueNullable = srcValueType.FullName.StartsWith("System.Nullable`1");
                var destValueNullable = destValueType.FullName.StartsWith("System.Nullable`1");
                if (!srcValueNullable && !destValueNullable) continue;
                if (srcValueNullable) {
                    var srcInnerType = srcValueType.GetGenericArguments()[0];
                    if (srcInnerType != destValueType) continue;
                    var srcPropValueExpr = Expression.Property(Expression.PropertyOrField(srcExpr, pname), "Value");
                    var nullableChkExpr = Expression.Property(Expression.PropertyOrField(srcExpr, pname),"HasValue");
                    codes.Add(Expression.IfThen(nullableChkExpr,Expression.Assign(Expression.PropertyOrField(destExpr,pname), srcPropValueExpr)));
                    continue;
                }
                if (destValueNullable)
                {
                    var destInnerType = destValueType.GetGenericArguments()[0];
                    if (destInnerType != srcValueType) continue;
                    var nullableCtor = destValueType.GetConstructors().FirstOrDefault(c=>c.GetParameters().Length==1);
                    var nullableValueExpr = Expression.New(nullableCtor, Expression.PropertyOrField(srcExpr, pname));
                    codes.Add(Expression.Assign(Expression.PropertyOrField(destExpr, pname), nullableValueExpr));

                    continue;
                }

            }

            //if (paramSrcExpr.Type != this.SrcType) {
            //    codes.Add(Expression.Assign(paramDestExpr,Expression.Convert(destExpr,paramDestExpr.Type)));
            //}
            var retLabel = Expression.Label(paramDestExpr.Type,"return_label");
            var retValueExpr = paramDestExpr.Type == this.DestType ? destExpr as Expression : Expression.Convert(destExpr, typeof(object));
            var returnExpr = Expression.Return(retLabel, retValueExpr, paramDestExpr.Type);
            codes.Add(returnExpr);
            codes.Add(Expression.Label(retLabel, Expression.Constant(null, paramDestExpr.Type)));

            var block = Expression.Block(paramDestExpr.Type, locals, codes);
            return block;
        }
    }
}
