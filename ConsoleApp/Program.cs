using System;
using Microsoft.Data.Sqlite;
using Terminal.Gui;
using Entities;
using System.Collections.Generic;
using ServiceLib;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    class Program
    {
        static string serializedFilePath = "./serialized.xml";
        static void Main(string[] args)
        {
            string databaseFile = "../data/data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source ={databaseFile}");
            connection.Open();

            Application.Init();

            MenuBar menu = new MenuBar(new MenuBarItem[] {
          new MenuBarItem ("_File", new MenuItem [] {
              new MenuItem ("_Export", "", ExportFile),
              new MenuItem ("_Import", "", ImportFromFile),
              new MenuItem ("_Image", "", GenerateGraph),
              new MenuItem ("_Exit", "", OnQuit),
          }),
          new MenuBarItem("_Help", "", null),
      });

            MainWindow win = new MainWindow(databaseFile)
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };

            Application.Top.Add(menu, win);
            Application.Run();

            connection.Close();
        }
        static void OnQuit()
        {
            Application.RequestStop();
        }
        static void ExportFile()
        {
            ExportDialog dialog = new ExportDialog();
            Application.Run(dialog);
            if (!dialog._canceled && dialog.searchInput.Text != null)
            {
                try
                {
                    if (ExportDialog.fileLabel.Text == null || ExportDialog.fileLabel.Text == "default")
                    {
                        RemoteService.RemoteServiceCommand("GetPostDataForExport$" + dialog.searchInput.Text.ToString());
                        Post[] posts = Deserialize<Post[]>(serializedFilePath);
                        Export.Serialize(posts, "./output.xml");
                    }
                    else
                    {
                        RemoteService.RemoteServiceCommand("GetPostDataForExport$" + dialog.searchInput.Text.ToString());
                        Post[] posts = Deserialize<Post[]>(serializedFilePath);
                        Export.Serialize(posts, ExportDialog.fileLabel.Text.ToString());
                    }
                }
                catch
                {
                    int resultButtonIndex = MessageBox.ErrorQuery(
          "Error",
          "Failed to export to the file",
          "OK");
                }

            }
        }
        static void ImportFromFile()
        {
            ImportDialog dialog = new ImportDialog();
            Application.Run(dialog);
            if (!dialog._canceled && dialog.GetFilePath() != null)
            {
                try
                {
                    Post[] posts = Import.Deserialize(dialog.GetFilePath());
                    for (int i = 0; i < posts.Length; i++)
                    {
                        Serialize<Post>(posts[i], serializedFilePath);
                        RemoteService.RemoteServiceCommand("InsertImported$");
                    }
                }
                catch (System.Exception)
                {
                    int resultButtonIndex = MessageBox.ErrorQuery(
                    "Error",
                    "Failed to import from the file",
                    "OK");
                    return;
                }
            }
        }
        static void GenerateGraph()
        {
            GraphDialog dialog = new GraphDialog();
            Application.Run(dialog);
            if (!dialog._canceled && dialog.GetFilePath() != null)
            {
                try
                {
                    RemoteService.RemoteServiceCommand("GetAllPosts$");
                    List<Post> posts = Deserialize<List<Post>>(serializedFilePath);

                    RemoteService.RemoteServiceCommand("GetAllComments$");
                    List<Comment> comments = Deserialize<List<Comment>>(serializedFilePath);

                    Graph.CreateGraph(posts, comments, dialog.GetFilePath());
                }
                catch (System.Exception)
                {
                    int resultButtonIndex = MessageBox.ErrorQuery(
                    "Error",
                    "Failed to create graph in this file",
                    "OK");
                    return;
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
