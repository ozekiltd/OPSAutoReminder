using System.Collections.Generic;

namespace AutoReminder.Utils.MessageModifier
{
    class TemplateModifier : ITemplateModifier
    {
        private List<IMessageModifier> _messageModifiers;

        public TemplateModifier()
        {
            _messageModifiers = new List<IMessageModifier>();
        }

        public void Register(IMessageModifier messageModifier)
        {
            _messageModifiers.Add(messageModifier);
        }

        public string Modify(string templateMessage, MessageContext messageContext)
        {
            foreach (var modifier in _messageModifiers)
                templateMessage = modifier.Modify(templateMessage, messageContext);

            return templateMessage;
        }
    }
}
