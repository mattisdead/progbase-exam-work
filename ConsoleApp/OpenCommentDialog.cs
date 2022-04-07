using Terminal.Gui;
using ServiceLib;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    public class OpenCommentDialog : Dialog
    {
        protected Comment comment;
        public bool edited;
        public bool deleted;
        private TextField commentInput;
        private TextField idInput;
        private TextField postIdInput;
        private TextField dateInput;
        private TextField userIdInput;
        private Label isPinnedInput;
        public OpenCommentDialog(Comment comment)
        {
            this.comment = comment;
            this.Title = "Open comment";

            Button cancelBtn = new Button("Back");
            cancelBtn.Clicked += OnCreateDialogCancelled;

            this.AddButton(cancelBtn);

            Label idLbl = new Label(2, 4, "Comment id:");
            idInput = new TextField("")
            {
                X = Pos.Right(idLbl) + 2,
                Y = Pos.Top(idLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(idLbl, idInput);

            Label commentLbl = new Label(2, 6, "Comment:");
            commentInput = new TextField("")
            {
                X = Pos.Right(idLbl) + 2,
                Y = Pos.Top(commentLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(commentLbl, commentInput);

            Label postIdLbl = new Label(2, 8, "Post id:");
            postIdInput = new TextField("")
            {
                X = Pos.Right(idLbl) + 2,
                Y = Pos.Top(postIdLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(postIdLbl, postIdInput);

            Label dateLbl = new Label(2, 10, "Date:");
            dateInput = new TextField("")
            {
                X = Pos.Right(idLbl) + 2,
                Y = Pos.Top(dateLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(dateLbl, dateInput);

            Label userLbl = new Label(2, 12, "User id:");
            userIdInput = new TextField("")
            {
                X = Pos.Right(idLbl) + 2,
                Y = Pos.Top(userLbl),
                Width = Dim.Fill() - 2,
                ReadOnly = true,
            };
            this.Add(userLbl, userIdInput);

            Label isPinnedLbl = new Label(2, 14, "Pinned");
            isPinnedInput = new Label("")
            {
                X = Pos.Right(idLbl) + 2,
                Y = Pos.Top(isPinnedLbl),
                Width = Dim.Fill() - 2
            };
            this.Add(isPinnedLbl, isPinnedInput);

            if (comment.post != null && comment.post.author != null && MainWindow.currentUser.id == comment.post.author.id)
            {
                if (comment.post.idOfPinnedComment == comment.id)
                {
                    Button pinBtn = new Button("Unpin")
                    {
                        X = Pos.Right(isPinnedLbl) + 14,
                        Y = Pos.Top(isPinnedInput),
                    };
                    pinBtn.Clicked += Unpinned;
                    this.Add(pinBtn);
                }
                else
                {
                    Button pinBtn = new Button("Pin")
                    {
                        X = Pos.Right(isPinnedLbl) + 14,
                        Y = Pos.Top(isPinnedInput),
                    };
                    pinBtn.Clicked += Pinned;
                    this.Add(pinBtn);
                }

            }

            Button editCommentBtn = new Button(2, 16, "Edit");
            editCommentBtn.Clicked += OnEditCommentButtonClicked;
            this.Add(editCommentBtn);

            Button deleteCommentBtn = new Button("Delete")
            {
                X = Pos.Right(editCommentBtn) + 2,
                Y = Pos.Top(editCommentBtn),
            };
            deleteCommentBtn.Clicked += OnDeleteCommentButtonClicked;
            this.Add(deleteCommentBtn);

        }
        private void OnDeleteCommentButtonClicked()
        {
            int index = MessageBox.Query("Delete comment", "Are you sure you want to delete this comment?", "No", "Yes");
            if (index == 1)
            {
                deleted = true;
                Application.RequestStop();
            }
        }
        private void OnEditCommentButtonClicked()
        {
            EditCommentDialog dialog = new EditCommentDialog();
            dialog.SetComment(this.comment);

            Application.Run(dialog);

            if (!dialog._canceled)
            {
                Comment updatedComment = dialog.GetComment();
                updatedComment.id = this.comment.id;
                updatedComment.post = this.comment.post;
                updatedComment.author = this.comment.author;
                updatedComment.date = this.comment.date;

                this.edited = true;
                this.SetComment(updatedComment);
            }
        }
        private void OnCreateDialogCancelled()
        {
            Application.RequestStop();
        }
        public void SetComment(Comment comment)
        {
            this.comment = comment;
            this.commentInput.Text = comment.comment;
            this.idInput.Text = comment.id.ToString();
            this.postIdInput.Text = comment.post.id.ToString();
            this.dateInput.Text = comment.date.ToShortDateString();
            this.userIdInput.Text = comment.author.id.ToString();
            if (comment.post.idOfPinnedComment == comment.id)
            {
                this.isPinnedInput.Text = "true";
            }
            else
            {
                this.isPinnedInput.Text = "false";
            }
        }
        private void Pinned()
        {
            this.comment.post.idOfPinnedComment = this.comment.id;
            Serialize<Post>(comment.post, MainWindow.serializedFilePath);
            RemoteService.RemoteServiceCommand("UpdatePost$" + comment.post.id);
        }
        private void Unpinned()
        {
            this.comment.post.idOfPinnedComment = 0;
            Serialize<Post>(comment.post, MainWindow.serializedFilePath);
            RemoteService.RemoteServiceCommand("UpdatePost$" + comment.post.id);
        }
        public Comment GetComment()
        {
            return this.comment;
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
    }
}