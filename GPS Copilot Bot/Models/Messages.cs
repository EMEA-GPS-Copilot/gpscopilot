using GPS_Copilot.Models;
using System.Collections.Generic;

namespace GPS_Copilot.Models
{
    public class Messages
    {
        public Messages()
        {
            MessageList = new List<Message>();
        }
        public List<Message> MessageList { get; set; }
    }

}
