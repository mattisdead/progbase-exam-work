using Terminal.Gui;
using Entities;
using ServiceLib;

namespace ConsoleApp
{
    public class EditCommentDialog : CreateCommentDialog
    {
        public EditCommentDialog()
        {
            this.Title = "Edit comment";
        }
        public void SetComment(Comment comment)
        {
            this.commentInput.Text = comment.comment;
        }
    }
}