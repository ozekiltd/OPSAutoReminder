namespace AutoReminder.Utils.MessageModifier
{
    interface ITemplateModifier
    {
        void Register(IMessageModifier messageModifier);
        string Modify(string templateMessage, MessageContext messageContext);
    }
}
