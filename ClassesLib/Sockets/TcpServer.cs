using ClassesLib.Serialization;
using ClassesLib.Sockets.Settings;

namespace ClassesLib.Sockets
{
    public class TcpServer : TcpServerBase
    {
        public TcpServer(TcpServerSettings settings) : base(settings)
        {
        }

        protected override byte[] ReceivedDataHandler(byte[] recvBytes)
        {
            ISerializer<TaskInfo> serializer = new TaskInfoSerializer();
            var taskInfo = serializer.DesirializeToObj(recvBytes);

            if (taskInfo is null)
                return null;

            taskInfo.AddHours(1);

            return serializer.SerializeToBytes(taskInfo);
        }
    }
}
