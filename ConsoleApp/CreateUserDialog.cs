using Terminal.Gui;
using Entities;
using ServiceLib;

namespace ConsoleApp
{
    public class CreateUserDialog : Dialog
    {
        protected TextField userLoginInput;
        protected TextField userPasswordInput;
        public bool _canceled;
        public CreateUserDialog()
        {
            this.Title = "Create user";
            Button okBtn = new Button("Ok");
            okBtn.Clicked += OnCreateDialogSubmitted;

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnCreateDialogCancelled;

            this.AddButton(cancelBtn);
            this.AddButton(okBtn);

            Label userLoginLbl = new Label(2, 4, "Login:");
            userLoginInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(userLoginLbl),
                Width = Dim.Fill() - 2,
            };
            this.Add(userLoginLbl, userLoginInput);

            Label userPasswordLbl = new Label(2, 6, "Password:");
            userPasswordInput = new TextField("")
            {
                X = 20,
                Y = Pos.Top(userPasswordLbl),
                Width = Dim.Fill() - 2,
            };
            this.Add(userPasswordLbl, userPasswordInput);
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
        public User GetUser()
        {
            User user = new User()
            {
                login = userLoginInput.Text.ToString(),
                password = userPasswordInput.Text.ToString(),
            };
            return user;
        }
    }
}