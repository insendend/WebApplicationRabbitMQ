using System;
using System.IO;
using System.Net;
using ClassesLib.Sockets.Server;
using ClassesLib.Sockets.Settings;
using Serilog;
using Serilog.Formatting.Json;

namespace AsyncTcpServer
{
    class TcpServerMain
    {
        static void Main(string[] args)
        {
            Console.Title = "TCP Server";

            var logFile = Path.Combine(Environment.CurrentDirectory, "logFile.json");
            var logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.File(new JsonFormatter(), logFile)
                .CreateLogger();
            var servSettings = new TcpServerSettings {Ip = IPAddress.Any, Port = 3333, ClientCount = 1024};
            var serv = new TcpServer(servSettings, logger);
            serv.Start();

            logger.Information("Press [enter] to exit.");
            Console.WriteLine();
            Console.ReadLine();
        }
    }
}
