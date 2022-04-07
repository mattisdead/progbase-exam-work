using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using ServiceLib;

namespace Entities
{
    public class CommentRepo
    {
        private SqliteConnection connection;
        private static string filePath;
        public CommentRepo(SqliteConnection connection, string file)
        {
            filePath = file;
            this.connection = connection;
        }
        public static Comment GetComment(SqliteDataReader reader)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source ={filePath}");
            connection.Open();
            UserRepo userRepo = new UserRepo(connection, filePath);
            PostRepo postRepo = new PostRepo(connection, filePath);

            Comment comment = new Comment();
            comment.id = int.Parse(reader.GetString(0));
            comment.comment = reader.GetString(1);

            int userId = int.Parse(reader.GetString(2));
            comment.author = userRepo.GetById(userId);

            comment.author.id = userId;

            int postId = int.Parse(reader.GetString(3));
            if (postRepo.GetById(postId) == null)
            {
                comment.post = new Post()
                {
                    post = "DELETED",
                    id = postId,
                };
            }
            else
            {
                comment.post = postRepo.GetById(postId);

                comment.post.id = postId;
            }

            comment.date = DateTime.Parse(reader.GetString(4));

            return comment;
        }
        public int DeleteById(int id)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM comments WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int nChanged = command.ExecuteNonQuery();
            return nChanged;
        }
        public Comment GetById(int id)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source ={filePath}");
            connection.Open();
            UserRepo userRepo = new UserRepo(connection, filePath);
            PostRepo postRepo = new PostRepo(connection, filePath);

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            SqliteDataReader reader = command.ExecuteReader();
            Comment comment = new Comment();

            if (reader.Read())
            {
                comment.id = int.Parse(reader.GetString(0));
                comment.comment = reader.GetString(1);

                string strUserId = reader.GetString(2);
                int userId = int.Parse(strUserId);
                User author = new User(userId);
                comment.author = author;

                int postId = int.Parse(reader.GetString(3));
                if (postRepo.GetById(postId) == null)
                {
                    comment.post = new Post()
                    {
                        post = "DELETED",
                        id = postId,
                    };
                }
                else
                {
                    comment.post = postRepo.GetById(postId);

                    comment.post.id = postId;
                }

                comment.date = DateTime.Parse(reader.GetString(4));
            }
            else
            {
                comment.comment = null;
            }
            reader.Close();
            return comment;
        }
        public int Insert(Comment comment)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source ={filePath}");
            connection.Open();
            UserRepo userRepo = new UserRepo(connection, filePath);
            PostRepo postRepo = new PostRepo(connection, filePath);

            SqliteCommand command = connection.CreateCommand();
            Random rand = new Random();
            command.CommandText = @"INSERT INTO comments (comment, user_id, post_id, date) VALUES ($comment, $user_id, $post_id, $date);SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$comment", comment.comment);
            command.Parameters.AddWithValue("$user_id", comment.author.id);
            if (comment.post != null)
            {
                command.Parameters.AddWithValue("$post_id", comment.post.id);
            }
            else
            {
                command.Parameters.AddWithValue("$post_id", rand.Next(0, 10000));
            }
            command.Parameters.AddWithValue("$date", DateTime.Now.ToString("o"));

            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public int GetDataForImage(System.DateTime date)
        {
            string strDate = date.Year.ToString() + '.' + date.Month.ToString() + '.' + date.Day.ToString();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE date 5 ";
            command.Parameters.AddWithValue("$value", strDate);

            SqliteDataReader reader = command.ExecuteReader();
            int i = 0;
            while (reader.Read()) i++;
            return i;
        }
        public int GetPagesCount(int pageLenght)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT COUNT(*) FROM comments";
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
            command.CommandText = @"SELECT COUNT(*) FROM comments WHERE comment LIKE '%' || $searchValue || '%'";
            command.Parameters.AddWithValue("$searchValue", searchValue);
            long count = (long)command.ExecuteScalar();
            return (int)Math.Ceiling((double)count / pageLenght);
        }
        public List<Comment> GetPage(int pageNumber, int pageLenght)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments LIMIT $numberOfRows OFFSET $pageNumber";
            command.Parameters.AddWithValue("$numberOfRows", pageLenght);
            command.Parameters.AddWithValue("$pageNumber", (pageNumber - 1) * pageLenght);

            SqliteDataReader reader = command.ExecuteReader();
            List<Comment> comments = new List<Comment>();
            while (reader.Read())
            {
                Comment comment = GetComment(reader);
                comments.Add(comment);
            }
            return comments;
        }
        public List<Comment> GetSearchPage(string searchValue, int pageNumber, int pageLenght)
        {
            if (searchValue == "")
            {
                List<Comment> allComments = GetPage(pageNumber, pageLenght);
                return allComments;
            }
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments WHERE comment LIKE '%' || $searchValue || '%' LIMIT $numberOfRows OFFSET $pageNumber";
            command.Parameters.AddWithValue("$numberOfRows", pageLenght);
            command.Parameters.AddWithValue("$pageNumber", (pageNumber - 1) * pageLenght);
            command.Parameters.AddWithValue("$searchValue", searchValue);

            SqliteDataReader reader = command.ExecuteReader();
            List<Comment> comments = new List<Comment>();
            while (reader.Read())
            {
                Comment comment = GetComment(reader);
                comments.Add(comment);
            }
            return comments;
        }
        public bool Delete(int id)
        {
            SqliteConnection connection = new SqliteConnection($"Data Source ={filePath}");
            connection.Open();
            UserRepo userRepo = new UserRepo(connection, filePath);
            PostRepo postRepo = new PostRepo(connection, filePath);

            if (GetById(id) != null)
            {
                Comment comment = GetById(id);
                comment.id = id;
                if (comment.post != null && postRepo.GetById(comment.post.id) != null && comment.post.comments != null)
                {
                    comment.post = postRepo.GetById(comment.post.id);
                    int index = -1;
                    for (int i = 0; i < comment.post.comments.Length; i++)
                    {
                        if (comment.post.comments[i].id == comment.id)
                        {
                            index = i;
                            comment.post.comments[i] = null;
                            break;
                        }
                    }
                    if (index != -1)
                    {
                        for (int i = index; i < comment.post.comments.Length; i++)
                        {
                            if (i == comment.post.comments.Length - 1)
                            {
                                comment.post.comments[i] = null;
                                break;
                            }
                            comment.post.comments[i] = comment.post.comments[i + 1];
                        }
                        Array.Resize(ref comment.post.comments, comment.post.comments.Length - 1);
                        postRepo.Update(comment.post.id, comment.post);
                    }
                }
                if (comment.author != null && userRepo.GetById(comment.author.id) != null)
                {
                    comment.author = userRepo.GetById(comment.author.id);
                    int index = -1;
                    for (int i = 0; i < comment.author.comments.Length; i++)
                    {
                        if (comment.author.comments[i].id == comment.id)
                        {
                            index = i;
                            comment.author.comments[i] = null;
                            break;
                        }
                    }
                    if (index != -1)
                    {
                        for (int i = index; i < comment.author.comments.Length; i++)
                        {
                            if (i == comment.author.comments.Length - 1)
                            {
                                comment.author.comments[i] = null;
                                break;
                            }
                            comment.author.comments[i] = comment.author.comments[i + 1];
                        }
                        Array.Resize(ref comment.author.comments, comment.author.comments.Length - 1);
                        userRepo.UpdateEverythingButPassword(comment.author.id, comment.author);
                    }
                }
            }

            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"DELETE FROM comments WHERE id = $id";
            command.Parameters.AddWithValue("$id", id);

            int res = command.ExecuteNonQuery();
            if (res == 0) return false;
            else return true;
        }
        public bool Update(int commentId, Comment updatedComment)
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"UPDATE comments SET comment = $comment WHERE id = $id";
            command.Parameters.AddWithValue("$id", commentId);
            command.Parameters.AddWithValue("$comment", updatedComment.comment);

            SqliteDataReader reader = command.ExecuteReader();
            return true;
        }
        public List<Comment> GetAll()
        {
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT * FROM comments";

            SqliteDataReader reader = command.ExecuteReader();
            List<Comment> comments = new List<Comment>();
            while (reader.Read())
            {
                Comment comment = GetComment(reader);
                comments.Add(comment);
            }
            return comments;
        }
        public int InsertWithId(Comment comment)
        {
            SqliteCommand command = connection.CreateCommand();
            Random rand = new Random();
            command.CommandText = @"INSERT INTO comments (id, comment, user_id, post_id, date) VALUES ($id, $comment, $user_id, $post_id, $date);SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$id", comment.id);
            command.Parameters.AddWithValue("$comment", comment.comment);
            command.Parameters.AddWithValue("$user_id", comment.author.id);
            command.Parameters.AddWithValue("$post_id", comment.post.id);
            command.Parameters.AddWithValue("$date", DateTime.Now.ToString("o"));

            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public int GetCommentsForPostPagesCount(int pageLenght, Post post)
        {
            List<Comment> comments = new List<Comment>();

            if (post.comments != null)
            {
                for (int i = 0; i < post.comments.Length; i++)
                {
                    int commentId = post.comments[i].id;
                    Comment comment = GetById(commentId);
                    if (comment.comment != null)
                    {
                        comments.Add(comment);
                    }
                }
            }
            return (int)Math.Ceiling((double)comments.Count / pageLenght);
        }
        public int InsertImported(Comment comment)
        {
            SqliteCommand command = connection.CreateCommand();
            Random rand = new Random();
            command.CommandText =
            @"INSERT INTO comments (comment, user_id, post_id, date) VALUES ($comment, $user_id, $post_id, $date);SELECT last_insert_rowid();";
            command.Parameters.AddWithValue("$comment", comment.comment);

            command.Parameters.AddWithValue("$user_id", comment.author.id);

            command.Parameters.AddWithValue("$post_id", comment.post.id);

            command.Parameters.AddWithValue("$date", comment.date.ToString("o"));

            long newId = (long)command.ExecuteScalar();
            return (int)newId;
        }
        public List<Comment> GetCommentsForPostPage(int pageNumber, int pageLenght, Post post)
        {
            List<Comment> comments = new List<Comment>();
            if (post.comments == null) return comments;

            int index = pageLenght * pageNumber - pageLenght;
            if (post.comments.Length == 0 || index < 0)
            {
                return null;
            }
            for (int i = index; i < pageLenght; i++)
            {
                if (i > post.comments.Length - 1) break;
                Comment comm = GetById(post.comments[i].id);
                if (comm.comment == null)
                {
                    pageLenght++;
                    continue;
                }
                comments.Add(comm);
            }
            return comments;
        }
        public List<Comment> GetAllCommentsToThePost(Post post)
        {
            List<Comment> comments = new List<Comment>();

            if (post.comments != null)
            {
                for (int i = 0; i < post.comments.Length; i++)
                {
                    int commentId = post.comments[i].id;
                    Comment comment = GetById(commentId);
                    if (comment.comment != null)
                    {
                        comments.Add(comment);
                    }
                }
            }
            return comments;
        }
    }
}