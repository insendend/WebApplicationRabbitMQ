using System;
using Autofac;
using ClassesLib.Autofac;
using ClassesLib.Serialization;
using ClassesLib.Sockets.Settings;
using Serilog;

namespace ClassesLib.Sockets.Server
{
    public class TcpServer : TcpServerBase
    {
        public TcpServer(TcpServerSettings settings, ILogger logger) : base(settings, logger)
        {
        }
       
        protected override byte[] ProcessData(byte[] recvBytes)
        {
            try
            {
                var serializer = IocContainer.Container.Resolve<ISerializer<TaskInfo>>();
                var taskInfo = serializer.DesirializeToObj(recvBytes);

                if (taskInfo is null)
                    return null;
                Logger.Information("Received object: {@taskInfo}", taskInfo);

                taskInfo.AddHours(1);
                Logger.Information("Added one hour: {@taskInfo}", taskInfo.Time);

                return serializer.SerializeToBytes(taskInfo);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Processing data failed");
                return null;
            }   
        }
    }
}
