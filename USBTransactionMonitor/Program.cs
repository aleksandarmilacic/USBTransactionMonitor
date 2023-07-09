using System;
using System.Management;

namespace USBTransactionMonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            // Subscribe to USB device insertion and removal events
            var watcher = new ManagementEventWatcher();
            var query = new WqlEventQuery("SELECT * FROM __InstanceOperationEvent WITHIN 1 WHERE TargetInstance ISA 'Win32_USBControllerDevice'");
            watcher.EventArrived += USBEventHandler;
            watcher.Query = query;
            watcher.Start();

            Console.WriteLine("USB Transaction Monitor running. Press any key to exit...");
            Console.ReadKey();

            // Stop monitoring
            watcher.Stop();
        }

        static void USBEventHandler(object sender, EventArrivedEventArgs e)
        {
            var instance = (ManagementBaseObject)e.NewEvent["TargetInstance"];
            var eventType = e.NewEvent.ClassPath.ClassName;

            if (eventType.Equals("__InstanceCreationEvent"))
            {
                // USB device inserted
                var deviceId = (string)instance["Dependent"];
                Console.WriteLine("USB Device Inserted:");
                Console.WriteLine($"Device ID: {deviceId}");
                Console.WriteLine();
            }
            else if (eventType.Equals("__InstanceDeletionEvent"))
            {
                // USB device removed
                var deviceId = (string)instance["Dependent"];
                Console.WriteLine("USB Device Removed:");
                Console.WriteLine($"Device ID: {deviceId}");
                Console.WriteLine();
            }
        }
    }
}
