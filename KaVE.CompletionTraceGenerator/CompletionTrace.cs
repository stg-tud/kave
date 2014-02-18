using System;
using System.Collections.Generic;
using System.Linq;
using KaVE.CompletionTraceGenerator.Model;

namespace KaVE.CompletionTraceGenerator
{
    public class CompletionTrace
    {
        public CompletionTrace()
        {
            Actions = new List<CompletionAction>();
        }

        public long DurationInMillis { get; set; }
        public IList<CompletionAction> Actions { get; private set; }

        public void AppendAction(CompletionAction action)
        {
            Actions.Add(action);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((CompletionTrace) obj);
        }

        protected bool Equals(CompletionTrace other)
        {
            return DurationInMillis == other.DurationInMillis && Actions.SequenceEqual(other.Actions);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (DurationInMillis.GetHashCode()*397) ^ (Actions != null ? Actions.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return string.Format("[DurationInMillis: {0}, Actions: [{1}]]", DurationInMillis, string.Join(",", Actions));
        }
    }
}