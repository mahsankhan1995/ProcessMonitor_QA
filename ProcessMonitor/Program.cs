using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;

namespace ProcessMonitor
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Thread mainThread = Thread.CurrentThread;
            bool inputValidator;

                /*
                String processName = "Notepad";
                double processLifeTime = 5;
                double freq = 1;
                */

                String processName;
                double processLifeTime, freq;

                Console.WriteLine("Please enter process name ....");
                processName = Console.ReadLine();

                do // ---------------input Validation for process max lifetime
                {
                    Console.WriteLine("Please enter Max lifetime(minutes) of this process...");
                    inputValidator = double.TryParse(Console.ReadLine(), out processLifeTime);
                }
                while (!inputValidator);

                do // ---------------input Validation for process max lifetime
                {
                    Console.WriteLine("Please enter Frequncy(minutes) to monitor the process...");
                    inputValidator = double.TryParse(Console.ReadLine(), out freq);
                }
                while (!inputValidator);

                Console.WriteLine("Searching for process \"" + processName + "\" if found will be Killed after " + processLifeTime + " Minutes for activity\n\nor press 'Q' to quit the applcation.");

            Thread thread1 = new Thread(ExitApp);
            thread1.Start();
            
            ProcessMonitorClass(processName, processLifeTime, freq);
           




        }


        private static void ProcessMonitorClass(String processName, double processLifeTime, double freq)
        {

            TimeSpan processRunTime;
            
            Process[] process = Process.GetProcessesByName(processName);

            if (process.Length == 0)
            {
                Thread.Sleep(Convert.ToInt32(freq) * 60000);

                ProcessMonitorClass(processName, processLifeTime, freq);
            }

            else
            {              
                // If process is running......
                foreach (Process myprocess in process)
                {
                    processRunTime = DateTime.Now - myprocess.StartTime;
                
                    if (processRunTime.TotalMinutes >= processLifeTime)
                    {
                        Console.WriteLine("Process \"" + processName + "\"found \nKilled process \""
                            + processName + "\" after \"" + processRunTime.ToString("mm':'ss") 
                            + "\" Minutes of activity");
                        myprocess.Kill();
                        LogWriter("Process \"" + processName + "\" was Killed\n"); //after \"" + processRunTime.ToString("mm':'ss")   + "\" Minutes of activity");
                        Console.ReadLine();
                        Environment.Exit(0);
                        
                    }

                    else{

                        Thread.Sleep((Convert.ToInt32(processLifeTime) - Convert.ToInt32(processRunTime.TotalMinutes)) * 990);
                        ProcessMonitorClass(processName, processLifeTime, freq);
                        break;
                        
                    }
                    
                }

            }
        }

        //------------------------ For exiting console app-----------------------------
        private static void ExitApp()
        {
            if(Console.ReadLine()=='Q'.ToString())
            {
                Environment.Exit(0);
            }
            else
            {
                ExitApp();
            }
        }


        //------------------------ For logging-----------------------------
        public static void LogWriter(String message)
        {
            string logPath = ConfigurationManager.AppSettings["logPath"];

            using(StreamWriter writer = new StreamWriter(logPath, true))
            {
                writer.WriteLine($"{DateTime.Now} : {message}");
            }
        }
    }
}


