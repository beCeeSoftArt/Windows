namespace EventLog
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create the source, if it does not already exist.
            if (!System.Diagnostics.EventLog.SourceExists("beCeeSoftArt"))
            {
                System.Diagnostics.EventLog.CreateEventSource("beCeeSoftArt", "beCeeSoftArt Events");
            }

            // Create an EventLog instance and assign its source.
            System.Diagnostics.EventLog myLog = new System.Diagnostics.EventLog { Source = "beCeeSoftArt" };

            // Write an informational entry to the event log.    
            myLog.WriteEntry("beCeeSoftArt Test Entry.");
        }
    }
}
