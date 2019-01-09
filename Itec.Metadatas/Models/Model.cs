using Itec.Datas;
using Itec.Metas;
using System;
using System.Collections.Generic;
using System.Text;

namespace Itec.Models
{
    public class Model
    {
        public Model(Class cls
            , Validations.ValidatorFactoryFactory validatorFactoryFactory
            ,IConfigFactory configFactory
            ) {
            this.ValidatorFactoryFactory = validatorFactoryFactory;
            this.Class = cls;
            this.ConfigFactory = configFactory;
        }
        object _Value;
        public object Value { get { return _Value; } }

        string _Fieldnames;
        public string Fieldnames() {
            return _Fieldnames;
        }

        public Model Fieldnames(string fieldnames) {
            this._Fieldnames = fieldnames;
            return this;
        }

        string _ConfigName;
        public string ConfigName()
        {
            return _ConfigName;
        }

        public Model ConfigName(string configName)
        {
            this._ConfigName = configName;
            return this;
        }

        IDataSource _DataSource;
        public IDataSource DataSource()
        {
            return _DataSource;
        }

        public Model DataSource(IDataSource dataSource)
        {
            this._DataSource = dataSource;
            return this;
        }

        public Class Class { get; private set; }

        public Validations.ValidatorFactoryFactory ValidatorFactoryFactory { get; private set; }

        
        Validations.Validator _Validator;
        public Validations.Validator Validator {
            get {
                if (_Validator == null) {
                    var factory = this.ValidatorFactoryFactory.GetFactory(this.Class, this._ConfigName);
                    _Validator = factory.GetValidator(this._Fieldnames);
                }
                return _Validator;
            }
        }


        IDataSource _Config;
        public IDataSource Configs
        {
            get
            {
                if (_Config == null)
                {
                    _Config = this.ConfigFactory.GetClassConfig(this.Class,this._ConfigName);
                }
                return _Config;
            }
        }


        public IConfigFactory ConfigFactory {
            get;private set;
        }


        public Validations.IValidation Validate() {
            
            return this.Validator.Validate(this._DataSource);
        }


        public virtual Model Fill() {
            //this._Value = this.Class.CreateInstance() as T;
            this._Value = this._DataSource.Get(this.Class.Type);
            return this;
        }
    }
}
