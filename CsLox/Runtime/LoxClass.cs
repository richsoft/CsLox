using CsLox.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsLox.Runtime
{
    class LoxClass : ILoxCallable
    {
        private readonly HashMap<string, LoxFunction> _methods;

        public string Name { get; }
        public LoxClass Superclass { get; }

        public int Arity { get
            {
                // Check if we have a constructor
                LoxFunction initializer = _methods.Get("init");
                if (initializer == null) return 0;
                return initializer.Arity;
            } }
        
        public LoxClass(string name, LoxClass superclass, HashMap<string, LoxFunction> methods)
        {
            this.Name = name;
            this.Superclass = superclass;
            this._methods = methods;
        }

        public object Call(Interpreter interpreter, IList<object> arguments)
        {
            LoxInstance instance = new LoxInstance(this);

            // Constructor
            LoxFunction initializer = _methods.Get("init");
            if (initializer != null)
            {
                initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        public LoxFunction FindMethod(LoxInstance instance, string name)
        {
            if (_methods.ContainsKey(name))
            {
                return _methods.Get(name).Bind(instance);
            }

            // Check the superclass if we are inherited
            if (this.Superclass != null )
            {
                return this.Superclass.FindMethod(instance, name);
            }

            return null;
        }


        public override string ToString()
        {
            return this.Name;
        }
    }
}
