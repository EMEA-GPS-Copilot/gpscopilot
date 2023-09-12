namespace GPS_Copilot.Models
{
    public class ContextHelper
    {
        public ContextHelper()
        {
            IsSystemMessageLoaded = false;
        }

        public bool IsSystemMessageLoaded { get; set; }

        public string Username { get; set; }
    }
}
