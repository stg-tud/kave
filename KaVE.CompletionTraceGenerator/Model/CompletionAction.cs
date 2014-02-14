namespace KaVE.CompletionTraceGenerator.Model
{
    public class CompletionAction
    {
        public ActionType Type { get; private set; }
        public Direction? Direction { get; private set; }
        public int? Index { get; private set; }
        public string Token { get; private set; }

        private CompletionAction() {}

        protected bool Equals(CompletionAction other)
        {
            return Type == other.Type && Direction == other.Direction && Index == other.Index && string.Equals(Token, other.Token);
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
            if (obj.GetType() != this.GetType())
            {
                return false;
            }
            return Equals((CompletionAction) obj);
        }

        public override string ToString()
        {
            return string.Format("[Type: {0}, Direction: {1}, Index: {2}, Token: {3}]", Type, Direction, Index, Token);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) Type;
                hashCode = (hashCode*397) ^ Direction.GetHashCode();
                hashCode = (hashCode*397) ^ Index.GetHashCode();
                hashCode = (hashCode*397) ^ (Token != null ? Token.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static CompletionAction NewCancel()
        {
            return new CompletionAction {Type = ActionType.Cancel};
        }

        public static CompletionAction NewApply()
        {
            return new CompletionAction {Type = ActionType.Apply};
        }

        public static CompletionAction NewFilter(string token)
        {
            return new CompletionAction {Type = ActionType.Filter, Token = token};
        }

        public static CompletionAction NewMouseGoto(int currentIndex)
        {
            return new CompletionAction {Type = ActionType.MouseGoto, Index = currentIndex};
        }

        public static CompletionAction NewScroll(int startIndex)
        {
            return new CompletionAction {Type = ActionType.Scroll, Index = startIndex};
        }

        public static CompletionAction NewPageStep(Direction direction)
        {
            return new CompletionAction {Type = ActionType.PageStep, Direction = direction};
        }

        public static CompletionAction NewStep(Direction direction)
        {
            return new CompletionAction {Type = ActionType.Step, Direction = direction};
        }
    }
}