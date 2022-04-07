using System;
using Entities;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System.IO;
using ServiceLib;

namespace DataGenerator
{
    public class DataGenerator
    {
        public static UserRepo userRepo;
        public static CommentRepo commentRepo;
        public static PostRepo postRepo;
        public static void Main(string[] args)
        {
            string databaseFile = "../data/data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source ={databaseFile}");
            connection.Open();
            userRepo = new UserRepo(connection, databaseFile);
            commentRepo = new CommentRepo(connection, databaseFile);
            postRepo = new PostRepo(connection, databaseFile);

            int times = 0;

            string entitity = args[0];
            if (!int.TryParse(args[1], out int number))
            {
                return;
            }

            if (entitity == null || number == 0)
            {
                return;
            }
            if (entitity.Equals("users", StringComparison.InvariantCultureIgnoreCase))
            {
                if (postRepo.GetAll().Count == 0 && commentRepo.GetAll().Count == 0)
                {
                    for (int i = 0; i < number; i++)
                    {
                        User user = GetRandomUser();
                        try
                        {
                            userRepo.Insert(user);
                        }
                        catch
                        {
                            i--;
                        }
                    }
                }
                else if (postRepo.GetAll().Count != 0 && commentRepo.GetAll().Count == 0)
                {
                    List<Post> posts = postRepo.GetAll();
                    int[] usersID = new int[posts.Count];
                    int border = 0;
                    if (number > posts.Count) border = posts.Count;
                    else border = number;
                    for (int i = 0; i < border; i++)
                    {
                        if (userRepo.GetById(usersID[i]).login == null)
                        {
                            User user = GetRandomUser();
                            user.id = usersID[i];
                            user.posts = new Post[] { posts[i] };
                            userRepo.InsertWithId(user);
                            times++;
                        }
                        else
                        {
                            User user = userRepo.GetById(usersID[i]);
                            if (user.posts != null)
                            {
                                Array.Resize(ref user.posts, user.posts.Length + 1);
                                user.posts[user.posts.Length - 1] = posts[i];
                            }
                            userRepo.Update(user.id, user);
                        }
                        if (times >= number) return;
                    }
                }
                else if (postRepo.GetAll().Count == 0 && commentRepo.GetAll().Count != 0)
                {
                    List<Comment> comments = commentRepo.GetAll();
                    int[] usersID = new int[comments.Count];
                    int border = 0;
                    if (number > comments.Count) border = comments.Count;
                    else border = number;
                    for (int i = 0; i < comments.Count; i++)
                    {
                        if (userRepo.GetById(usersID[i]).login == null)
                        {
                            User user = GetRandomUser();
                            user.id = usersID[i];
                            user.comments = new Comment[] { comments[i] };
                            userRepo.InsertWithId(user);
                            times++;
                        }
                        else
                        {
                            User user = userRepo.GetById(usersID[i]);
                            if (user.comments != null)
                            {
                                Array.Resize(ref user.comments, user.comments.Length + 1);
                                user.comments[user.comments.Length - 1] = comments[i];
                            }
                            userRepo.Update(user.id, user);
                        }
                        if (times >= number) return;
                    }
                }
                else if (postRepo.GetAll().Count != 0 && commentRepo.GetAll().Count != 0)
                {
                    List<Post> posts = postRepo.GetAll();
                    List<Comment> comments = commentRepo.GetAll();
                    int[] usersID = new int[posts.Count + comments.Count];
                    for (int i = 0; i < posts.Count; i++)
                    {
                        usersID[i] = posts[i].author.id;
                    }
                    for (int i = 0; i < comments.Count; i++)
                    {
                        usersID[i + posts.Count] = comments[i].author.id;
                    }
                    int border = 0;
                    if (number > posts.Count) border = posts.Count;
                    else border = number;
                    for (int i = 0; i < border; i++)
                    {
                        if (userRepo.GetById(usersID[i]).login == null)
                        {
                            User user = GetRandomUser();
                            user.id = usersID[i];
                            user.posts = new Post[] { posts[i] };
                            userRepo.InsertWithId(user);
                            times++;
                        }
                        else
                        {
                            User user = userRepo.GetById(usersID[i]);
                            if (user.posts != null)
                            {
                                Array.Resize(ref user.posts, user.posts.Length + 1);
                                user.posts[user.posts.Length - 1] = posts[i];
                            }
                            userRepo.Update(user.id, user);
                        }
                        if (times >= number) return;
                    }
                    if (Math.Abs(number - posts.Count) > comments.Count) border = comments.Count;
                    else border = Math.Abs(number - posts.Count);
                    for (int i = 0; i < comments.Count; i++)
                    {
                        if (userRepo.GetById(usersID[i + posts.Count]).login == null)
                        {
                            User user = GetRandomUser();
                            user.id = usersID[i + posts.Count];
                            user.comments = new Comment[] { comments[i] };
                            userRepo.InsertWithId(user);
                            times++;
                        }
                        else
                        {
                            User user = userRepo.GetById(usersID[i + posts.Count]);
                            if (user.comments != null)
                            {
                                Array.Resize(ref user.comments, user.comments.Length + 1);
                                user.comments[user.comments.Length - 1] = comments[i];
                            }
                            userRepo.Update(user.id, user);
                        }
                        if (times >= number) return;
                    }
                }
                if (times < number)
                {
                    for (int i = times; i < number; i++)
                    {
                        User user = GetRandomUser();
                        try
                        {
                            userRepo.Insert(user);
                        }
                        catch
                        {
                            i--;
                        }
                    }
                }
            }
            else if (entitity.Equals("comments", StringComparison.InvariantCultureIgnoreCase))
            {
                if (postRepo.GetAll().Count == 0 && userRepo.GetAll().Count == 0)
                {
                    for (int i = 0; i < number; i++)
                    {
                        Comment comment = GetRandomComment();
                        try
                        {
                            commentRepo.InsertWithId(comment);
                        }
                        catch
                        {
                            i--;
                        }
                    }
                }
                else if (postRepo.GetAll().Count != 0 && userRepo.GetAll().Count == 0)
                {
                    List<Post> posts = postRepo.GetAll();
                    string strCommentsID = "";
                    int border = 0;
                    if (number > posts.Count) border = posts.Count;
                    else border = number;
                    for (int i = 0; i < posts.Count; i++)
                    {
                        if (posts[i].comments != null)
                        {
                            for (int j = 0; j < posts[i].comments.Length; j++)
                            {
                                strCommentsID += posts[i].comments[j].id + ",";
                            }
                            string[] arrCommentsID = strCommentsID.Split(',');
                            int[] commentsId = new int[arrCommentsID.Length - 1];
                            for (int j = 0; j < commentsId.Length; j++)
                            {
                                int.TryParse(arrCommentsID[j], out commentsId[j]);
                            }
                            for (int j = 0; j < commentsId.Length; j++)
                            {
                                if (commentRepo.GetById(commentsId[j]).comment == null)
                                {
                                    Comment comment = GetRandomComment();
                                    comment.id = commentsId[j];
                                    comment.post = posts[i];
                                    commentRepo.InsertWithId(comment);
                                    times++;
                                }
                            }
                        }
                        if (times >= number) return;
                    }
                }
                else if (postRepo.GetAll().Count == 0 && userRepo.GetAll().Count != 0)
                {
                    List<User> users = userRepo.GetAll();
                    string strCommentsID = "";
                    int border = 0;
                    if (number > users.Count) border = users.Count;
                    else border = number;
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].comments != null)
                        {
                            for (int j = 0; j < users[i].comments.Length; j++)
                            {
                                strCommentsID += users[i].comments[j].id + ",";
                            }
                            string[] arrCommentsID = strCommentsID.Split(',');
                            int[] commentsId = new int[arrCommentsID.Length - 1];
                            for (int j = 0; j < commentsId.Length; j++)
                            {
                                commentsId[j] = int.Parse(arrCommentsID[j]);
                            }
                            for (int j = 0; j < commentsId.Length; j++)
                            {
                                if (commentRepo.GetById(commentsId[j]).comment == null)
                                {
                                    Comment comment = GetRandomComment();
                                    comment.id = commentsId[j];
                                    comment.author = users[i];
                                    commentRepo.InsertWithId(comment);
                                    times++;
                                }
                            }
                        }
                        if (times >= number) return;
                    }
                }
                else if (postRepo.GetAll().Count != 0 && userRepo.GetAll().Count != 0)
                {
                    List<Post> posts = postRepo.GetAll();
                    string strCommentsID = "";
                    int border = 0;
                    if (number > posts.Count) border = posts.Count;
                    else border = number;
                    for (int i = 0; i < posts.Count; i++)
                    {
                        if (posts[i].comments != null)
                        {
                            for (int j = 0; j < posts[i].comments.Length; j++)
                            {
                                strCommentsID += posts[i].comments[j].id + ",";
                            }
                            string[] arrCommentsID = strCommentsID.Split(',');
                            int[] commentsId = new int[arrCommentsID.Length - 1];
                            for (int j = 0; j < commentsId.Length; j++)
                            {
                                int.TryParse(arrCommentsID[j], out commentsId[j]);
                            }
                            for (int j = 0; j < commentsId.Length; j++)
                            {
                                if (commentRepo.GetById(commentsId[j]).comment == null)
                                {
                                    Comment comment = GetRandomComment();
                                    comment.id = commentsId[j];
                                    comment.post = posts[i];
                                    commentRepo.InsertWithId(comment);
                                    times++;
                                }
                            }
                        }
                        if (times >= number) return;
                    }
                    List<User> users = userRepo.GetAll();
                    strCommentsID = "";
                    if (Math.Abs(number - posts.Count) > users.Count) border = users.Count;
                    else border = Math.Abs(number - posts.Count);
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].comments != null)
                        {
                            for (int j = 0; j < users[i].comments.Length; j++)
                            {
                                strCommentsID += users[i].comments[j].id + ",";
                            }
                            string[] arrCommentsID = strCommentsID.Split(',');
                            int[] commentsId = new int[arrCommentsID.Length - 1];
                            for (int j = 0; j < commentsId.Length; j++)
                            {
                                commentsId[j] = int.Parse(arrCommentsID[j]);
                            }
                            for (int j = 0; j < commentsId.Length; j++)
                            {
                                if (commentRepo.GetById(commentsId[j]).comment == null)
                                {
                                    Comment comment = GetRandomComment();
                                    comment.id = commentsId[j];
                                    comment.author = users[i];
                                    commentRepo.InsertWithId(comment);
                                    times++;
                                }
                            }
                        }
                        if (times >= number) return;
                    }
                }
                if (times < number)
                {
                    for (int i = times; i < number; i++)
                    {
                        Comment comment = GetRandomComment();
                        try
                        {
                            commentRepo.InsertWithId(comment);
                        }
                        catch
                        {
                            i--;
                        }
                    }
                }
            }
            else if (entitity.Equals("posts", StringComparison.InvariantCultureIgnoreCase))
            {
                if (commentRepo.GetAll().Count == 0 && userRepo.GetAll().Count == 0)
                {
                    for (int i = 0; i < number; i++)
                    {
                        Post post = GetRandomPost();
                        try
                        {
                            postRepo.Insert(post);
                        }
                        catch
                        {
                            i--;
                        }
                    }
                }
                else if (commentRepo.GetAll().Count != 0 && userRepo.GetAll().Count == 0)
                {
                    List<Comment> comments = commentRepo.GetAll();
                    int[] postsId = new int[comments.Count];
                    int border = 0;
                    if (number > comments.Count) border = comments.Count;
                    else border = number;
                    for (int i = 0; i < comments.Count; i++)
                    {
                        if (postRepo.GetById(postsId[i]).post == null)
                        {
                            Post post = GetRandomPost();
                            post.id = postsId[i];
                            post.comments = new Comment[] { comments[i] };
                            postRepo.InsertWithId(post);
                            times++;
                        }
                        else
                        {
                            Post post = postRepo.GetById(postsId[i]);
                            if (post.comments != null)
                            {
                                Array.Resize(ref post.comments, post.comments.Length + 1);
                                post.comments[post.comments.Length - 1] = comments[i];
                            }
                            postRepo.Update(post.id, post);
                        }
                        if (times >= number) return;
                    }
                }
                else if (commentRepo.GetAll().Count == 0 && userRepo.GetAll().Count != 0)
                {
                    List<User> users = userRepo.GetAll();
                    string strPostsID = "";
                    int border = 0;
                    if (number > users.Count) border = users.Count;
                    else border = number;
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].posts != null)
                        {
                            for (int j = 0; j < users[i].posts.Length; j++)
                            {
                                strPostsID += users[i].posts[j].id + ",";
                            }
                            string[] arrPostsID = strPostsID.Split(',');
                            int[] postsId = new int[arrPostsID.Length - 1];
                            for (int j = 0; j < postsId.Length; j++)
                            {
                                postsId[j] = int.Parse(arrPostsID[j]);
                            }
                            for (int j = 0; j < postsId.Length; j++)
                            {
                                if (postRepo.GetById(postsId[j]).post == null)
                                {
                                    Post post = GetRandomPost();
                                    post.id = postsId[j];
                                    post.author = users[i];
                                    postRepo.InsertWithId(post);
                                    times++;
                                }
                                if (times >= number) return;
                            }
                        }
                        if (times >= number) return;
                    }

                }
                else if (commentRepo.GetAll().Count != 0 && userRepo.GetAll().Count != 0)
                {
                    List<Comment> comments = commentRepo.GetAll();
                    int[] postsId = new int[comments.Count];
                    int border = 0;
                    if (number > comments.Count) border = comments.Count;
                    else border = number;
                    for (int i = 0; i < comments.Count; i++)
                    {
                        if (postRepo.GetById(postsId[i]).post == null)
                        {
                            Post post = GetRandomPost();
                            post.id = postsId[i];
                            post.comments = new Comment[] { comments[i] };
                            postRepo.InsertWithId(post);
                            times++;
                        }
                        else
                        {
                            Post post = postRepo.GetById(postsId[i]);
                            if (post.comments != null)
                            {
                                Array.Resize(ref post.comments, post.comments.Length + 1);
                                post.comments[post.comments.Length - 1] = comments[i];
                            }
                            postRepo.Update(post.id, post);
                        }
                        if (times >= number) return;
                    }

                    List<User> users = userRepo.GetAll();
                    string strPostsID = "";
                    if (Math.Abs(number - comments.Count) > comments.Count) border = users.Count;
                    else border = Math.Abs(number - comments.Count);
                    for (int i = 0; i < users.Count; i++)
                    {
                        if (users[i].posts != null)
                        {
                            for (int j = 0; j < users[i].posts.Length; j++)
                            {
                                strPostsID += users[i].posts[j].id + ",";
                            }
                            string[] arrPostsID = strPostsID.Split(',');
                            postsId = new int[arrPostsID.Length - 1];
                            for (int j = 0; j < postsId.Length; j++)
                            {
                                postsId[j] = int.Parse(arrPostsID[j]);
                            }
                            for (int j = 0; j < postsId.Length; j++)
                            {
                                if (postRepo.GetById(postsId[j]).post == null)
                                {
                                    Post post = GetRandomPost();
                                    post.id = postsId[j];
                                    post.author = users[i];
                                    postRepo.InsertWithId(post);
                                    times++;
                                }
                                if (times >= number) return;
                            }
                        }
                        if (times >= number) return;
                    }
                }
                if (times < number)
                {
                    for (int i = times; i < number; i++)
                    {
                        Post post = GetRandomPost();
                        try
                        {
                            postRepo.Insert(post);
                        }
                        catch
                        {
                            i--;
                        }
                    }
                }
            }
        }
        public static User GetRandomUser()
        {
            string usersText = File.ReadAllText("../data/generated/users.csv");
            string[] logins = usersText.Split("\r\n");

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            string password = "";
            for (int j = 0; j < 8; j++)
            {
                password += chars[random.Next(chars.Length)];
            }
            SHA256 sha256Hash = SHA256.Create();
            string passwordHash = GetHash(sha256Hash, password);

            string login = logins[random.Next(20)];

            int border = random.Next(7);
            Post[] posts = new Post[border];
            for (int i = 0; i < border; i++)
            {
                posts[i] = new Post(random.Next(1000));
            }

            border = random.Next(7);
            Comment[] comments = new Comment[border];
            for (int i = 0; i < border; i++)
            {
                comments[i] = new Comment(random.Next(1000));
            }

            User user = new User(login + random.Next(1000), passwordHash, random.Next(2), posts, comments);
            return user;
        }
        public static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public static Comment GetRandomComment()
        {
            string commentsText = File.ReadAllText("../data/generated/comments.csv");
            string[] comments = commentsText.Split("\r\n");
            Random random = new Random();
            Comment comment = new Comment(comments[random.Next(20)], new User(random.Next(10000)), new Post(random.Next(1000)), DateTime.Now);
            return comment;
        }
        public static Post GetRandomPost()
        {
            string postsText = File.ReadAllText("../data/generated/posts.csv");
            string[] posts = postsText.Split("\r\n");
            Random random = new Random();

            Comment[] comments = new Comment[random.Next(10)];
            for (int i = 0; i < comments.Length; i++)
            {
                comments[i] = new Comment(random.Next(10000));
            }
            int idOfPinnedComment = 0;
            if (comments.Length > 0 && random.Next(2) == 0)
            {
                idOfPinnedComment = comments[random.Next(comments.Length)].id;
            }
            Post post = new Post(posts[random.Next(20)], idOfPinnedComment, new User(random.Next(10000)), comments, DateTime.Now);
            return post;
        }

    }
}
