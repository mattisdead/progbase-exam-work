using Terminal.Gui;
using System.Collections.Generic;
using Entities;
using Microsoft.Data.Sqlite;
using ServiceLib;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    public class ShowCommentsDialog : Dialog
    {
        Post post;
        private Label pageLbl = new Label("0");
        private Label totalPagesLbl = new Label();
        private int page = 1;
        private int pageLenght = 5;
        private ListView allCommentsListView = new ListView(new List<Comment>());
        private Button closeBtn;
        private FrameView frameView;
        private Button prevPageBtn;
        private Button nextPageBtn;
        string serializedFilePath = "./serialized.xml";
        public ShowCommentsDialog(Post post)
        {
            this.post = post;
            SqliteConnection connection = new SqliteConnection($"Data Source ={MainWindow.filePath}");
            connection.Open();

            Serialize<Post>(post, serializedFilePath);
            RemoteService.RemoteServiceCommand("GetCommentsForPostPage$" + page + "$" + pageLenght);
            List<Comment> allComm = Deserialize<List<Comment>>(serializedFilePath);
            allCommentsListView = new ListView(Deserialize<List<Comment>>(serializedFilePath))
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allCommentsListView.OpenSelectedItem += OnOpenComment;

            prevPageBtn = new Button(2, 6, "<");
            prevPageBtn.Clicked += OnPreviousCommentPageClicked;

            Serialize<Post>(post, serializedFilePath);
            RemoteService.RemoteServiceCommand("GetCommentsForPostPagesCount$" + pageLenght);
            int pages = Deserialize<int>(serializedFilePath);

            pageLbl = new Label(page.ToString())
            {
                X = Pos.Right(prevPageBtn) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            totalPagesLbl = new Label(pages.ToString())
            {
                X = Pos.Right(pageLbl) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            if (pages == 0) this.pageLbl.Text = "0";
            else this.pageLbl.Text = page.ToString();

            nextPageBtn = new Button(">")
            {
                X = Pos.Right(totalPagesLbl) + 2,
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextCommentPageClicked;
            this.Add(prevPageBtn, pageLbl, totalPagesLbl, nextPageBtn);

            frameView = new FrameView("Comments")
            {
                X = 2,
                Y = 8,
                Width = Dim.Fill() - 4,
                Height = pageLenght + 4,
            };
            frameView.Add(allCommentsListView);
            this.Add(frameView);

            closeBtn = new Button("Close")
            {
                X = Pos.Left(frameView),
                Y = Pos.Bottom(frameView) + 1,
            };
            closeBtn.Clicked += CloseCommentsButtonClicked;
            this.Add(closeBtn);
        }
        private void OnOpenComment(ListViewItemEventArgs args)
        {
            Comment comment = (Comment)args.Value;
            OpenCommentDialog dialog = new OpenCommentDialog(comment);
            dialog.SetComment(comment);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                if (MainWindow.currentUser.id != comment.author.id && MainWindow.currentUser.isModerator != 1)
                {
                    MessageBox.ErrorQuery("Delete comments", "You're not allowed to delete this comment", "Ok");
                    return;
                }
                RemoteService.RemoteServiceCommand("DeleteComment$" + comment.id);
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    Serialize<Post>(comment.post, MainWindow.serializedFilePath);
                    RemoteService.RemoteServiceCommand("GetCommentsForPostPagesCount$" + pageLenght);
                    int pages = Deserialize<int>(serializedFilePath);
                    if (page > pages && page > 0)
                    {
                        page--;
                        this.ShowCurrentCommentPage();
                    }
                    Serialize<Post>(comment.post, MainWindow.serializedFilePath);
                    RemoteService.RemoteServiceCommand("GetCommentsForPostPage$" + page + "$" + pageLenght);
                    allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));

                    if (comment.post != null && comment.post.idOfPinnedComment == comment.id)
                    {
                        comment.post.idOfPinnedComment = 0;
                    }
                }
                else
                {
                    MessageBox.ErrorQuery("Delete comment", "Failed to delete comment", "Ok");
                }
            }
            if (dialog.edited)
            {
                if (MainWindow.currentUser.id != comment.author.id)
                {
                    MessageBox.ErrorQuery("Edit comment", "You're not allowed to edit this comment", "Ok");
                    return;
                }
                comment = dialog.GetComment();
                int postId = comment.post.id;
                comment.post = new Post(postId);
                Serialize<Comment>(dialog.GetComment(), serializedFilePath);
                RemoteService.RemoteServiceCommand("UpdateComment$" + comment.id);
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetCommentPage$" + page + "$" + pageLenght);
                    allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));
                }
                else
                {
                    MessageBox.ErrorQuery("Update comment", "Failed to update comment", "Ok");
                }
            }
        }
        private void ShowCurrentCommentPage()
        {
            Serialize<Post>(post, serializedFilePath);
            RemoteService.RemoteServiceCommand("GetCommentsForPostPagesCount$" + pageLenght);
            this.totalPagesLbl.Text = Deserialize<int>(serializedFilePath).ToString();

            if (totalPagesLbl.Text == "0") this.pageLbl.Text = "0";
            else this.pageLbl.Text = page.ToString();

            Serialize<Post>(post, serializedFilePath);
            RemoteService.RemoteServiceCommand("GetCommentsForPostPage$" + page + "$" + pageLenght);
            this.allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));
        }
        private void CloseCommentsButtonClicked()
        {
            Application.RequestStop();
        }
        private void OnNextCommentPageClicked()
        {
            Serialize<Post>(post, serializedFilePath);
            RemoteService.RemoteServiceCommand("GetCommentsForPostPagesCount$" + pageLenght);
            int totalPages = Deserialize<int>(serializedFilePath);
            if (page >= totalPages)
            {
                return;
            }
            this.page++;
            ShowCurrentCommentPage();
        }
        private void OnPreviousCommentPageClicked()
        {
            if (page <= 1)
            {
                return;
            }
            this.page--;
            ShowCurrentCommentPage();
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