namespace AutoReminder.Utils.MessageModifier
{
    class MessageReplacer : IMessageModifier
    {
        private string _parameterValue;

        public MessageReplacer(string parameterValue)
        {
            _parameterValue = parameterValue;
        }

        public string Modify(string templateString, MessageContext currentContext)
        {
            var contextValue = currentContext.GetValue(_parameterValue);
            if (contextValue == null)
                return templateString;

            return templateString.Replace(_parameterValue, contextValue);
        }
    }
}
