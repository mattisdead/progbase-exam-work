using System.Net;
using System.Net.Sockets;
using static System.Console;
using System.Text;
using Terminal.Gui;

namespace ConsoleApp
{
    public class RemoteService
    {
        public static byte[] RemoteServiceCommand(string command)
        {
            IPAddress ipAddress = IPAddress.Loopback;
            int port = 3000;

            Socket socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint remoteEP = new IPEndPoint(ipAddress, port);

            try
            {
                socket.Connect(remoteEP);

                byte[] bytes = Encoding.ASCII.GetBytes(command);
                int nSent = socket.Send(bytes);

                byte[] buffer = new byte[1024];
                int nRecievedBytes = socket.Receive(buffer);

                socket.Close();
                return buffer;
            }
            catch
            {
                MessageBox.ErrorQuery("Connection", "Could not connect to server", "Ok");
                RemoteServiceCommand(command);
                return null;
            }
        }
    }
}