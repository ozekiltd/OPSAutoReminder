using System.Collections.Specialized;

namespace AutoReminder.Utils.MessageModifier
{
    public class MessageContext
    {
        private NameValueCollection _contextParameters;

        public MessageContext()
        {
            _contextParameters = new NameValueCollection();
        }

        public string GetValue(string key)
        {
            return _contextParameters.Get(key);
        }

        public void SetValue(string key, string value)
        {
            _contextParameters.Add(key, value);
        }
    }
}
