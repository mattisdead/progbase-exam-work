using Terminal.Gui;
using Entities;
using ServiceLib;

namespace ConsoleApp
{
    public class CreateCommentDialog : Dialog
    {
        protected TextField commentInput;
        public bool _canceled;
        public CreateCommentDialog()
        {
            this.Title = "Create comment";
            Button okBtn = new Button("Ok");
            okBtn.Clicked += OnCreateDialogSubmitted;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCancelled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label commentLbl = new Label(2, 4, "Comment:");
            commentInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(commentLbl),
                Width = Dim.Fill() - 2,
            };
            this.Add(commentLbl, commentInput);
        }
        private void OnCreateDialogCancelled()
        {
            this._canceled = true;
            Application.RequestStop();
        }
        private void OnCreateDialogSubmitted()
        {
            this._canceled = false;
            Application.RequestStop();
        }
        public Comment GetComment()
        {
            Comment comment = new Comment()
            {
                comment = commentInput.Text.ToString(),
            };
            return comment;
        }
    }
}