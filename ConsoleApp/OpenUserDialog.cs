using Terminal.Gui;
using Entities;
using Microsoft.Data.Sqlite;
using ServiceLib;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    public class OpenUserDialog : Dialog
    {
        public bool deleted;
        public bool edited;
        public bool passwordEdited;
        private string oldPassword;
        protected User user;
        private TextField userLoginInput;
        private TextField userPasswordInput;
        private TextField useridInput;
        private Label isModeratorInput;
        private TextField postsIdInput;
        private TextField commentsIdInput;
        string serializedFilePath = "./serialized.xml";
        public OpenUserDialog()
        {
            this.Title = "Open user";

            Button cancelBtn = new Button("Back");
            cancelBtn.Clicked += OnCreateDialogCancelled;

            this.AddButton(cancelBtn);

            Label commentsIdLbl = new Label(2, 12, "Comments id:");
            commentsIdInput = new TextField("")
            {
                X = Pos.Right(commentsIdLbl) + 2,
                Y = Pos.Top(commentsIdLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(commentsIdLbl, commentsIdInput);

            Label userIdLbl = new Label(2, 4, "User id:");
            useridInput = new TextField("")
            {
                X = Pos.Right(commentsIdLbl) + 2,
                Y = Pos.Top(userIdLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(userIdLbl, useridInput);

            Label userLoginLbl = new Label(2, 6, "Login:");
            userLoginInput = new TextField("")
            {
                X = Pos.Right(commentsIdLbl) + 2,
                Y = Pos.Top(userLoginLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(userLoginLbl, userLoginInput);

            Label isModeratorLbl = new Label(2, 8, "Moderator:");
            isModeratorInput = new Label("")
            {
                X = Pos.Right(commentsIdLbl) + 2,
                Y = Pos.Top(isModeratorLbl),
                Width = Dim.Fill() - 2
            };
            this.Add(isModeratorLbl, isModeratorInput);

            Label postsIdLbl = new Label(2, 10, "Posts id:");
            postsIdInput = new TextField("")
            {
                X = Pos.Right(commentsIdLbl) + 2,
                Y = Pos.Top(postsIdLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(postsIdLbl, postsIdInput);

            Button editUserBtn = new Button(2, 14, "Edit");
            editUserBtn.Clicked += OnEditUserButtonClicked;
            this.Add(editUserBtn);

            Button deleteUserBtn = new Button("Delete")
            {
                X = Pos.Right(editUserBtn) + 2,
                Y = Pos.Top(editUserBtn),
            };
            deleteUserBtn.Clicked += OnDeleteUserButtonClicked;
            this.Add(deleteUserBtn);
        }
        private void OnDeleteUserButtonClicked()
        {
            int index = MessageBox.Query("Delete user", "Are you sure you want to delete this user?", "No", "Yes");
            if (index == 1)
            {
                deleted = true;
                Application.RequestStop();
            }
        }
        private void OnEditUserButtonClicked()
        {
            EditUserDialog dialog = new EditUserDialog();
            dialog.SetUser(this.user);

            Application.Run(dialog);

            if (!dialog._canceled)
            {
                User updatedUser = dialog.GetUser();
                if (oldPassword != updatedUser.password) passwordEdited = true;
                else passwordEdited = false;
                this.edited = true;
                this.SetUser(updatedUser);
            }
        }
        public User GetUser()
        {
            return this.user;
        }
        private void OnCreateDialogCancelled()
        {
            Application.RequestStop();
        }
        public void SetUser(User user)
        {
            this.user = user;
            this.userLoginInput.Text = user.login;
            oldPassword = user.password;
            this.useridInput.Text = user.id.ToString();
            if (user.isModerator == 1) this.isModeratorInput.Text = "true";
            else this.isModeratorInput.Text = "false";
            string allPosts = "";
            if (user.posts != null)
            {
                for (int i = 0; i < user.posts.Length; i++)
                {
                    RemoteService.RemoteServiceCommand("GetPostById$" + user.posts[i].id);
                    Post post = Deserialize<Post>(serializedFilePath);
                    if (post.post != "DELETED" && post.post != "" && post.post != null)
                    {
                        allPosts += user.posts[i].id + ",";
                    }
                }
            }
            this.postsIdInput.Text = allPosts;
            string allComments = "";
            if (user.comments != null)
            {
                for (int i = 0; i < user.comments.Length; i++)
                {
                    RemoteService.RemoteServiceCommand("GetCommentById$" + user.comments[i].id);
                    Comment comment = Deserialize<Comment>(serializedFilePath);
                    if (comment.comment != "DELETED" && comment.comment != "" && comment.comment != null)
                    {
                        allComments += user.comments[i].id + ",";
                    }
                }
            }
            this.commentsIdInput.Text = allComments;

            if (MainWindow.currentUser.isModerator == 1 && user.isModerator == 0)
            {
                Button makeUserModerBtn = new Button("Make moderator")
                {
                    X = 24,
                    Y = 10,
                };
                makeUserModerBtn.Clicked += MakeUserModer;
                this.Add(makeUserModerBtn);
            }
        }
        private void MakeUserModer()
        {
            SqliteConnection connection = new SqliteConnection($"Data Source ={MainWindow.filePath}");
            connection.Open();

            this.user.isModerator = 1;
            Serialize<User>(user, serializedFilePath);
            RemoteService.RemoteServiceCommand("UpdateUserLogin$" + user.id);
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