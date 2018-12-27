using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Itec.Metas
{
    public class Method
    {
        public Method(MethodInfo methodInfo, Class cls = null)
        {
            this.MethodInfo = methodInfo;
            this.Class = cls;
            
            
        }
        public Class Class { get; private set; }

        public MethodInfo MethodInfo { get; private set; }
        public string Name { get { return this.MethodInfo.Name; } }

        List<Attribute> _Attributes;
        public IReadOnlyList<Attribute> Attributes
        {
            get
            {
                if (_Attributes == null)
                {
                    lock (this)
                    {
                        if (_Attributes == null)
                        {
                            _Attributes = new List<Attribute>(this.MethodInfo.GetCustomAttributes());
                        }
                    }
                }
                return _Attributes;
            }
        }

        public T GetAttribute<T>() where T : Attribute
        {
            return this.Attributes.FirstOrDefault(p => p.GetType() == typeof(T)) as T;
        }

        

        List<ParameterInfo> _Parameters;
        public IReadOnlyList<ParameterInfo> Parameters
        {
            get
            {
                if (_Parameters == null)
                {
                    lock (this)
                    {
                        if (_Parameters == null)
                        {
                            _Parameters = new List<ParameterInfo>(this.MethodInfo.GetParameters());
                        }
                    }
                }
                return _Parameters;
            }
        }

        public Type ReturnType { get { return this.MethodInfo.ReturnType; } }

        Func<object,IValueProvider, object> _DoCall;

        public object Call(object instance,IValueProvider valueProvider) {
            if (_DoCall == null) {
                lock (this) {
                    if (_DoCall == null) _DoCall = MakeCall<object>(this);
                }

            }
            return _DoCall(instance,valueProvider);
        }

        #region Call
        static MethodInfo GetMethod = typeof(IValueProvider).GetMethod("Get", 1, new Type[] { typeof(string) });
        static Type NullableType = typeof(Itec.Noneable<>);

        protected static Func<T, IValueProvider, object> MakeCall<T>(Method method)
        {
            var instanceExpr = Expression.Parameter(typeof(T), "instance");
            var providerExpr = Expression.Parameter(typeof(IValueProvider), "valueProvider");
            var lamda = Expression.Lambda<Func<T, IValueProvider, object>>(MakeCall(method, instanceExpr, providerExpr), instanceExpr, providerExpr);
            return lamda.Compile();
        }
        protected static Expression MakeCall(Method method,ParameterExpression instanceExpr, ParameterExpression valueProviderExpr) {

            var argList = new List<Expression>();
            var locals = new List<ParameterExpression>();
            var codes = new List<Expression>();
            if (instanceExpr.Type != method.Class.Type)
            {
                var instExpr = Expression.Parameter(method.Class.Type, "inst");
                locals.Add(instExpr);
                codes.Add(Expression.Assign(instExpr, Expression.Convert(instanceExpr, method.Class.Type)));
                instanceExpr = instExpr;
            }

            foreach (var parInfo in method.Parameters) {
                var parType = parInfo.ParameterType;
                var nullableExpr = Expression.Parameter(NullableType.MakeGenericType(parInfo.ParameterType),parInfo.Name);
                locals.Add(nullableExpr);
                codes.Add(Expression.Assign(
                    nullableExpr,
                    Expression.Call(instanceExpr, GetMethod.MakeGenericMethod(parType), Expression.Constant(parInfo.Name))
                ));
                var actualValueExpr = Expression.Condition(
                    Expression.PropertyOrField(nullableExpr, "HasValue")
                    , Expression.PropertyOrField(nullableExpr,"Value")
                    ,parInfo.HasDefaultValue?(Expression)Expression.Constant(parInfo.DefaultValue,parInfo.ParameterType): Expression.PropertyOrField(nullableExpr, "Value")
                );
                argList.Add(actualValueExpr);

            }
            Expression callExpr = Expression.Call(instanceExpr,method.MethodInfo,argList);
            if (method.ReturnType != null && method.ReturnType != typeof(void))
            {
                var retTarget = Expression.Label(method.ReturnType);
                if (method.ReturnType == typeof(object))
                {
                    codes.Add(Expression.Return(retTarget, callExpr));

                }
                else {
                    codes.Add(Expression.Convert(Expression.Return(retTarget, callExpr), typeof(object)));

                }
                codes.Add(Expression.Label(retTarget));

            }
            else {
                var retTarget = Expression.Label(typeof(object));
                codes.Add(callExpr);
                codes.Add(Expression.Return(retTarget, callExpr));
                codes.Add(Expression.Label(retTarget));
            }
            return Expression.Block(method.ReturnType,locals,codes);
        }
        #endregion

    }
}
