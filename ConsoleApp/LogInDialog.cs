using Terminal.Gui;
using Entities;

namespace ConsoleApp
{
    public class LogInDialog : Dialog
    {
        public bool canceled;
        public TextField loginInput;
        public TextField passwordInput;
        bool loggedIn;
        public LogInDialog()
        {
            this.Title = "Log in";

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnLogInDialogCancelled;

            Button logInBtn = new Button("Log in");
            logInBtn.Clicked += OnLogInDialogSubmitted;

            this.AddButton(cancelBtn);
            this.AddButton(logInBtn);

            Label passwordLbl = new Label(2, 6, "Password:");
            passwordInput = new TextField("")
            {
                X = Pos.Right(passwordLbl) + 2,
                Y = Pos.Top(passwordLbl),
                Width = Dim.Fill() - 2,
                Secret = true
            };
            this.Add(passwordLbl, passwordInput);

            Label loginLbl = new Label(2, 4, "Login:");
            loginInput = new TextField("")
            {
                X = Pos.Right(passwordLbl) + 2,
                Y = Pos.Top(loginLbl),
                Width = Dim.Fill() - 2,
            };
            this.Add(loginLbl, loginInput);
        }
        private void OnLogInDialogCancelled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnLogInDialogSubmitted()
        {
            this.canceled = false;
            Application.RequestStop();
        }
    }
}