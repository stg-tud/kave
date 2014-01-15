using KaVE.Model.Utils;

namespace KaVE.Model.Names.CSharp
{
    public class EventName : MemberName, IEventName
    {
        private static readonly WeakNameCache<EventName> Registry = WeakNameCache<EventName>.Get(
            id => new EventName(id));

        /// <summary>
        /// Event names follow the scheme <code>'modifiers' ['event-handler-type name'] ['declaring-type name'].'name'</code>.
        /// Examples of type names are:
        /// <list type="bullet">
        ///     <item><description><code>[ChangeEventHandler, IO, Version=1.2.3.4] [TextBox, GUI, Version=5.6.7.8].Changed</code></description></item>
        /// </list>
        /// </summary>
        public new static EventName Get(string identifier)
        {
            return Registry.GetOrCreate(identifier);
        }

        private EventName(string identifier) : base(identifier) {}

        public override string Name
        {
            get { return Identifier.Substring(Identifier.LastIndexOf('.') + 1); }
        }

        public ITypeName HandlerType
        {
            get { return ValueType; }
        }
    }
}