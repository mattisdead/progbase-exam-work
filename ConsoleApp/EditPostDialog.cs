using Terminal.Gui;
using Entities;
using ServiceLib;

namespace ConsoleApp
{
    public class EditPostDialog : CreatePostDialog
    {
        public EditPostDialog()
        {
            this.Title = "Edit post";
        }
        public void SetPost(Post post)
        {
            this.postInput.Text = post.post;
        }
    }
}