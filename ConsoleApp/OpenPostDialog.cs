using Terminal.Gui;
using Entities;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using ServiceLib;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    public class OpenPostDialog : Dialog
    {
        public Post post;
        public bool edited;
        public bool deleted;
        private TextField postInput;
        private TextField idOfPinnedCommentInput;
        private TextField userIdInput;
        private TextField commentsIdInput;
        private TextField dateInput;
        private TextField postIdInput;
        private ListView allCommentsListView = new ListView(new List<Comment>());
        private int page = 1;
        private int pageLenght = 5;
        private Label pageLbl = new Label("1");
        private Label totalPagesLbl = new Label();
        string serializedFilePath = "./serialized.xml";
        public OpenPostDialog()
        {
            this.Title = "Open post";

            Button cancelBtn = new Button("Back");
            cancelBtn.Clicked += OnCreateDialogCancelled;

            this.AddButton(cancelBtn);

            Label idOfPinnedCommentLbl = new Label(2, 8, "Id of pinned comment:");
            idOfPinnedCommentInput = new TextField("")
            {
                X = Pos.Right(idOfPinnedCommentLbl) + 2,
                Y = Pos.Top(idOfPinnedCommentLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(idOfPinnedCommentLbl, idOfPinnedCommentInput);

            Label postIdLbl = new Label(2, 4, "Post id:");
            postIdInput = new TextField("")
            {
                X = Pos.Right(idOfPinnedCommentLbl) + 2,
                Y = Pos.Top(postIdLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(postIdLbl, postIdInput);

            Label postLbl = new Label(2, 6, "Post:");
            postInput = new TextField("")
            {
                X = Pos.Right(idOfPinnedCommentLbl) + 2,
                Y = Pos.Top(postLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(postLbl, postInput);

            Label userIdLbl = new Label(2, 10, "User id:");
            userIdInput = new TextField("")
            {
                X = Pos.Right(idOfPinnedCommentLbl) + 2,
                Y = Pos.Top(userIdLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(userIdLbl, userIdInput);

            Label commentsIdLbl = new Label(2, 12, "Comments id:");
            commentsIdInput = new TextField("")
            {
                X = Pos.Right(idOfPinnedCommentLbl) + 2,
                Y = Pos.Top(commentsIdLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(commentsIdLbl, commentsIdInput);

            Label dateLbl = new Label(2, 14, "Date:");
            dateInput = new TextField("")
            {
                X = Pos.Right(idOfPinnedCommentLbl) + 2,
                Y = Pos.Top(dateLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(dateLbl, dateInput);

            Button editPostBtn = new Button(2, 16, "Edit");
            editPostBtn.Clicked += OnEditPostButtonClicked;
            this.Add(editPostBtn);

            Button deletePostBtn = new Button("Delete")
            {
                X = Pos.Right(editPostBtn) + 2,
                Y = Pos.Top(editPostBtn),
            };
            deletePostBtn.Clicked += OnDeletePostButtonClicked;
            this.Add(deletePostBtn);

            Button showAllCommentsBtn = new Button("Show comments")
            {
                X = Pos.Right(deletePostBtn) + 2,
                Y = Pos.Top(editPostBtn),
            };
            showAllCommentsBtn.Clicked += OnShowAllCommentsButtonClicked;
            this.Add(showAllCommentsBtn);

            Button addCommentBtn = new Button("Add comment")
            {
                X = Pos.Right(showAllCommentsBtn) + 2,
                Y = Pos.Top(editPostBtn),
            };
            addCommentBtn.Clicked += OnAddCommentButtonClicked;
            this.Add(addCommentBtn);
        }
        private void OnCreateDialogCancelled()
        {
            Application.RequestStop();
        }
        private void OnEditPostButtonClicked()
        {
            EditPostDialog dialog = new EditPostDialog();
            dialog.SetPost(this.post);

            Application.Run(dialog);

            if (!dialog._canceled)
            {
                Post updatedPost = dialog.GetPost();
                updatedPost.id = this.post.id;
                updatedPost.idOfPinnedComment = this.post.idOfPinnedComment;
                updatedPost.comments = this.post.comments;
                updatedPost.author = this.post.author;
                updatedPost.date = this.post.date;

                this.edited = true;
                this.SetPost(updatedPost);
            }
        }
        public Post GetPost()
        {
            return this.post;
        }
        public void SetPost(Post post)
        {
            this.post = post;
            this.postInput.Text = post.post;
            this.idOfPinnedCommentInput.Text = post.idOfPinnedComment.ToString();
            this.userIdInput.Text = post.author.id.ToString();
            string allCommentsId = "";
            if (post.comments != null)
            {
                for (int i = 0; i < post.comments.Length; i++)
                {
                    allCommentsId += post.comments[i].id + ",";
                }
            }
            this.commentsIdInput.Text = allCommentsId;
            this.dateInput.Text = post.date.ToString();
            this.postIdInput.Text = post.id.ToString();
        }
        private void OnDeletePostButtonClicked()
        {
            int index = MessageBox.Query("Delete post", "Are you sure you want to delete this post?", "No", "Yes");
            if (index == 1)
            {
                deleted = true;
                Application.RequestStop();
            }
        }
        private void OnShowAllCommentsButtonClicked()
        {
            ShowCommentsDialog dialog = new ShowCommentsDialog(post);
            Application.Run(dialog);
        }
        private void OnAddCommentButtonClicked()
        {
            SqliteConnection connection = new SqliteConnection($"Data Source ={MainWindow.filePath}");
            connection.Open();

            CreateCommentDialog dialog = new CreateCommentDialog();
            Application.Run(dialog);

            if (!dialog._canceled)
            {
                Comment comment = dialog.GetComment();
                comment.author = MainWindow.currentUser;
                comment.post = this.post;

                Serialize<Comment>(comment, serializedFilePath);
                RemoteService.RemoteServiceCommand("InsertComment$");
                int id = Deserialize<int>(serializedFilePath);
                comment.id = id;

                RemoteService.RemoteServiceCommand("GetPostById$" + comment.post.id);
                Post getByIdPost = Deserialize<Post>(serializedFilePath);

                if (comment.post != null && comment.post.comments == null)
                {
                    comment.post.comments = new Comment[1];
                    comment.post.comments[0] = new Comment(comment.id);
                }
                else if (comment.post != null && getByIdPost != null)
                {
                    Array.Resize(ref comment.post.comments, comment.post.comments.Length + 1);
                    comment.post.comments[comment.post.comments.Length - 1] = new Comment(comment.id);


                }
                Serialize<Post>(comment.post, serializedFilePath);
                RemoteService.RemoteServiceCommand("UpdatePost$" + comment.post.id);

                RemoteService.RemoteServiceCommand("GetUserById$" + comment.author.id);
                User getByIdUser = Deserialize<User>(serializedFilePath);

                if (comment.author != null && comment.author.comments == null)
                {
                    comment.author.comments = new Comment[1];
                    comment.author.comments[0] = new Comment(comment.id);
                }
                if (comment.author != null && getByIdUser != null)
                {
                    Array.Resize(ref comment.author.comments, comment.author.comments.Length + 1);
                    comment.author.comments[comment.author.comments.Length - 1] = new Comment(comment.id);

                    Serialize<User>(comment.author, serializedFilePath);
                    RemoteService.RemoteServiceCommand("UpdateUserLogin$" + comment.author.id);
                }

                Serialize<Post>(post, serializedFilePath);
                RemoteService.RemoteServiceCommand("GetCommentsForPostPage$" + page + "$" + pageLenght);
                allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));

                Serialize<Post>(post, serializedFilePath);
                RemoteService.RemoteServiceCommand("GetCommentsForPostPagesCount$" + pageLenght);
                int pages = Deserialize<int>(serializedFilePath);
                string x = totalPagesLbl.Text.ToString();
                if (x != "" && pages > int.Parse(totalPagesLbl.Text.ToString()))
                {
                    page++;
                    this.ShowCurrentCommentPage();
                }
                Serialize<Post>(post, serializedFilePath);
                RemoteService.RemoteServiceCommand("GetCommentsForPostPage$" + page + "$" + pageLenght);
                allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));
            }
        }
        private void ShowCurrentCommentPage()
        {
            this.pageLbl.Text = page.ToString();

            Serialize<Post>(post, serializedFilePath);
            RemoteService.RemoteServiceCommand("GetCommentsForPostPagesCount$" + pageLenght);
            this.totalPagesLbl.Text = Deserialize<int>(serializedFilePath).ToString();

            Serialize<Post>(post, serializedFilePath);
            RemoteService.RemoteServiceCommand("GetCommentsForPostPage$" + page + "$" + pageLenght);
            this.allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));
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