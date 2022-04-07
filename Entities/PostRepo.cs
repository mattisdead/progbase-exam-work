using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using ServiceLib;

namespace Entities
{
    public class PostRepo
    {
        private SqliteConnection connection;
        private static string filePath;
        public PostRepo(SqliteConnection connection, string file)
        {
            filePath = file;
            this.connection = connection;
        }
        public static Post GetPost(SqliteDataReader reader)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source ={filePath}");
            connection.Open();
            CommentRepo commentRepo = new CommentRepo(connection, filePath);
            UserRepo userRepo = new UserRepo(connection, filePath);

            Post post = new Post();
            post.id = int.Parse(reader.GetString(0));
            post.post = reader.GetString(1);
            post.idOfPinnedComment = int.Parse(reader.GetString(2));

            string strCommentsId = reader.GetString(4);
            if (strCommentsId.Length != 0)
            {
                string[] arrCommentsId = strCommentsId.Split(',');
                Comment[] comments = new Comment[arrCommentsId.Length - 1];
                for (int i = 0; i < comments.Length; i++)
                {
                    int.TryParse(arrCommentsId[i], out int commentId);
                    comments[i] = commentRepo.GetById(commentId);
                    if (comments[i].comment == null)
                    {
                        comments[i].id = commentId;
                        comments[i].comment = "DELETED";
                    }
                }
                post.comments = comments;
            }

            post.author = userRepo.GetById(int.Parse(reader.GetString(3)));

            post.author.id = int.Parse(reader.GetString(3));

            post.date = DateTime.Parse(reader.GetString(5));

            return post;
        }
        public Post GetById(int id)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            Post post = new Post();

