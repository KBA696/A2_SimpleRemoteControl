using System.Diagnostics;

namespace SRC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Process process = Process.GetCurrentProcess();
            FileInfo info = new FileInfo(process.MainModule.FileName);
            programLocation = info.DirectoryName;

            var builder = WebApplication.CreateBuilder(args);

            var app = builder.Build();

            app.MapGet("/", (HttpContext context) => { return Perform(Request.Status); });
            app.MapGet("/start", (HttpContext context) => { return Perform(Request.Start); });
            app.MapGet("/stop", (HttpContext context) => { return Perform(Request.Stop); });
            app.MapGet("/reboot", Reboot);

            app.Run();
        }

        static string Reboot(HttpContext context)
        {
            Perform(Request.Stop);

            Thread.Sleep(2000);

            return Perform(Request.Start);
        }

        /// <summary>
        /// Расположение программы
        /// </summary>
        static string programLocation;

        /// <summary>
        /// Выполнить
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        static string Perform(Request request)
        {
            Process[] process = Process.GetProcessesByName("КМЗсервер");

            switch (request)
            {
                case Request.Start:
                    if (process.Length == 0)
                    {
                        try
                        {
                            Process.Start(programLocation + "\\КМЗсервер.exe");
                            return "Сервер запущен";
                        }
                        catch
                        {
                            return "Не найден фаил сервера";
                        }
                    }
                    return "Сервер уже запущен";
                case Request.Stop:
                    if (process.Length > 0)
                    {
                        foreach (var proces in process)
                        {
                            proces.Kill();
                        }
                        return "Сервер выключен";
                    }
                    return "Сервер не запущен";
                case Request.Status:
                    if (process.Length > 0)
                    {
                        string result = "Сервер включен\n";
                        try
                        {
                            result += File.ReadAllText(programLocation + "\\user.txt");
                        }
                        catch { }
                        return result;
                    }
                    return "Сервер неработает";
                default:
                    return "Неизвестная команда";
            }
        }
    }
}