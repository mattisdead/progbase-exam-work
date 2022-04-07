namespace ServiceLib
{
    public class User
    {
        public int id;
        public string login;
        public string password;
        public int isModerator;
        public Post[] posts;
        public Comment[] comments;
        public User(string login, string password, int isModerator, Post[] posts, Comment[] comments)
        {
            this.login = login;
            this.password = password;
            this.isModerator = isModerator;
            this.posts = posts;
            this.comments = comments;
        }
        public User(int id, string login, string password, int isModerator, Post[] posts, Comment[] comments)
        {
            this.id = id;
            this.login = login;
            this.password = password;
            this.isModerator = isModerator;
            this.posts = posts;
            this.comments = comments;
        }
        public User(string login, string password)
        {
            this.login = login;
            this.password = password;
            this.isModerator = 0;
            this.posts = null;
            this.comments = null;
        }
        public User()
        {
            this.login = "";
            this.password = "";
            this.isModerator = 0;
        }
        public User(int id)
        {
            this.id = id;
            this.login = "";
            this.password = "";
            this.isModerator = 0;
        }
        public override string ToString()
        {
            return $"[{id}] {login}";
        }
    }
}