using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Itec
{
    public static class ValConvert
    {
        
        public static Noneable<T> ConvertTo<T>(this string input) {
            var parser = GetParser<T>();
            return parser(input);
        }
        
        public static Func<string,Noneable<T>> GetParser<T>()  {
            object valTypeInfo = null;
            Type t = typeof(T);
            if (!Parsers.TryGetValue(t.GUID, out valTypeInfo)) {
                if (!t.IsEnum) return null;
                valTypeInfo = DynamicParsers.GetOrAdd(t.GUID,(id)=> {
                     return MakeEnumParser<T>();
                });
            }
            return valTypeInfo as Func<string, Noneable<T>>;
        } 

        static System.Text.RegularExpressions.Regex NumberRegex = new System.Text.RegularExpressions.Regex("\\s*\\d+\\s*");
        static MethodInfo IsNullOrWhiteSpaceMethod = typeof(string).GetMethod("IsNullOrWhiteSpace");
        static MethodInfo IsMatchMethod = typeof(Regex).GetMethod("IsMatch", new Type[] { typeof(string) });
        static MethodInfo IntParseMethod = typeof(int).GetMethod("Parse",new Type[] { typeof(string)});
        //static Noneable<T> ParseEnum
        static int GetEnumValue(Dictionary<string ,int> dic, string key) {
            int ret = 0;
            return dic.TryGetValue(key,out ret)?ret:int.MaxValue;
            
        }
        static Func<string, Noneable<T>> MakeEnumParser<T>() {
            var txtExpr = Expression.Parameter(typeof(string),"txt");
            //Expression.Invoke()
            var noneExpr = Expression.New(typeof(Noneable<T>));
            var codes =new List<Expression>();
            var locals = new List<ParameterExpression>();
            var retTarget = Expression.Label(typeof(Noneable<T>));
            //if (string.IsNullOrWhiteSpace(t)) return null;
            codes.Add(Expression.IfThen(
                //IF (string.IsNullOrWhiteSpace(t))
                Expression.Call(IsNullOrWhiteSpaceMethod, txtExpr)
                //THEN return null
                , Expression.Return(retTarget, noneExpr)
            ));
            //if (NumberRegex.IsMatch(t)) {return new Nullable<T>(int.Parse(txt)) }
            codes.Add(Expression.IfThen(
                //IF (NumberRegex.IsMatch(t))
                Expression.Call(Expression.Constant(NumberRegex), IsMatchMethod, txtExpr)
                //THEN return
                , Expression.Return(retTarget
                    //NEW
                    , Expression.New(
                        //Nullable<T>.ctor(T v)
                        typeof(Noneable<T>).GetConstructor(new Type[] { typeof(T) })
                        //(T) int.Parse(t)
                        , Expression.Convert(Expression.Call(IntParseMethod, txtExpr), typeof(T))
                    )
                )
            ));

            var names = Enum.GetNames(typeof(T));
            var values = Enum.GetValues(typeof(T));
            var dict = new Dictionary<string, int>();
            for (var i = 0; i < names.Length; i++) {
                dict.Add(names[i],(int)values.GetValue(i));
            }
            var mbs = new Func<Dictionary<string,int>,string,int>(GetEnumValue);
            var getEnumValueExpr = Expression.Invoke(Expression.Constant(mbs),Expression.Constant(dict), txtExpr);
            var nValueExpr = Expression.Parameter(typeof(int),"nval");
            locals.Add(nValueExpr);
            codes.Add(Expression.Assign(nValueExpr,getEnumValueExpr));
            var retValueExpr = Expression.Condition(
                Expression.Equal(nValueExpr, Expression.Constant(int.MaxValue))
                , Expression.New(typeof(Noneable<T>))
                , Expression.New(typeof(Noneable<T>).GetConstructor(new Type[] { typeof(T)}),Expression.Convert(nValueExpr,typeof(T)))
            );
            codes.Add(Expression.Return(retTarget,retValueExpr));
            codes.Add(Expression.Label(retTarget,Expression.New(typeof(Noneable<T>))));
            var block = Expression.Block(locals,codes);//typeof(Nullable<T>),
            var lamda = Expression.Lambda<Func<string, Noneable<T>>>(block,txtExpr);
            return lamda.Compile();

        }

        

        static bool IsNumber(string txt)
        {
            return NumberRegex.IsMatch(txt);
        }

        static ConcurrentDictionary<Guid, object> DynamicParsers = new ConcurrentDictionary<Guid, object>();

        static Dictionary<Guid, object> Parsers = new Dictionary<Guid, object>() {
            { typeof(byte).GUID,new Func<string, Noneable<byte>>((t)=>{
                byte rs = 0;return byte.TryParse(t,out rs)?new Noneable<byte>(rs):new Noneable<byte>();
            })}
            ,{ typeof(short).GUID,new Func<string, Noneable<short>>((t)=>{
                short rs = 0;return short.TryParse(t,out rs)?new Noneable<short>(rs):new Noneable<short>();
            })}
            ,{ typeof(ushort).GUID,new Func<string, Noneable<ushort>>((t)=>{
                ushort rs = 0;return ushort.TryParse(t,out rs)?new Noneable<ushort>(rs):new Noneable<ushort>();
            })}
            ,{ typeof(bool).GUID,new Func<string, Noneable<bool>>((t)=>{
                if(string.IsNullOrWhiteSpace(t)) return new Noneable<bool>(false);
                if(IsNumber(t)) {
                    var n = int.Parse(t);
                    if(n!=0) return new Noneable<bool>(true);
                    else return new Noneable<bool>(false);
                }
                if(t=="false"||t=="off") return new Noneable<bool>(false);
                return new Noneable<bool>(true);
            })}
            ,{ typeof(int).GUID,new Func<string, Noneable<int>>((t)=>{
                int v;
                return int.TryParse(t,out v)?new Noneable<int>(v):new Noneable<int>();
            })}
            ,{ typeof(uint).GUID,new Func<string, Noneable<uint>>((t)=>{
                uint v;
                return uint.TryParse(t,out v)?new Noneable<uint>(v):new Noneable<uint>();
            })}
            ,{ typeof(long).GUID,new Func<string, Noneable<long>>((t)=>{
                long v;
                return long.TryParse(t,out v)?new Noneable<long>(v):new Noneable<long>();
            })}
            ,{ typeof(ulong).GUID,new Func<string, Noneable<ulong>>((t)=>{
                ulong v;
                return ulong.TryParse(t,out v)?new Noneable<ulong>(v):new Noneable<ulong>();
            })}
            ,{ typeof(float).GUID,new Func<string, Noneable<float>>((t)=>{
                float v;
                return float.TryParse(t,out v)?new Noneable<float>(v):new Noneable<float>();
            })}
            ,{ typeof(double).GUID,new Func<string, Noneable<double>>((t)=>{
                double v;
                return double.TryParse(t,out v)?new Noneable<double>(v):new Noneable<double>();
            })}
            ,{ typeof(decimal).GUID,new Func<string, Noneable<decimal>>((t)=>{
                decimal v;
                return decimal.TryParse(t,out v)?new Noneable<decimal>(v):new Noneable<decimal>();
            })}
            ,{ typeof(DateTime).GUID,new Func<string, Noneable<DateTime>>((t)=>{
                if(string.IsNullOrWhiteSpace(t)) return new Noneable<DateTime>();
                t = t.Replace("T"," ");
                DateTime v;
                return DateTime.TryParse(t,out v)?new Noneable<DateTime>(v):new Noneable<DateTime>();
            })}
            ,{ typeof(Guid).GUID,new Func<string, Noneable<Guid>>((t)=>{
                if(string.IsNullOrWhiteSpace(t)) return new Noneable<Guid>();
                Guid v;
                return Guid.TryParse(t,out v)?new Noneable<Guid>(v):new Noneable<Guid>();
            })}
        };
    }
}
