using System;
using System.Net;
using System.Net.Sockets;
using ClassesLib.Sockets.Settings;
using Serilog;

namespace ClassesLib.Sockets.Server
{
    public abstract class TcpServerBase : IDisposable
    {
        private readonly TcpServerSettings _settings;
        private Socket _server;
        private IPEndPoint _endPoint;
        protected readonly ILogger Logger;

        protected TcpServerBase(TcpServerSettings settings, ILogger logger)
        {
            _settings = settings;
            Logger = logger;          
        }

        public void Start()
        {
            try
            {
                _server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
                _endPoint = new IPEndPoint(_settings.Ip, _settings.Port);

                _server.Bind(_endPoint);
                _server.Listen(_settings.ClientCount);

                Logger.Information($"Awaiting for TCP requests at {_server.LocalEndPoint}");

                _server.BeginAccept(AcceptCallback, null);

                Logger.Information("Press [enter] to exit.");
                Console.WriteLine();
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Start failed");
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var client = _server.EndAccept(ar);
                Logger.Information($"Client {client.RemoteEndPoint} connected.");

                var state = new CustomStateObject { Client = client };

                client
                    .BeginReceive(state.BuffBytes, 0, state.BuffBytes.Length, SocketFlags.None, ReceiveCallback, state);

                _server.BeginAccept(AcceptCallback, null);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Accept failed");
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = ar.AsyncState as CustomStateObject;
                state.ReadBytes = state.Client.EndReceive(ar);
                Logger.Information($"Received from {state.Client.RemoteEndPoint} {state.ReadBytes} bytes.");
                state.ContentStream
                    .BeginWrite(state.BuffBytes, 0, state.ReadBytes, WriteCallback, state);
            }
            catch (Exception e)
            {
                Logger.Error(e, "Receive data failed");
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
                Logger.Error(e, "Write data failed");
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
                Logger.Information($"Sent to {client.RemoteEndPoint} {sentBytes} bytes.");
            }
            catch (Exception e)
            {
                Logger.Error(e, "Send data failed");
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
                client.Disconnect(false);
                Logger.Information($"Client {client.RemoteEndPoint} disconnected.");
                client.Close();
            }
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _server?.Dispose();
        }
    }
}
