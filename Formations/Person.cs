using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Formations
{
    [Serializable]
    public class Person : IEquatable<Person>
    {
        private String _name;
        private String _password;
        public String ipAddress { get; set; }

        public String Name 
        { 
            get 
            { 
                return _name; 
            } 
            private set 
            { 
                _name = value; 
            } 
        }
        public String Password 
        { 
            get 
            { 
                return _password; 
            } 
            private set 
            { 
                _password = value; 
            } 
        }
        public Person(String name, String password)
        {
            this.Name = name;
            this.Password = password;
        }
        public override String ToString()
        {
            return Name + " - " + ipAddress;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Person objAsPart = obj as Person;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public bool Equals(Person other)
        {
            bool result = true;
            if (!this.Name.Equals(other.Name))
            {
                result = false;
            }
            if (!this.Password.Equals(other.Password))
            {
                result = false;
            }
            if (!this.ipAddress.Equals(other.ipAddress))
            {
                result = false;
            }
            return result;
        }
    }
}
