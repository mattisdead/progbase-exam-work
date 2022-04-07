using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using System;
using System.Security.Cryptography;
using System.Text;
using ServiceLib;

namespace Entities
{
    public class UserRepo
    {
        private SqliteConnection connection;
        private static string filePath;
        public UserRepo(SqliteConnection connection, string file)
        {
            filePath = file;
            this.connection = connection;
        }
        public static User GetUser(SqliteDataReader reader)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source ={filePath}");
            connection.Open();
            PostRepo postRepo = new PostRepo(connection, filePath);
            CommentRepo commentRepo = new CommentRepo(connection, filePath);

            User user = new User();

            user.id = int.Parse(reader.GetString(0));
            user.login = reader.GetString(1);

            user.password = reader.GetString(2);
            user.isModerator = int.Parse(reader.GetString(3));

            string strPostsId = reader.GetString(4);
            if (strPostsId.Length != 0)
            {
                string[] arrPostsId = strPostsId.Split(',');
                Post[] posts = new Post[arrPostsId.Length - 1];
                for (int i = 0; i < posts.Length; i++)
                {
                    int.TryParse(arrPostsId[i], out int postId);
                    posts[i] = new Post()
                    {
                        id = postId,
                        post = "DELETED",
                    };
                }
                user.posts = posts;
            }

            string strCommentsId = reader.GetString(5);
            if (strCommentsId.Length != 0)
            {
                string[] arrCommentsId = strCommentsId.Split(',');
                Comment[] comments = new Comment[arrCommentsId.Length - 1];
                for (int i = 0; i < comments.Length; i++)
                {
                    int.TryParse(arrCommentsId[i], out int commentId);
                    comments[i] = new Comment()
                    {
                        id = commentId,
                        comment = "DELETED",
                    };
                }
                user.comments = comments;
            }

