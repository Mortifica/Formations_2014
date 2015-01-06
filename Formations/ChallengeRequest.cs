using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Formations
{
    [Serializable]
    public class ChallengeRequest
    {
        private Person _sender;
        private Person _reciever;
        private bool _isAccepted = false;

        public Person Sender
        {
            get { return _sender; }
            private set { _sender = value; }
        }
        public Person Reciever
        {
            get { return _reciever; }
            private set { _reciever = value; }
        }
        public bool IsAccepted
        {
            get { return _isAccepted; }
            set { _isAccepted = value; }
        }
        public ChallengeRequest(Person sender, Person reciever)
        {
            this.Sender = sender;
            this.Reciever = reciever;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ChallengeRequest objAsPart = obj as ChallengeRequest;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public bool Equals(ChallengeRequest other)
        {
            bool result = true;
            if (!this.Sender.Equals(other.Sender) && !this.Reciever.Equals(other.Reciever))
            {
                result = false;
            }
            return result;
        }
        public String ToString()
        {
            return "Sender is " + Sender.Name + "\nReceiver is " + Reciever.Name + "\nIsAccepted is " + IsAccepted;
        }
    }
}
