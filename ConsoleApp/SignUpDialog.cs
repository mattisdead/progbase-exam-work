using Terminal.Gui;

namespace ConsoleApp
{
    public class SignUpDialog : Dialog
    {
        public bool canceled;
        public TextField loginInput;
        public TextField passwordInput;
        bool loggedIn;
        public SignUpDialog()
        {
            this.Title = "Sign up";

            Button cancelBtn = new Button("Cancel");
            cancelBtn.Clicked += OnSignUpDialogCancelled;

            Button logInBtn = new Button("Log in");
            logInBtn.Clicked += OnSignUpDialogSubmitted;

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
        private void OnSignUpDialogCancelled()
        {
            this.canceled = true;
            Application.RequestStop();
        }
        private void OnSignUpDialogSubmitted()
        {
            this.canceled = false;
            Application.RequestStop();
        }
    }
}