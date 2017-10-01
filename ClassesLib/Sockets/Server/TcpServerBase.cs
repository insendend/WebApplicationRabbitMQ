using System;
using System.Net;
using System.Net.Sockets;
using ClassesLib.Sockets.Settings;

namespace ClassesLib.Sockets.Server
{
    public abstract class TcpServerBase : IDisposable
    {
        private readonly TcpServerSettings settings;
        private readonly Socket server;
        private readonly IPEndPoint endPoint;

        protected TcpServerBase(TcpServerSettings settings)
        {
            this.settings = settings;
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            endPoint = new IPEndPoint(settings.Ip, settings.Port);
        }

        public void Start()
        {
            try
            {
                server.Bind(endPoint);
                server.Listen(settings.ClientCount);

                server.BeginAccept(AcceptCallback, null);
                Console.WriteLine($"Awaiting for TCP requests at {server.LocalEndPoint}...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            try
            {
                var client = server.EndAccept(ar);
                Console.WriteLine($"Client {client.RemoteEndPoint} connected.");

                var state = new CustomStateObject { Client = client };

                client
                    .BeginReceive(state.BuffBytes, 0, state.BuffBytes.Length, SocketFlags.None, ReceiveCallback, state);

                server.BeginAccept(AcceptCallback, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                var state = ar.AsyncState as CustomStateObject;
                state.ReadBytes = state.Client.EndReceive(ar);
                Console.WriteLine($"Received from {state.Client.RemoteEndPoint} {state.ReadBytes} bytes.");

                state.ContentStream
                    .BeginWrite(state.BuffBytes, 0, state.ReadBytes, WriteCallback, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
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
                Console.WriteLine($"Sent to {client.RemoteEndPoint} {sentBytes} bytes.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                client.Shutdown(SocketShutdown.Both);
                client.Disconnect(false);
                Console.WriteLine($"Client {client.RemoteEndPoint} disconnected.");
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
