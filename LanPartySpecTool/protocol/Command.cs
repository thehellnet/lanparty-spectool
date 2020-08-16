namespace LanPartySpecTool.protocol
{
    public class Command
    {
        private readonly CommandAction _action;

        public Command(CommandAction action)
        {
            _action = action;
        }

        public CommandAction Action => _action;

        protected bool Equals(Command other)
        {
            return _action == other._action;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Command) obj);
        }

        public override int GetHashCode()
        {
            return (int) _action;
        }

        public override string ToString()
        {
            return _action.ToString();
        }
    }
}