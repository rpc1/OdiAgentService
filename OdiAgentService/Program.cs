using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Net;


namespace OdiAgentService  
{
    class OdiAgent : System.ServiceProcess.ServiceBase
    {
        private System.Diagnostics.EventLog eventLog1;
        private int eventId;
        private String logFileName;
        private String odiAgentUrl;


        /// <summary>s
        /// Главная точка входа для приложения.
        /// </summary>
        static void Main()
        {

            ServiceBase[] ServicesToRun;
            ServicesToRun =
              new ServiceBase[] { new OdiAgent() };
            ServiceBase.Run(ServicesToRun);
        }



        public void Logger(String lines)
        {

            System.IO.StreamWriter file = new System.IO.StreamWriter(logFileName, true);
            file.WriteLine(lines);
            file.Close();
        }

        public OdiAgent()
        {
            InitializeComponent();

            eventLog1 = new System.Diagnostics.EventLog("Application");
            eventLog1.Source = "OdiAgentService";
               
         
        }
        public void OnTimer(object sender, System.Timers.ElapsedEventArgs args)
        {
            eventLog1.WriteEntry("Monitoring the System", EventLogEntryType.Information, eventId++);
        }

        private void InitializeComponent()
        {
            this.ServiceName = "OdiAgentService";

        }
        /// <summary>
        /// Set things in motion so your service can do its work.
        /// </summary>
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("Starts service", EventLogEntryType.Information);
            // Set up a timer that triggers every minute.
            System.Timers.Timer timer = new System.Timers.Timer();
            timer.Interval = 60000; // 60 seconds
            timer.Elapsed += new System.Timers.ElapsedEventHandler(this.OnTimer);
            timer.Start();
        }
        /// <summary>
        /// Stop this service.
        /// </summary>
        protected override void OnStop()
        {
            eventLog1.WriteEntry("Stop service", EventLogEntryType.Information);
        }
        private static void runService(String serviceName)
        {
            ServiceController controller = new ServiceController(serviceName);
            if (controller.Status != ServiceControllerStatus.Running)
                controller.Start();
        }

        private static void pingODIAgent()
        {

            HttpWebRequest http = (HttpWebRequest)HttpWebRequest.Create("https://www.mail.ru:990");
            //http.Referer = "referrer";
            http.Timeout = 2000;

            HttpWebResponse response;
            try
            {
                response = (HttpWebResponse)http.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                    {

                        string responseJson = sr.ReadToEnd();
                        Console.WriteLine(responseJson);
                    }
                    response.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Connection error");
            }
            Console.ReadKey();
        }
    }


}
