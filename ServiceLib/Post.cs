using System;

namespace ServiceLib
{
    public class Post
    {
        public int id;
        public string post;
        public int idOfPinnedComment;
        public User author;
        public Comment[] comments;
        public System.DateTime date;
        public Post(string post, int idOfPinnedComment, User author, Comment[] comments, System.DateTime date)
        {
            this.post = post;
            this.idOfPinnedComment = idOfPinnedComment;
            this.author = author;
            this.comments = comments;
            this.date = date;
        }
        public Post(int id, string post, int idOfPinnedComment, User author, Comment[] comments, System.DateTime date)
        {
            this.id = id;
            this.post = post;
            this.idOfPinnedComment = idOfPinnedComment;
            this.author = author;
            this.comments = comments;
            this.date = date;
        }
        public Post(int id)
        {
            this.id = id;
            this.post = "";
            this.idOfPinnedComment = 0;
        }
        public Post(string post)
        {
            this.post = post;
            this.idOfPinnedComment = 0;
        }
        public Post()
        {
            this.id = 0;
            this.post = "";
            this.idOfPinnedComment = 0;
        }
        public override string ToString()
        {
            return $"[{id}] {post}";
        }
    }
}