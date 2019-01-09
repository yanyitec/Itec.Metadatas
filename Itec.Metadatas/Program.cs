using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;
using Itec.Metas;

namespace Itec.Models
{
    public class User
    {
        public enum Genders {
            Undefined,
            Male,
            Female
        }
        public int Id;
        public string Name { get; set; }

        public int? Age { get; set; }

        public Genders Gender { get; set; }

        public string Description;

        public Article Article;
    }

    public class Article
    {
        public int Id { get; set; }

        public string Description;

        public string Name { get; set; }

        public int? Age { get; set; }

        public User User { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Copy Test");
            Copy();
            Console.WriteLine("Setter Test");
            SetterTest();
            Console.WriteLine("Test Complete");
            Console.ReadKey();
        }

        static void GetterTest()
        {
            var user = new User()
            {
                Id = 2,
                Name = "yy"
            };
            var fields = typeof(User).GetFields();
            var propId = new Property(fields.First(p => p.Name == "Id"));
            var id = propId.GetValue(user);
            Console.WriteLine("Get Id Value:" + id);

            var propName = new Property(typeof(User).GetProperties().First(p => p.Name == "Name"));
            var name = propName.GetValue(user);
            Console.WriteLine("Get Name Value:" + name);
        }

        static void SetterTest()
        {
            var user = new User()
            {
                Id = 2,
                Name = "yy"
            };
            var fields = typeof(User).GetFields();
            var props = typeof(User).GetProperties();
            var propId = new Property(fields.First(p => p.Name == "Id"));
            propId.SetValue(user, 3);
            Console.WriteLine("Set Id Value(3):" + user.Id.ToString());

            propId.SetValue(user, "23");
            Console.WriteLine("Set Id Value('23'):" + user.Id.ToString());

            propId.SetValue(user, "abc");
            Console.WriteLine("Set Id Value('abc'):" + user.Id.ToString());

            propId.SetValue(user, new object());
            Console.WriteLine("Set Id Value(new object()):" + user.Id.ToString());

            propId.SetValue(user, JToken.FromObject(33));
            Console.WriteLine("Set Id to JInt(33):" + user.Id.ToString());

            propId.SetValue(user, null);
            Console.WriteLine("Set Id to NULL:" + user.Id.ToString());

            //Nullable
            var propAge = new Property(props.First(p => p.Name == "Age"));

            propAge.SetValue(user, 3);
            Console.WriteLine("Set Age Value(3):" + user.Age.ToString());

            propAge.SetValue(user, "23");
            Console.WriteLine("Set Age Value('23'):" + user.Age.ToString());

            propAge.SetValue(user, "abc");
            Console.WriteLine("Set Age Value('abc'):" + user.Age.ToString());

            propAge.SetValue(user, new object());
            Console.WriteLine("Set Age Value(new object()):" + user.Age.ToString());

            propAge.SetValue(user, JToken.FromObject(33));
            Console.WriteLine("Set Age to JInt(33):" + user.Age.ToString());

            propAge.SetValue(user, null);
            Console.WriteLine("Set Age to NULL:" + user.Age.ToString());

            //gender
            var propGender = new Property(props.First(p => p.Name == "Gender"));

            propGender.SetValue(user, 3);
            Console.WriteLine("Set Gender Value(3):" + user.Gender.ToString());

            propGender.SetValue(user, "23");
            Console.WriteLine("Set Gender Value('23'):" + user.Gender.ToString());

            propGender.SetValue(user, "abc");
            Console.WriteLine("Set Gender Value('abc'):" + user.Gender.ToString());

            propGender.SetValue(user, "Female");
            Console.WriteLine("Set Gender Value('Female'):" + user.Gender.ToString());

            propGender.SetValue(user, new object());
            Console.WriteLine("Set Gender Value(new object()):" + user.Gender.ToString());

            propGender.SetValue(user, JToken.FromObject(33));
            Console.WriteLine("Set Gender to JInt(33):" + user.Gender.ToString());

            propGender.SetValue(user, null);
            Console.WriteLine("Set Gender to NULL:" + user.Gender.ToString());
            
            
            //name


            var propName = new Property(props.First(p => p.Name == "Name"));
            propName.SetValue(user, "yanyi");
            Console.WriteLine("Set Name Value(yanyi):" + user.Name.ToString());

            propName.SetValue(user, new object());
            Console.WriteLine("Set Name Value(new object())[Null Check]:" + user.Name.ToString());


            propName.SetValue(user, 34);
            Console.WriteLine("Set Name Value(34):" + user.Name.ToString());


            propName.SetValue(user, null);
            Console.WriteLine("Set Name Value to NULL:" + (user.Name == null).ToString());

            var arti = new Article
            {
                Name = "My Article"
            };
            var jobj = JObject.FromObject(arti);
            propName.SetValue(user, jobj);
            Console.WriteLine("Set Name Value(JObject({Name:'MyArticle'})):" + user.Name);


            var propArticle = new Property(fields.First(p => p.Name == "Article"));
            propArticle.SetValue(user, "yanyi");
            Console.WriteLine("Set Article Value(yanyi)[Null check]:" + (user.Article == null).ToString());
            propArticle.SetValue(user, new Article() { Id=56});
            Console.WriteLine("Set Article Value(new Article() { Id=56}):" + user.Article.Id);

            propArticle.SetValue(user, null);
            Console.WriteLine("Set Article Value to NULL:" + (user.Article == null).ToString());


            propArticle.SetValue(user, jobj);
            Console.WriteLine("Set Article Value(JObject({Name:'MyArticle'})):" + user.Article.Name);


        }
        static void Generic()
        {
            var user = new User()
            {
                Id = 2,
                Name = "yy"
            };
            var fields = typeof(User).GetFields();
            var propId = new Property<User>(fields.First(p => p.Name == "Id"));
            var id = propId.GetValue(user);
            Console.WriteLine("Get Id Value:" + id);

            propId.SetValue(user, 4);
            Console.WriteLine("Get Name Value:" + user.Id);
        }

        static void Copy()
        {
            var user = new User()
            {
                Id = 2,
                Name = "y1",
                Description = "aa"
            };
            var title = new Article()
            {
                Id = 3,
                Name = "yy",
                Description = "bb"
            };

            var cls = new Class(typeof(User));
            var title1 = cls.CopyTo(user, title, "Id,Description");
            Console.WriteLine("Articile.Id(2)=" + title.Id.ToString());
            Console.WriteLine("Articile.Name(yy)=" + title.Name.ToString());
            Console.WriteLine("Articile.Description(aa)=" + title.Description.ToString());
        }
    }
}
