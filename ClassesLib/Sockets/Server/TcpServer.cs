using System;
using ClassesLib.Serialization;
using ClassesLib.Sockets.Settings;

namespace ClassesLib.Sockets.Server
{
    public class TcpServer : TcpServerBase
    {
        public TcpServer(TcpServerSettings settings) : base(settings)
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

                taskInfo.AddHours(1);

                return serializer.SerializeToBytes(taskInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }   
        }
    }
}
