namespace AutoReminder.Utils.MessageModifier
{
    public interface IMessageModifier
    {
        string Modify(string templateString, MessageContext currentContext);
    }
}
