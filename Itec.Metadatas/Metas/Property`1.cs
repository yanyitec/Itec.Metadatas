using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Itec.Metas
{
    public class Property<T>:Property
        //where T:class
    {
        public Property(MemberInfo memberInfo,Class<T> cls=null):base(memberInfo,cls)
        {
            
        }

        public new Class<T> Class { get; private set; }
        #region GetValue
        Func<T, object> _GetValue;
        public object GetValue(T instance)
        {
            if (_GetValue == null)
            {
                lock (this)
                {
                    if (_GetValue == null) _GetValue = MakeGetter(this.MemberInfo);
                }
            }
            return _GetValue(instance);
        }



        static Func<T, object> MakeGetter(MemberInfo memberInfo)
        {
            var instanceParameterExpr = Expression.Parameter(typeof(T), "instanceParameter");
            var bodyExpr = MakeGetterExpression(memberInfo, instanceParameterExpr);
            var lamda = Expression.Lambda<Func<T, object>>(bodyExpr, instanceParameterExpr);
            return lamda.Compile();
        }
        #endregion

        #region setValue


        Action<T, object> _SetValue;
        public void SetValue(T instance, object value)
        {
            if (_SetValue == null)
            {
                lock (this)
                {
                    if (_SetValue == null) _SetValue = MakeSetter(this.MemberInfo);
                }
            }
            _SetValue(instance, value);
        }
        static Action<T, object> MakeSetter(MemberInfo memberInfo)
        {
            var instanceParameterExpr = Expression.Parameter(typeof(T), "instanceParameter");
            var valueParameterExpr = Expression.Parameter(typeof(object), "valueParameter");
            var bodyExpr = MakeSetterExpression(memberInfo, instanceParameterExpr, valueParameterExpr);
            var lamda = Expression.Lambda<Action<T, object>>(bodyExpr, instanceParameterExpr, valueParameterExpr);
            return lamda.Compile();
        }
        #endregion
    }
}
