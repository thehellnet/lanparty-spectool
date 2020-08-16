using NUnit.Framework;

namespace LanPartySpecTool.protocol
{
    public class CommandParserTest
    {
        [Test]
        public void TestParseNull()
        {
            const string input = null;
            var actual = CommandParser.Parse(input);
            Assert.IsNull(actual);
        }

        [Test]
        public void TestParseEmpty()
        {
            const string input = "";
            var actual = CommandParser.Parse(input);
            Assert.IsNull(actual);
        }

        [Test]
        public void TestParseNotValid()
        {
            const string input = "test";
            var actual = CommandParser.Parse(input);
            Assert.IsNull(actual);
        }

        [Test]
        public void TestParseEmptyJson()
        {
            const string input = "{}";
            var actual = CommandParser.Parse(input);
            Assert.IsNull(actual);
        }

        [Test]
        public void TestParseValidJsonInvalidAction()
        {
            const string input = "{\"action\": \"test\"}";
            var actual = CommandParser.Parse(input);
            Assert.IsNull(actual);
        }

        [Test]
        public void TestParseValid()
        {
            const string input = "{\"action\": \"JOIN_SPECTATE\"}";
            var expected = new Command(CommandAction.JoinSpectate);
            var actual = CommandParser.Parse(input);
            Assert.AreEqual(expected, actual);
        }
    }
}