            return user;
        }
        public int DeleteById(int id)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int nChanged = command.ExecuteNonQuery();
            return nChanged;
        }
        public User GetById(int id)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();

            if (reader.Read())
            {
                user.id = int.Parse(reader.GetString(0));
                user.login = reader.GetString(1);
                user.password = reader.GetString(2);
                user.isModerator = int.Parse(reader.GetString(3));

                string strPostsId = reader.GetString(4);
                if (strPostsId.Length != 0)
                {
                    string[] arrPostsId = strPostsId.Split(',');
                    Post[] posts = new Post[arrPostsId.Length - 1];
                    for (int i = 0; i < posts.Length; i++)
                    {
                        int.TryParse(arrPostsId[i], out int postId);
                        posts[i] = new Post()
                        {
                            id = postId,
                            post = "DELETED",
                        };
                    }
                    user.posts = posts;
                }

                string strCommentsId = reader.GetString(5);
                string[] arrCommentsId = strCommentsId.Split(',');
                Comment[] comments = new Comment[arrCommentsId.Length - 1];
                for (int i = 0; i < comments.Length; i++)
                {
                    int.TryParse(arrCommentsId[i], out int commentId);
                    comments[i] = new Comment()
                    {
                        id = commentId,
                        comment = "DELETED",
                    };
                }
                user.comments = comments;
            }
            else
            {
                user.login = null;
                user.password = null;
                user.isModerator = 0;
                user.posts = null;
                user.comments = null;
            }
            reader.Close();
            return user;
        }
        private bool FindByLogin(string login)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE login = $login";
            command.Parameters.AddWithValue("$login", login);

            SqliteDataReader reader = command.ExecuteReader();

            if (reader.Read())
            {
                return true;
            }
            return false;
        }
        public int Insert(User user)
        {
            SqliteCommand command = connection.CreateCommand();
            Random rand = new Random();
            command.CommandText =
            @"INSERT INTO users (login, password, isModerator, posts_id, comments_id) VALUES ($login, $password, $isModerator, $posts_id, $comments_id);SELECT last_insert_rowid();";

            command.Parameters.AddWithValue("$login", user.login);

            SHA256 sha256Hash = SHA256.Create();
            string passwordHash = GetHash(sha256Hash, user.password);
            command.Parameters.AddWithValue("$password", passwordHash);
            command.Parameters.AddWithValue("$isModerator", user.isModerator);

            string postsId = "";
            if (user.posts != null)
            {
                for (int i = 0; i < user.posts.Length; i++)
                {
                    postsId += user.posts[i].id + ",";
                }
            }
            command.Parameters.AddWithValue("$posts_id", postsId);

            string commentsId = "";
            if (user.comments != null)
            {
                for (int i = 0; i < user.comments.Length; i++)
                {
                    commentsId += user.comments[i].id + ",";
                }
            }
            command.Parameters.AddWithValue("$comments_id", commentsId);

            return (int)(long)command.ExecuteScalar();
        }
        public int InsertWithId(User user)
        {
            SqliteCommand command = connection.CreateCommand();
            Random rand = new Random();
            command.CommandText =
            @"INSERT INTO users (id, login, password, isModerator, posts_id, comments_id) VALUES ($id, $login, $password, $isModerator, $posts_id, $comments_id);SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$id", user.id);
            command.Parameters.AddWithValue("$login", user.login);

            SHA256 sha256Hash = SHA256.Create();
            string passwordHash = GetHash(sha256Hash, user.password);
            command.Parameters.AddWithValue("$password", passwordHash);

            command.Parameters.AddWithValue("$isModerator", rand.Next(0, 2));

            string postsId = "";
            for (int i = 0; i < user.posts.Length; i++)
            {
                postsId += user.posts[i].id + ",";
            }
            command.Parameters.AddWithValue("$posts_id", postsId);

            string commentsId = "";
            for (int i = 0; i < user.comments.Length; i++)
            {
                commentsId += user.comments[i].id + ",";
            }
            command.Parameters.AddWithValue("$comments_id", commentsId);

            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public Post[] GetAllPosts(int id)
        {
            User user = this.GetById(id);
            for (int i = 0; i < user.posts.Length; i++)
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM posts WHERE id = $id";
                command.Parameters.AddWithValue("$id", user.posts[i].id);

                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user.posts[i].post = reader.GetString(1);
                }
            }
            return user.posts;
        }
        public Comment[] GetAllComments(int id)
        {
            User user = this.GetById(id);
            for (int i = 0; i < user.comments.Length; i++)
            {
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT * FROM comments WHERE id = $id";
                command.Parameters.AddWithValue("$id", user.comments[i].id);

                SqliteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    user.comments[i].comment = reader.GetString(1);
                }
            }
            return user.comments;
        }
        public List<User> GetPage(int pageNumber, int pageLenght)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users LIMIT $numberOfRows OFFSET $pageNumber";
            command.Parameters.AddWithValue("$numberOfRows", pageLenght);
            command.Parameters.AddWithValue("$pageNumber", (pageNumber - 1) * pageLenght);

            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = GetUser(reader);
                users.Add(user);
            }
            return users;
        }
        public List<User> GetSearchPage(string searchValue, int pageNumber, int pageLenght)
        {
            if (searchValue == "")
            {
                List<User> allUsers = GetPage(pageNumber, pageLenght);
                return allUsers;
            }
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE login LIKE '%' || $searchValue || '%' LIMIT $numberOfRows OFFSET $pageNumber";
            command.Parameters.AddWithValue("$numberOfRows", pageLenght);
            command.Parameters.AddWithValue("$pageNumber", (pageNumber - 1) * pageLenght);
            command.Parameters.AddWithValue("$searchValue", searchValue);

            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = GetUser(reader);
                users.Add(user);
            }
            return users;
        }
        public int GetSearchPagesCount(string searchValue, int pageLenght)
        {
            if (searchValue == "")
            {
                int pages = GetPagesCount(pageLenght);
                return pages;
            }
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users WHERE login LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$searchValue", searchValue);
            long count = (long)command.ExecuteScalar();
            return (int)Math.Ceiling((double)count / pageLenght);
        }
        public int GetPagesCount(int pageLenght)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM users";
            long count = (long)command.ExecuteScalar();
            return (int)Math.Ceiling((double)count / pageLenght);
        }
        public bool Delete(int id)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM users WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int res = command.ExecuteNonQuery();
            if (res == 0) return false;
            else return true;
        }
        public bool Update(int userID, User updatedUser)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE users SET login = $login , password =  $password , isModerator = $isModerator, posts_id = $postsId, comments_id = $commentsId WHERE id = $id";
            command.Parameters.AddWithValue("$id", userID);
            command.Parameters.AddWithValue("$login", updatedUser.login);

            SHA256 sha256Hash = SHA256.Create();
            string passwordHash = GetHash(sha256Hash, updatedUser.password);
            command.Parameters.AddWithValue("$password", passwordHash);

            command.Parameters.AddWithValue("$isModerator", updatedUser.isModerator);
            string postsID = "";
            if (updatedUser.posts != null)
            {
                for (int i = 0; i < updatedUser.posts.Length; i++)
                {
                    postsID += updatedUser.posts[i].id + ",";
                }
            }
            command.Parameters.AddWithValue("$postsId", postsID);

            string commentsID = "";
            if (updatedUser.comments != null)
            {
                for (int i = 0; i < updatedUser.comments.Length; i++)
                {
                    commentsID += updatedUser.comments[i].id + ",";
                }
            }
            command.Parameters.AddWithValue("$commentsId", commentsID);

            try
            {
                SqliteDataReader reader = command.ExecuteReader();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        public bool UpdateEverythingButPassword(int userID, User updatedUser)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE users SET login = $login , password =  $password , isModerator = $isModerator, posts_id = $postsId, comments_id = $commentsId WHERE id = $id";
            command.Parameters.AddWithValue("$id", userID);
            command.Parameters.AddWithValue("$login", updatedUser.login);

            command.Parameters.AddWithValue("$password", updatedUser.password);

            command.Parameters.AddWithValue("$isModerator", updatedUser.isModerator);
            string postsID = "";
            if (updatedUser.posts != null)
            {
                for (int i = 0; i < updatedUser.posts.Length; i++)
                {
                    postsID += updatedUser.posts[i].id + ",";
                }
            }
            command.Parameters.AddWithValue("$postsId", postsID);

            string commentsID = "";
            if (updatedUser.comments != null)
            {
                for (int i = 0; i < updatedUser.comments.Length; i++)
                {
                    commentsID += updatedUser.comments[i].id + ",";
                }
            }
            command.Parameters.AddWithValue("$commentsId", commentsID);

            try
            {
                SqliteDataReader reader = command.ExecuteReader();
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
        private static string GetHash(HashAlgorithm hashAlgorithm, string input)
        {
            byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            var sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        public User FindUser(string userName)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users WHERE login = $login";
            command.Parameters.AddWithValue("$login", userName);

            SqliteDataReader reader = command.ExecuteReader();
            User user = new User();

            if (reader.Read())
            {
                user.id = int.Parse(reader.GetString(0));
                user.login = reader.GetString(1);
                user.password = reader.GetString(2);
                user.isModerator = int.Parse(reader.GetString(3));

                string strPostsId = reader.GetString(4);
                if (strPostsId.Length != 0)
                {
                    string[] arrPostsId = strPostsId.Split(',');
                    Post[] posts = new Post[arrPostsId.Length - 1];
                    for (int i = 0; i < posts.Length; i++)
                    {
                        int.TryParse(arrPostsId[i], out int postId);
                        posts[i] = new Post()
                        {
                            id = postId,
                            post = "DELETED",
                        };
                    }
                    user.posts = posts;
                }

                string strCommentsId = reader.GetString(5);
                string[] arrCommentsId = strCommentsId.Split(',');
                Comment[] comments = new Comment[arrCommentsId.Length - 1];
                for (int i = 0; i < comments.Length; i++)
                {
                    int.TryParse(arrCommentsId[i], out int commentId);
                    comments[i] = new Comment()
                    {
                        id = commentId,
                        comment = "DELETED",
                    };
                }
                user.comments = comments;
            }
            else
            {
                user.login = null;
                user.password = null;
                user.isModerator = 0;
                user.posts = null;
                user.comments = null;
            }
            reader.Close();
            return user;
        }
        public List<User> GetAll()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM users";

            SqliteDataReader reader = command.ExecuteReader();
            List<User> users = new List<User>();
            while (reader.Read())
            {
                User user = GetUser(reader);
                users.Add(user);
            }
            return users;
        }
    }
}