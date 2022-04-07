namespace ServiceLib
{
    public class Comment
    {
        public int id;
        public string comment;
        public User author;
        public Post post;
        public System.DateTime date;
        public Comment(string comment, User author, Post post, System.DateTime date)
        {
            this.comment = comment;
            this.author = author;
            this.post = post;
            this.date = date;
        }
        public Comment(int id, string comment, User author, Post post, System.DateTime date)
        {
            this.id = id;
            this.comment = comment;
            this.author = author;
            this.post = post;
            this.date = date;
        }
        public Comment()
        {
            this.comment = "";
            this.author = null;
            this.post = null;
        }
        public Comment(int id)
        {
            this.id = id;
            this.comment = "";
            this.author = null;
            this.post = null;
        }
        public override string ToString()
        {
            return $"[{id}] {comment}";
        }
    }
}