using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using ServiceLib;

namespace Entities
{
    public class Service : IService
    {
        CommentRepo commentRepo;
        PostRepo postRepo;
        UserRepo userRepo;
        SqliteConnection connection;
        string filePath;
        public Service(SqliteConnection connection, string filePath)
        {
            this.connection = connection;
            this.filePath = filePath;
            this.commentRepo = new CommentRepo(connection, filePath);
            this.postRepo = new PostRepo(connection, filePath);
            this.userRepo = new UserRepo(connection, filePath);
        }
        public bool DeleteComment(int id)
        {
            return commentRepo.Delete(id);
        }
        public List<Post> GetPostSearchPage(string searchValue, int pageNumber, int pageLenght)
        {
            return postRepo.GetSearchPage(searchValue, pageNumber, pageLenght);
        }
        public bool UpdateUserLogin(int userID, User updatedUser)
        {
            return userRepo.UpdateEverythingButPassword(userID, updatedUser);
        }
        public int DeleteCommentById(int id)
        {
            return commentRepo.DeleteById(id);
        }
        public bool DeletePost(int id)
        {
            return postRepo.Delete(id);
        }
        public int DeletePostById(int id)
        {
            return postRepo.DeleteById(id);
        }
        public bool DeleteUser(int id)
        {
            return userRepo.Delete(id);
        }
        public int DeleteUserById(int id)
        {
            return userRepo.DeleteById(id);
        }
        public User FindUser(string userName)
        {
            return userRepo.FindUser(userName);
        }
        public List<Comment> GetAllComments()
        {
            return commentRepo.GetAll();
        }
        public List<Comment> GetAllCommentsToThePost(Post post)
        {
            return commentRepo.GetAllCommentsToThePost(post);
        }
        public List<User> GetAllUsers()
        {
            return userRepo.GetAll();
        }
        public Comment[] GetAllUsersComments(int id)
        {
            return userRepo.GetAllComments(id);
        }
        public Post[] GetAllUsersPosts(int id)
        {
            return userRepo.GetAllPosts(id);
        }
        public List<Post> GetAllPosts()
        {
            return postRepo.GetAll();
        }
        public Comment GetCommentById(int id)
        {
            return commentRepo.GetById(id);
        }
        public int GetCommentDataForImage(DateTime date)
        {
            return commentRepo.GetDataForImage(date);
        }
        public int InsertImported(Post post)
        {
            return postRepo.InsertImported(post);
        }
        public List<Comment> GetCommentPage(int pageNumber, int pageLenght)
        {
            return commentRepo.GetPage(pageNumber, pageLenght);
        }
        public int GetCommentPagesCount(int pageLenght)
        {
            return commentRepo.GetPagesCount(pageLenght);
        }
        public List<Comment> GetCommentSearchPage(string searchValue, int pageNumber, int pageLenght)
        {
            return commentRepo.GetSearchPage(searchValue, pageNumber, pageLenght);
        }
        public int GetCommentSearchPagesCount(string searchValue, int pageLenght)
        {
            return commentRepo.GetSearchPagesCount(searchValue, pageLenght);
        }
        public List<Comment> GetCommentsForPostPage(int pageNumber, int pageLenght, Post post)
        {
            return commentRepo.GetCommentsForPostPage(pageNumber, pageLenght, post);
        }
        public int GetCommentsForPostPagesCount(int pageLenght, Post post)
        {
            return commentRepo.GetCommentsForPostPagesCount(pageLenght, post);
        }
        public Post GetPostById(int id)
        {
            return postRepo.GetById(id);
        }
        public int GetPostSearchPagesCount(string searchValue, int pageLenght)
        {
            return postRepo.GetSearchPagesCount(searchValue, pageLenght);
        }
        public List<Post> GetPostPage(int pageNumber, int pageLenght)
        {
            return postRepo.GetPage(pageNumber, pageLenght);
        }
        public int GetPostPagesCount(int pageLenght)
        {
            return postRepo.GetPagesCount(pageLenght);
        }
        public Post[] GetPostDataForExport(string text)
        {
            return postRepo.GetDataForExport(text);
        }
        public List<User> GetSearchUserPage(string searchValue, int pageNumber, int pageLenght)
        {
            return userRepo.GetSearchPage(searchValue, pageNumber, pageLenght);
        }
        public int GetSearchUserPagesCount(string searchValue, int pageLenght)
        {
            return userRepo.GetSearchPagesCount(searchValue, pageLenght);
        }
        public User GetUserById(int id)
        {
            return userRepo.GetById(id);
        }
        public List<User> GetUserPage(int pageNumber, int pageLenght)
        {
            return userRepo.GetPage(pageNumber, pageLenght);
        }
        public int GetUserPagesCount(int pageLenght)
        {
            return userRepo.GetPagesCount(pageLenght);
        }
        public int InsertComment(Comment comment)
        {
            return commentRepo.Insert(comment);
        }
        public int InsertCommentWithId(Comment comment)
        {
            return commentRepo.InsertWithId(comment);
        }
        public int InsertPost(Post post)
        {
            return postRepo.Insert(post);
        }
        public int InsertPostWithId(Post post)
        {
            return postRepo.InsertWithId(post);
        }
        public int InsertUser(User user)
        {
            return userRepo.Insert(user);
        }
        public int InsertUserWithId(User user)
        {
            return userRepo.InsertWithId(user);
        }
        public bool UpdateComment(int commentId, Comment updatedComment)
        {
            return commentRepo.Update(commentId, updatedComment);
        }
        public bool UpdatePost(int postId, Post updatedPost)
        {
            return postRepo.Update(postId, updatedPost);
        }
        public bool UpdateUser(int userID, User updatedUser)
        {
            return userRepo.Update(userID, updatedUser);
        }
    }
}