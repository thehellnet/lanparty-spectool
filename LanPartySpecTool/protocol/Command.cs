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
    }
}