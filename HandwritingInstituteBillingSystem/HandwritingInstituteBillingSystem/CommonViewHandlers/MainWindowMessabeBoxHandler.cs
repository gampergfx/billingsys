using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandwritingInstituteBillingSystem.CommonViewHandlers
{
    static class MainWindowMessabeBoxHandler
    {
        public static event EventHandler<MessageData> ShowErrorMessageEvent;

        public static void OnShowMessageEvent(MessageData e)
        {
            ShowErrorMessageEvent?.Invoke(null, e);
        }
    }

    static class NewFormMessabeBoxHandler
    {
        public static event EventHandler<MessageData> ShowErrorMessageEvent;

        public static void OnShowMessageEvent(MessageData e)
        {
            ShowErrorMessageEvent?.Invoke(null, e);
        }
    }

    public class MessageData
    {
        public string   Message { get; set; }
        public string   Title { get; set; }
    }
}
