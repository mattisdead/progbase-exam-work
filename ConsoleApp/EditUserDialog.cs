using Terminal.Gui;
using Entities;
using ServiceLib;

namespace ConsoleApp
{
    public class EditUserDialog : CreateUserDialog
    {
        public EditUserDialog()
        {
            this.Title = "Edit user";
        }
        public void SetUser(User user)
        {
            this.userLoginInput.Text = user.login;
            this.userPasswordInput.Text = user.password;
        }
    }
}