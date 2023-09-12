using Azure.AI.OpenAI;
using System.Collections.Generic;

namespace GPS_Copilot.Models
{
    public class Message
    {

        public Message() { }
        public Role Role { get; set; }
        public string Text { get; set; }
    }
}