            if (reader.Read())
            {
                post.id = int.Parse(reader.GetString(0));
                post.post = reader.GetString(1);
                post.idOfPinnedComment = int.Parse(reader.GetString(2));

                string strUserId = reader.GetString(3);
                int userId = int.Parse(strUserId);
                User author = new User(userId);
                post.author = author;

                string strCommentsId = reader.GetString(4);
                string[] arrCommentsId = strCommentsId.Split(',');
                Comment[] comments = new Comment[arrCommentsId.Length - 1];
                if (strCommentsId != "")
                {
                    for (int i = 0; i < comments.Length; i++)
                    {
                        int.TryParse(arrCommentsId[i], out int commentId);
                        comments[i] = new Comment()
                        {
                            id = commentId,
                        };
                    }
                }

                post.comments = comments;

                post.date = DateTime.Parse(reader.GetString(5));
            }
            else
            {
                post.post = null;
                post.idOfPinnedComment = 0;
                post.comments = null;
                post.author = null;
            }
            reader.Close();
            return post;
        }
        public int DeleteById(int id)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM posts WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int nChanged = command.ExecuteNonQuery();
            return nChanged;
        }
        public int Insert(Post post)
        {
            SqliteCommand command = connection.CreateCommand();
            Random rand = new Random();
            command.CommandText =
            @"INSERT INTO posts (post, idOfPinnedComment, user_id, comments_id, date) VALUES ($post, $idOfPinnedComment, $user_id, $comments_id, $date);SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$post", post.post);

            command.Parameters.AddWithValue("$user_id", post.author.id);

            string commentsId = "";
            command.Parameters.AddWithValue("$comments_id", commentsId);

            int idOfPinnedComment = 0;
            command.Parameters.AddWithValue("$idOfPinnedComment", idOfPinnedComment);

            command.Parameters.AddWithValue("$date", DateTime.Now.ToString("o"));

            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public int InsertImported(Post post)
        {
            SqliteCommand command = connection.CreateCommand();
            Random rand = new Random();
            command.CommandText =
            @"INSERT INTO posts (post, idOfPinnedComment, user_id, comments_id, date) VALUES ($post, $idOfPinnedComment, $user_id, $comments_id, $date);SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$post", post.post);
            CommentRepo commentRepo = new CommentRepo(connection, filePath);

            command.Parameters.AddWithValue("$user_id", post.author.id);

            if (post.comments != null)
            {
                string commentsId = "";
                for (int i = 0; i < post.comments.Length; i++)
                {
                    if (post.comments[i].comment != "DELETED" && post.comments[i].comment != null && post.comments[i].comment != "")
                    {
                        commentRepo.InsertImported(post.comments[i]);
                    }
                    commentsId += post.comments[i].id + ",";
                }
                command.Parameters.AddWithValue("$comments_id", commentsId);
            }
            else command.Parameters.AddWithValue("$comments_id", "");

            command.Parameters.AddWithValue("$idOfPinnedComment", post.idOfPinnedComment);

            command.Parameters.AddWithValue("$date", post.date.ToString("o"));

            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public Comment[] GetAllComments(int id)
        {
            Post post = this.GetById(id);
            return post.comments;
        }
        public Post[] GetDataForExport(string text)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE post LIKE '%' || $value || '%' ";
            command.Parameters.AddWithValue("$value", text);

            SqliteDataReader reader = command.ExecuteReader();
            Post[] posts = new Post[1];
            int i = 0;
            while (reader.Read())
            {
                if (i > posts.Length - 1) System.Array.Resize(ref posts, posts.Length + 1);
                posts[i] = GetPost(reader);
                i++;
            }
            return posts;
        }
        public int GetDataForImage(System.DateTime date)
        {
            string strDate = date.Year.ToString() + '-' + date.Month.ToString();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE date LIKE '%' || $value || '%' ";
            command.Parameters.AddWithValue("$value", strDate);

            SqliteDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read()) i++;
            return i;
        }
        public int GetPagesCount(int pageLenght)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM posts";
            long count = (long)command.ExecuteScalar();
            return (int)Math.Ceiling((double)count / pageLenght);
        }
        public int GetSearchPagesCount(string searchValue, int pageLenght)
        {
            if (searchValue == "")
            {
                int pages = GetPagesCount(pageLenght);
                return pages;
            }
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM posts WHERE post LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$searchValue", searchValue);
            long count = (long)command.ExecuteScalar();
            return (int)Math.Ceiling((double)count / pageLenght);
        }
        public List<Post> GetPage(int pageNumber, int pageLenght)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts LIMIT $numberOfRows OFFSET $pageNumber";
            command.Parameters.AddWithValue("$numberOfRows", pageLenght);
            command.Parameters.AddWithValue("$pageNumber", (pageNumber - 1) * pageLenght);

            SqliteDataReader reader = command.ExecuteReader();
            List<Post> posts = new List<Post>();
            while (reader.Read())
            {
                Post post = GetPost(reader);
                posts.Add(post);
            }
            return posts;
        }
        public List<Post> GetSearchPage(string searchValue, int pageNumber, int pageLenght)
        {
            if (searchValue == "")
            {
                List<Post> allPosts = GetPage(pageNumber, pageLenght);
                return allPosts;
            }
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts WHERE post LIKE '%' || $searchValue || '%' LIMIT $numberOfRows OFFSET $pageNumber";
            command.Parameters.AddWithValue("$numberOfRows", pageLenght);
            command.Parameters.AddWithValue("$pageNumber", (pageNumber - 1) * pageLenght);
            command.Parameters.AddWithValue("$searchValue", searchValue);

            SqliteDataReader reader = command.ExecuteReader();
            List<Post> posts = new List<Post>();
            while (reader.Read())
            {
                Post post = GetPost(reader);
                posts.Add(post);
            }
            return posts;
        }
        public bool Delete(int id)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM posts WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);
            UserRepo userRepo = new UserRepo(connection, filePath);

            if (GetById(id) != null)
            {
                Post post = GetById(id);
                post.id = id;
                if (post.author != null && userRepo.GetById(post.author.id) != null)
                {
                    post.author = userRepo.GetById(post.author.id);
                    int index = -1;
                    for (int i = 0; i < post.author.posts.Length; i++)
                    {
                        if (post.author.posts[i].id == post.id)
                        {
                            index = i;
                            post.author.posts[i] = null;
                            break;
                        }
                    }
                    if (index != -1)
                    {
                        for (int i = index; i < post.author.posts.Length; i++)
                        {
                            if (i == post.author.posts.Length - 1)
                            {
                                post.author.posts[i] = null;
                                break;
                            }
                            post.author.posts[i] = post.author.posts[i + 1];
                        }
                        Array.Resize(ref post.author.posts, post.author.posts.Length - 1);
                        userRepo.UpdateEverythingButPassword(post.author.id, post.author);
                    }
                }
            }

            int res = command.ExecuteNonQuery();
            if (res == 0) return false;
            else return true;
        }
        public bool Update(int postId, Post updatedPost)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE posts SET post = $post, idOfPinnedComment = $idOfPinnedComment, comments_id = $commentsId WHERE id = $id";
            command.Parameters.AddWithValue("$id", postId);
            command.Parameters.AddWithValue("$post", updatedPost.post);
            command.Parameters.AddWithValue("$idOfPinnedComment", updatedPost.idOfPinnedComment);

            string commentsID = "";
            if (updatedPost.comments != null)
            {
                for (int i = 0; i < updatedPost.comments.Length; i++)
                {
                    commentsID += updatedPost.comments[i].id + ",";
                }
            }
            command.Parameters.AddWithValue("$commentsId", commentsID);

            SqliteDataReader reader = command.ExecuteReader();
            return true;
        }
        public List<Post> GetAll()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM posts";

            SqliteDataReader reader = command.ExecuteReader();
            List<Post> posts = new List<Post>();
            while (reader.Read())
            {
                Post post = GetPost(reader);
                posts.Add(post);
            }
            return posts;
        }
        public int InsertWithId(Post post)
        {
            SqliteCommand command = connection.CreateCommand();
            Random rand = new Random();
            command.CommandText =
            @"INSERT INTO posts (id, post, idOfPinnedComment, user_id, comments_id, date) VALUES ($id, $post, $idOfPinnedComment, $user_id, $comments_id, $date);SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$id", post.id);
            command.Parameters.AddWithValue("$post", post.post);

            command.Parameters.AddWithValue("$user_id", post.author.id);

            string commentsId = "";
            for (int i = 0; i < post.comments.Length; i++)
            {
                commentsId += post.comments[i].id + ",";
            }
            command.Parameters.AddWithValue("$comments_id", commentsId);

            command.Parameters.AddWithValue("$idOfPinnedComment", post.idOfPinnedComment);

            command.Parameters.AddWithValue("$date", DateTime.Now.ToString("o"));

            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
    }
}