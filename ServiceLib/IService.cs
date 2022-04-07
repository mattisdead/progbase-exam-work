using Microsoft.Data.Sqlite;
using System.Collections.Generic;

namespace ServiceLib
{
    public interface IService
    {
        Post GetPostById(int id);
        bool UpdateUserLogin(int userID, User updatedUser);
        int DeletePostById(int id);
        int InsertPost(Post post);
        int GetPostPagesCount(int pageLenght);
        List<Post> GetPostPage(int pageNumber, int pageLenght);
        bool DeletePost(int id);
        bool UpdatePost(int concertId, Post updatedPost);
        int InsertPostWithId(Post post);
        int DeleteUserById(int id);
        User GetUserById(int id);
        int InsertUser(User user);
        int InsertUserWithId(User user);
        Post[] GetAllUsersPosts(int id);
        Comment[] GetAllUsersComments(int id);
        List<User> GetUserPage(int pageNumber, int pageLenght);
        List<User> GetSearchUserPage(string searchValue, int pageNumber, int pageLenght);
        int GetSearchUserPagesCount(string searchValue, int pageLenght);
        int GetUserPagesCount(int pageLenght);
        bool DeleteUser(int id);
        bool UpdateUser(int userID, User updatedUser);
        User FindUser(string userName);
        List<User> GetAllUsers();
        Post[] GetPostDataForExport(string text);
        int InsertImported(Post post);
        List<Post> GetAllPosts();
        int GetPostSearchPagesCount(string searchValue, int pageLenght);
        int DeleteCommentById(int id);
        Comment GetCommentById(int id);
        int InsertComment(Comment comment);
        int GetCommentDataForImage(System.DateTime date);
        int GetCommentPagesCount(int pageLenght);
        int GetCommentSearchPagesCount(string searchValue, int pageLenght);
        List<Comment> GetCommentPage(int pageNumber, int pageLenght);
        List<Comment> GetCommentSearchPage(string searchValue, int pageNumber, int pageLenght);
        List<Post> GetPostSearchPage(string searchValue, int pageNumber, int pageLenght);
        bool DeleteComment(int id);
        bool UpdateComment(int commentId, Comment updatedComment);
        List<Comment> GetAllComments();
        int InsertCommentWithId(Comment comment);
        int GetCommentsForPostPagesCount(int pageLenght, Post post);
        List<Comment> GetCommentsForPostPage(int pageNumber, int pageLenght, Post post);
        List<Comment> GetAllCommentsToThePost(Post post);
    }
}