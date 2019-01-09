using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Itec.Metas;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace Itec.Models
{
    /// <summary>
    /// 条件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Criteria<T>:Model
    {

        public Criteria(Class cls
            , Validations.ValidatorFactoryFactory validatorFactoryFactory
            , IConfigFactory configFactory
            , Expression<Func<T, bool>> initCriteria = null
            ):base(cls,validatorFactoryFactory,configFactory)
        {
            if (initCriteria != null)
            {
                this.FilterParameter = initCriteria.Parameters[0];
                this._FilterExpression = initCriteria.Body;
            }
            else
            {
                this.FilterParameter = System.Linq.Expressions.Expression.Parameter(typeof(T), "entity");
            }

        }




        [Newtonsoft.Json.JsonIgnore]
        public ParameterExpression FilterParameter { get; protected set; }
        protected Expression _FilterExpression;
        [Newtonsoft.Json.JsonIgnore]
        public Expression<Func<T, bool>> FilterExpression
        {
            get
            {
                if (this._FilterExpression == null) return null;
                return System.Linq.Expressions.Expression.Lambda<Func<T, bool>>(this._FilterExpression, this.FilterParameter);
            }

        }



        public Criteria<T> AndAlso(Expression<Func<T, bool>> criteria)
        {
            if (criteria == null) return this;
            if (this._FilterExpression == null)
            {
                this.FilterParameter = criteria.Parameters[0];
                this._FilterExpression = criteria.Body;
            }
            else
            {
                //this._Expression = System.Linq.Expressions.Expression.AndAlso(this._Expression, criteria);
                this._FilterExpression = System.Linq.Expressions.Expression.AndAlso(this.FilterExpression, Convert(criteria, criteria.Parameters[0]));
            }
            return this;
        }

        public Criteria<T> AndAlso(Criteria<T> criteria)
        {
            return this.AndAlso(criteria.FilterExpression);
        }

        public Criteria<T> OrElse(Expression<Func<T, bool>> criteria)
        {
            if (this._FilterExpression == null)
            {
                this.FilterParameter = criteria.Parameters[0];
                this._FilterExpression = criteria.Body;
            }
            else
            {
                //this._Expression = System.Linq.Expressions.Expression.OrElse(this._Expression, criteria);
                this._FilterExpression = System.Linq.Expressions.Expression.OrElse(this.FilterExpression, Convert(criteria, criteria.Parameters[0]));
            }
            return this;
        }

        public Criteria<T> OrElse(Criteria<T> criteria)
        {
            return this.OrElse(criteria.FilterExpression);
        }

        Expression Convert(Expression expr, ParameterExpression param)
        {
            if (expr == param) return this.FilterParameter;
            BinaryExpression bExpr = null;
            UnaryExpression uExpr = null;
            switch (expr.NodeType)
            {
                case ExpressionType.Lambda:
                    var lamda = (expr as LambdaExpression);
                    return Convert(lamda.Body, lamda.Parameters[0]);
                case ExpressionType.Constant:
                    return expr;
                case ExpressionType.And:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.And(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Add:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Add(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.AndAlso:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.OrElse(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.MemberAccess:
                    var member = expr as MemberExpression;
                    return System.Linq.Expressions.Expression.MakeMemberAccess(Convert(member.Expression, param), member.Member);
                case ExpressionType.Call:
                    var call = expr as MethodCallExpression;
                    var list = new List<Expression>();
                    foreach (var arg in call.Arguments)
                    {
                        list.Add(Convert(arg, param));
                    }
                    return System.Linq.Expressions.Expression.Call(Convert(call.Object, param), call.Method, list);
                case ExpressionType.Convert:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Convert(Convert(uExpr.Operand, param), uExpr.Type);
                case ExpressionType.Divide:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Divide(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Equal:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Equal(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.GreaterThan:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.GreaterThan(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.GreaterThanOrEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.GreaterThanOrEqual(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.LessThan:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LessThan(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.LessThanOrEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LessThan(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.LeftShift:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.LeftShift(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Multiply:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Multiply(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Negate:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Negate(Convert(uExpr.Operand, param));
                case ExpressionType.Not:
                    uExpr = expr as UnaryExpression;
                    return System.Linq.Expressions.Expression.Not(Convert(uExpr.Operand, param));
                case ExpressionType.NotEqual:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.NotEqual(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Or:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Or(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.OrElse:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.OrElse(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Power:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Power(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.RightShift:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.RightShift(Convert(bExpr.Left, param), Convert(bExpr.Right, param));
                case ExpressionType.Subtract:
                    bExpr = expr as BinaryExpression;
                    return System.Linq.Expressions.Expression.Subtract(Convert(bExpr.Left, param), Convert(bExpr.Right, param));

            }
            throw new NotSupportedException();
        }

        public static MethodInfo StringContainsMethod = typeof(string).GetMethod("Contains");
        public virtual Criteria<T> MakeCriteria()
        {
            var ds = this.DataSource();
            Expression expr = null;
            foreach (var prop in this.Class)
            {
                var fieldConfig = this.Configs.GetRaw(prop.Name) as JObject;
                var qt = fieldConfig["QueryType"];
                if (qt == null || qt.Type == JTokenType.Null || qt.Type == JTokenType.Undefined )
                {
                    var value = ds.Get(prop.PropertyType, prop.Name);
                    if (value == null) continue;
                    var fieldExpr = Expression.Equal(
                        Expression.Property(this.FilterParameter, prop.Name),
                        Expression.Constant(value, prop.PropertyType)
                    );
                    if (expr == null) expr = fieldExpr;
                    else expr = Expression.AndAlso(expr, fieldExpr);
                }
                else
                {
                    string qts = qt.ToString();
                    if (qts == "Range")
                    {
                        var minKey = prop.Name + "_MIN";
                        var value = ds.Get(prop.PropertyType, minKey);
                        if (value == null) continue;
                        var fieldExpr = Expression.Equal(
                            Expression.Property(this.FilterParameter, prop.Name),
                            Expression.Constant(value, prop.PropertyType)
                        );
                        if (expr == null) expr = fieldExpr;
                        else expr = Expression.AndAlso(expr, fieldExpr);

                        var maxKey = prop.Name + "_MAX";
                        value = ds.Get(prop.PropertyType, maxKey);
                        if (value == null) continue;
                        fieldExpr = Expression.Equal(
                            Expression.Property(this.FilterParameter, prop.Name),
                            Expression.Constant(value, prop.PropertyType)
                        );
                        if (expr == null) expr = fieldExpr;
                        else expr = Expression.AndAlso(expr, fieldExpr);
                    }
                    else if (qts == "Like")
                    {
                        var value = ds.Get(prop.PropertyType, prop.Name);
                        if (value == null) continue;
                        //p.FieldName.Contains('aa')
                        var fieldExpr = Expression.Call(
                            Expression.Property(this.FilterParameter, prop.Name),
                            StringContainsMethod,
                            Expression.Constant(value, prop.PropertyType)
                        );
                        if (expr == null) expr = fieldExpr;
                        else expr = Expression.AndAlso(expr, fieldExpr);
                    }


                }
            }
            this._FilterExpression = expr;
            return this;
        }


        public static Criteria<T> operator &(Criteria<T> criteria, Expression<Func<T, bool>> expr)
        {
            return  criteria.AndAlso(expr);
        }

        public static Criteria<T> operator |(Criteria<T> criteria, Expression<Func<T, bool>> expr)
        {
            return  criteria.OrElse(expr);
        }

        
    }
}