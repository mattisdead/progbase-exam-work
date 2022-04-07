using static System.Console;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using ServiceLib;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

namespace Entities
{
    public class RemoteUI
    {
        static SqliteConnection connection;
        static string serializedFilePath = "../ConsoleApp/serialized.xml";
        static string databaseFile;
        public static void Main(string[] args)
        {
            databaseFile = "../data/data.db";
            connection = new SqliteConnection($"Data Source ={databaseFile}");
            connection.Open();

            IPAddress address = IPAddress.Loopback;
            int port = 3000;

            IPEndPoint ipEndPoint = new IPEndPoint(address, port);

            Socket serverSocket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(ipEndPoint);
                serverSocket.Listen();
                WriteLine("Server is listening on port: " + port);
                while (true)
                {
                    WriteLine("Waiting for a new client...");
                    Socket newClient = serverSocket.Accept();
                    WriteLine("Client has connected: " + newClient.RemoteEndPoint);

                    Thread thread = new Thread(StartNewThread);
                    thread.Start(newClient);
                }
            }
            catch
            {
                Error.WriteLine("Could not start a server on port: " + port);
            }
        }
        static void StartNewThread(object obj)
        {
            Socket socket = (Socket)obj;
            ProcessClient(socket);
            socket.Close();
            WriteLine("Connection closed");
        }
        static void ProcessClient(Socket newClient)
        {
            byte[] buffer = new byte[1024];

            IService service = new Service(connection, databaseFile);

            while (true)
            {
                try
                {
                    WriteLine("Server is waiting for a message....");

                    int nBytes = newClient.Receive(buffer);
                    string inputText = Encoding.ASCII.GetString(buffer, 0, nBytes);

                    string[] command = inputText.Split('$');

                    if (command[0].Equals("DeleteComment"))
                    {
                        bool deleted = service.DeleteComment(int.Parse(command[1]));
                        buffer = Serialize<bool>(deleted, serializedFilePath);
                    }
                    else if (command[0].Equals("DeleteCommentById"))
                    {
                        int deleted = service.DeleteCommentById(int.Parse(command[1]));
                        buffer = Serialize<int>(deleted, serializedFilePath);
                    }
                    else if (command[0].Equals("DeletePost"))
                    {
                        bool deleted = service.DeletePost(int.Parse(command[1]));
                        buffer = Serialize<bool>(deleted, serializedFilePath);
                    }
                    else if (command[0].Equals("DeletePostById"))
                    {
                        int deleted = service.DeletePostById(int.Parse(command[1]));
                        buffer = Serialize<int>(deleted, serializedFilePath);
                    }
                    else if (command[0].Equals("DeleteUser"))
                    {
                        bool deleted = service.DeleteUser(int.Parse(command[1]));
                        buffer = Serialize<bool>(deleted, serializedFilePath);
                    }
                    else if (command[0].Equals("DeleteUserById"))
                    {
                        int deleted = service.DeleteUserById(int.Parse(command[1]));
                        buffer = Serialize<int>(deleted, serializedFilePath);
                    }
                    else if (command[0].Equals("FindUser"))
                    {
                        User user = service.FindUser(command[1]);
                        buffer = Serialize<User>(user, serializedFilePath);
                    }
                    else if (command[0].Equals("GetAllComments"))
                    {
                        List<Comment> comments = service.GetAllComments();
                        buffer = Serialize<List<Comment>>(comments, serializedFilePath);
                    }
                    else if (command[0].Equals("GetAllCommentsToThePost"))
                    {
                        Post post = Deserialize<Post>(serializedFilePath);
                        List<Comment> comments = service.GetAllCommentsToThePost(post);
                        buffer = Serialize<List<Comment>>(comments, serializedFilePath);
                    }
                    else if (command[0].Equals("GetAllUsers"))
                    {
                        List<User> users = service.GetAllUsers();
                        buffer = Serialize<List<User>>(users, serializedFilePath);
                    }
                    else if (command[0].Equals("GetAllUsersComments"))
                    {
                        Comment[] comments = service.GetAllUsersComments(int.Parse(command[1]));
                        buffer = Serialize<Comment[]>(comments, serializedFilePath);
                    }
                    else if (command[0].Equals("GetAllUsersPosts"))
                    {
                        Post[] posts = service.GetAllUsersPosts(int.Parse(command[1]));
                        buffer = Serialize<Post[]>(posts, serializedFilePath);
                    }
                    else if (command[0].Equals("GetCommentById"))
                    {
                        Comment comment = service.GetCommentById(int.Parse(command[1]));
                        buffer = Serialize<Comment>(comment, serializedFilePath);
                    }
                    else if (command[0].Equals("GetCommentDataForImage"))
                    {
                        int number = service.GetCommentDataForImage(System.DateTime.Parse(command[1]));
                        buffer = Serialize<int>(number, serializedFilePath);
                    }
                    else if (command[0].Equals("GetCommentPage"))
                    {
                        List<Comment> comments = service.GetCommentPage(int.Parse(command[1]), int.Parse(command[2]));
                        buffer = Serialize<List<Comment>>(comments, serializedFilePath);
                    }
                    else if (command[0].Equals("GetCommentPagesCount"))
                    {
                        int pages = service.GetCommentPagesCount(int.Parse(command[1]));
                        buffer = Serialize<int>(pages, serializedFilePath);
                    }
                    else if (command[0].Equals("GetCommentSearchPagesCount"))
                    {
                        int pages = service.GetCommentSearchPagesCount(command[1], int.Parse(command[2]));
                        buffer = Serialize<int>(pages, serializedFilePath);
                    }
                    else if (command[0].Equals("GetCommentSearchPage"))
                    {
                        List<Comment> comments = service.GetCommentSearchPage(command[1], int.Parse(command[2]), int.Parse(command[3]));
                        buffer = Serialize<List<Comment>>(comments, serializedFilePath);
                    }
                    else if (command[0].Equals("GetCommentsForPostPage"))
                    {
                        Post post = Deserialize<Post>(serializedFilePath);
                        List<Comment> comments = service.GetCommentsForPostPage(int.Parse(command[1]), int.Parse(command[2]), post);
                        buffer = Serialize<List<Comment>>(comments, serializedFilePath);
                    }
                    else if (command[0].Equals("GetCommentsForPostPagesCount"))
                    {
                        Post post = Deserialize<Post>(serializedFilePath);
                        int pages = service.GetCommentsForPostPagesCount(int.Parse(command[1]), post);
                        buffer = Serialize<int>(pages, serializedFilePath);
                    }
                    else if (command[0].Equals("GetPostById"))
                    {
                        Post post = service.GetPostById(int.Parse(command[1]));
                        buffer = Serialize<Post>(post, serializedFilePath);
                    }
                    else if (command[0].Equals("GetPostPage"))
                    {
                        List<Post> posts = service.GetPostPage(int.Parse(command[1]), int.Parse(command[2]));
                        buffer = Serialize<List<Post>>(posts, serializedFilePath);
                    }
                    else if (command[0].Equals("GetPostPagesCount"))
                    {
                        int pages = service.GetPostPagesCount(int.Parse(command[1]));
                        buffer = Serialize<int>(pages, serializedFilePath);
                    }
                    else if (command[0].Equals("GetSearchUserPage"))
                    {
                        List<User> users = service.GetSearchUserPage(command[1], int.Parse(command[2]), int.Parse(command[3]));
                        buffer = Serialize<List<User>>(users, serializedFilePath);
                    }
                    else if (command[0].Equals("GetSearchUserPagesCount"))
                    {
                        int pages = service.GetSearchUserPagesCount(command[1], int.Parse(command[2]));
                        buffer = Serialize<int>(pages, serializedFilePath);
                    }
                    else if (command[0].Equals("GetUserById"))
                    {
                        User user = service.GetUserById(int.Parse(command[1]));
                        buffer = Serialize<User>(user, serializedFilePath);
                    }
                    else if (command[0].Equals("GetUserPage"))
                    {
                        List<User> users = service.GetUserPage(int.Parse(command[1]), int.Parse(command[2]));
                        buffer = Serialize<List<User>>(users, serializedFilePath);
                    }
                    else if (command[0].Equals("GetUserPagesCount"))
                    {
                        int pages = service.GetUserPagesCount(int.Parse(command[1]));
                        buffer = Serialize<int>(pages, serializedFilePath);
                    }
                    else if (command[0].Equals("InsertComment"))
                    {
                        Comment comment = Deserialize<Comment>(serializedFilePath);
                        int id = service.InsertComment(comment);
                        buffer = Serialize<int>(id, serializedFilePath);
                    }
                    else if (command[0].Equals("InsertCommentWithId"))
                    {
                        Comment comment = Deserialize<Comment>(serializedFilePath);
                        int id = service.InsertCommentWithId(comment);
                        buffer = Serialize<int>(id, serializedFilePath);
                    }
                    else if (command[0].Equals("InsertPost"))
                    {
                        Post post = Deserialize<Post>(serializedFilePath);
                        int nChanged = service.InsertPost(post);
                        buffer = Serialize<int>(nChanged, serializedFilePath);
                    }
                    else if (command[0].Equals("InsertPostWithId"))
                    {
                        Post post = Deserialize<Post>(serializedFilePath);
                        int id = service.InsertPostWithId(post);
                        buffer = Serialize<int>(id, serializedFilePath);
                    }
                    else if (command[0].Equals("InsertUser"))
                    {
                        User user = Deserialize<User>(serializedFilePath);
                        int id = service.InsertUser(user);
                        buffer = Serialize<int>(id, serializedFilePath);
                    }
                    else if (command[0].Equals("InsertUserWithId"))
                    {
                        User user = Deserialize<User>(serializedFilePath);
                        int id = service.InsertUserWithId(user);
                        buffer = Serialize<int>(id, serializedFilePath);
                    }
                    else if (command[0].Equals("UpdateComment"))
                    {
                        Comment comment = Deserialize<Comment>(serializedFilePath);
                        bool nChanged = service.UpdateComment(int.Parse(command[1]), comment);
                        buffer = Serialize<bool>(nChanged, serializedFilePath);
                    }
                    else if (command[0].Equals("UpdatePost"))
                    {
                        Post post = Deserialize<Post>(serializedFilePath);
                        bool nChanged = service.UpdatePost(int.Parse(command[1]), post);
                        buffer = Serialize<bool>(nChanged, serializedFilePath);
                    }
                    else if (command[0].Equals("UpdateUser"))
                    {
                        User user = Deserialize<User>(serializedFilePath);
                        bool nChanged = service.UpdateUser(int.Parse(command[1]), user);
                        buffer = Serialize<bool>(nChanged, serializedFilePath);
                    }
                    else if (command[0].Equals("GetPostDataForExport"))
                    {
                        Post[] posts = service.GetPostDataForExport(command[1]);
                        buffer = Serialize<Post[]>(posts, serializedFilePath);
                    }
                    else if (command[0].Equals("InsertImported"))
                    {
                        Post post = Deserialize<Post>(serializedFilePath);
                        int x = service.InsertImported(post);
                        buffer = Serialize<int>(x, serializedFilePath);
                    }
                    else if (command[0].Equals("GetAllPosts"))
                    {
                        List<Post> posts = service.GetAllPosts();
                        buffer = Serialize<List<Post>>(posts, serializedFilePath);
                    }
                    else if (command[0].Equals("GetPostSearchPagesCount"))
                    {
                        int pages = service.GetPostSearchPagesCount(command[1], int.Parse(command[2]));
                        buffer = Serialize<int>(pages, serializedFilePath);
                    }
                    else if (command[0].Equals("GetPostSearchPage"))
                    {
                        List<Post> posts = service.GetPostSearchPage(command[1], int.Parse(command[2]), int.Parse(command[3]));
                        buffer = Serialize<List<Post>>(posts, serializedFilePath);
                    }
                    else if (command[0].Equals("UpdateUserLogin"))
                    {
                        User user = Deserialize<User>(serializedFilePath);
                        bool changed = service.UpdateUserLogin(int.Parse(command[1]), user);
                        buffer = Serialize<bool>(changed, serializedFilePath);
                    }

                    int nSentBytes = newClient.Send(buffer);
                    WriteLine("Response was sent");
                }
                catch (System.Exception ex)
                {
                    if (!ex.ToString().StartsWith("System.Net.Sockets."))
                    {
                        System.Console.WriteLine(ex);
                        WriteLine();
                    }
                    break;
                }
            }
        }
        public static byte[] Serialize<T>(T data, string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            System.IO.File.WriteAllText(filePath, "");
            System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath);
            ser.Serialize(writer, data);
            writer.Close();
            string text = System.IO.File.ReadAllText(filePath);
            byte[] bytes = Encoding.ASCII.GetBytes(text);


            return bytes;
        }
        public static T Deserialize<T>(string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(filePath);
            T value = (T)ser.Deserialize(reader);
            reader.Close();
            return value;
        }
    }
}