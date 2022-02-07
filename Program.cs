using System;
using System.Diagnostics;
using System.Threading;
using System.IO;

namespace SimpleProcLogger
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Boolean isErrors = false;

             void ShowHelp()
            {
                Console.WriteLine("Использование: SimpleProcLogger <путь_к_исполняемому_файлу> <интервал_времени_в_секундах>\n");
                Console.WriteLine("Пример: SimpleProcLogger.exe C:\\Windows\\System32\\mytestprorgam.exe 2");
                Console.WriteLine("Пример: SimpleProcLogger.exe \"D:\\Folder with spaces\\setup.exe\" 4");
                Console.ReadLine();
            }

            if (args.Length != 2)
            {
                ShowHelp();
                isErrors = true;
            } else{

                if (!File.Exists(args[0]))
                {
                    Console.WriteLine("Файл: " + args[0]+ " не существует");
                    ShowHelp();
                    isErrors = true;
                }

                if (!int.TryParse(args[1], out int number))
                {

                    Console.WriteLine(args[1] + " неправильный формат времени задержки");
                    ShowHelp();
                    isErrors = true;

                }
                else {
                    if (number % 1 != 0) 
                    {
                        Console.WriteLine(args[1] + " неправильный формат времени задержки (не целое)");
                        isErrors = true;
                        ShowHelp();
                    }

                    if (number < 0)
                    {
                        Console.WriteLine(args[1] + " неправильный формат времени задержки (отрицательное число)");
                        isErrors = true;
                        ShowHelp();
                    }


                }


        }

            if (!isErrors)
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = args[0];
                    process.Start();

                    PerformanceCounter cpuCounter = new PerformanceCounter("Process", "% Processor Time", process.ProcessName);

                    StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "ProcLog_" + process.ProcessName + ".csv");
                    sw.WriteLine("DateTime" + ";" + "Working Set (MB)" + ";" + "Private Bytes (MB)" + ";" + "Handle Count" + ";" + "CPU %");
                    sw.Flush();

                    while (!process.HasExited)
                    {
                        process.Refresh();
                        sw.WriteLine(DateTime.Now.ToString("yyyy.MM.dd_HH-mm-ss") + ";" + process.WorkingSet64 / (1024f * 1024f) + ";" + process.PrivateMemorySize64 / (1024f * 1024f) + ";" + process.HandleCount + ";" + cpuCounter.NextValue());
                        sw.Flush();
                        Thread.Sleep(int.Parse(args[1])*1000);
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);

                }
            }
        }
    }
}
