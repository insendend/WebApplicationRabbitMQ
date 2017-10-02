using System;
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
                ISerializer<TaskInfo> serializer = new TaskInfoSerializer();
                var taskInfo = serializer.DesirializeToObj(recvBytes);

                if (taskInfo is null)
                    return null;
                logger.Information("Received object: {@taskInfo}", taskInfo);

                taskInfo.AddHours(1);
                logger.Information("Added one hour: {@taskInfo}", taskInfo.Time);

                return serializer.SerializeToBytes(taskInfo);
            }
            catch (Exception e)
            {
                logger.Error(e, "Processing data failed");
                return null;
            }   
        }
    }
}
