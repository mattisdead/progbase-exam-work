using Terminal.Gui;
using Entities;
using ServiceLib;

namespace ConsoleApp
{
    public class CreatePostDialog : Dialog
    {
        protected TextField postInput;
        public bool _canceled;
        public CreatePostDialog()
        {
            this.Title = "Create post";
            Button okBtn = new Button("Ok");
            okBtn.Clicked += OnCreateDialogSubmitted;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCancelled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label postLbl = new Label(2, 4, "Post:");
            postInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(postLbl),
                Width = Dim.Fill() - 2,
            };
            this.Add(postLbl, postInput);
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
        public Post GetPost()
        {
            Post post = new Post()
            {
                post = postInput.Text.ToString(),
            };
            return post;
        }

    }
}