using System;
using System.Net;
using System.Net.Sockets;
using ClassesLib.Sockets.Settings;
using Serilog;

namespace ClassesLib.Sockets.Server
{
    public abstract class TcpServerBase : IDisposable
    {
        private readonly TcpServerSettings settings;
        private readonly Socket server;
        private readonly IPEndPoint endPoint;
        protected readonly ILogger logger;

        protected TcpServerBase(TcpServerSettings settings, ILogger logger)
        {
            this.settings = settings;
            this.logger = logger;
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            endPoint = new IPEndPoint(settings.Ip, settings.Port);
        }

        public void Start()
        {
            try
            {
                server.Bind(endPoint);
                server.Listen(settings.ClientCount);

                logger.Information($"Awaiting for TCP requests at {server.LocalEndPoint}");

                server.BeginAccept(AcceptCallback, null);
            }
            catch (Exception e)
            {
                logger.Error(e, "Start failed");
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var client = server.EndAccept(ar);
                logger.Information($"Client {client.RemoteEndPoint} connected.");

                var state = new CustomStateObject { Client = client };

                client
                    .BeginReceive(state.BuffBytes, 0, state.BuffBytes.Length, SocketFlags.None, ReceiveCallback, state);

                server.BeginAccept(AcceptCallback, null);
            }
            catch (Exception e)
            {
                logger.Error(e, "Accept failed");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = ar.AsyncState as CustomStateObject;
                state.ReadBytes = state.Client.EndReceive(ar);
                logger.Information($"Received from {state.Client.RemoteEndPoint} {state.ReadBytes} bytes.");
                state.ContentStream
                    .BeginWrite(state.BuffBytes, 0, state.ReadBytes, WriteCallback, state);
            }
            catch (Exception e)
            {
                logger.Error(e, "Receive data failed");
            }
        }

        private void WriteCallback(IAsyncResult ar)
        {
            var state = ar.AsyncState as CustomStateObject;

            try
            {
                state.ContentStream.EndWrite(ar);

                if (state.ReadBytes < state.BuffBytes.Length)
                {
                    var newData = ProcessData(state.ContentStream.ToArray());
                    state.Client
                        .BeginSend(newData, 0, newData.Length, SocketFlags.None, SendCallback, state.Client);
                }
                else
                {
                    state.Client
                        .BeginReceive(state.BuffBytes, 0, state.BuffBytes.Length, SocketFlags.None, ReceiveCallback, state);
                }
            }
            catch (Exception e)
            {
                logger.Error(e, "Write data failed");
            }
        }

        protected virtual byte[] ProcessData(byte[] data)
        {
            return data;
        }

        private void SendCallback(IAsyncResult ar)
        {
            var client = ar.AsyncState as Socket;

            try
            {
                var sentBytes = client.EndSend(ar);
                logger.Information($"Sent to {client.RemoteEndPoint} {sentBytes} bytes.");
            }
            catch (Exception e)
            {
                logger.Error(e, "Send data failed");
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
                client.Disconnect(false);
                logger.Information($"Client {client.RemoteEndPoint} disconnected.");
                client.Close();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            server?.Dispose();
        }
    }
}
