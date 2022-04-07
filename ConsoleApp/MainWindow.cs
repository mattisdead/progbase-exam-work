using Terminal.Gui;
using System.Collections.Generic;
using Entities;
using ServiceLib;
using DataGenerator;
using System.Xml.Serialization;
using System.IO;
using System.Text;

namespace ConsoleApp
{
    public class MainWindow : Window
    {
        private Label pageLbl = new Label("0");
        private Label totalPagesLbl = new Label("0");
        private int page = 0;
        private int pageLenght = 10;
        public ListView allCommentsListView = new ListView(new List<Comment>());
        private ListView allUsersListView = new ListView(new List<User>());
        private ListView allPostsListView = new ListView(new List<Post>());
        private Button closeBtn;
        private FrameView frameView;
        private Button prevPageBtn;
        private Button nextPageBtn;
        public static User currentUser = null;
        private Button logInBtn;
        private Button signUpBtn;
        public static string filePath;
        private string searchValue = "";
        TextField searchInput;
        Button searchBtn;
        Button loginBtn;
        public static string serializedFilePath = "./serialized.xml";
        public MainWindow(string file)
        {
            filePath = file;
            this.Title = "App";

            OnLogOffClicked();
        }
        private void ShowCurrentCommentPage()
        {
            RemoteService.RemoteServiceCommand("GetCommentSearchPagesCount$" + searchValue + "$" + pageLenght);
            this.totalPagesLbl.Text = Deserialize<int>(serializedFilePath).ToString();

            if (totalPagesLbl.Text == "0") this.pageLbl.Text = "0";
            else this.pageLbl.Text = page.ToString();

            RemoteService.RemoteServiceCommand("GetCommentSearchPage$" + searchValue + "$" + page + "$" + pageLenght);
            this.allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));
        }
        private void ShowCurrentUserPage()
        {
            RemoteService.RemoteServiceCommand("GetSearchUserPagesCount$" + searchValue + "$" + pageLenght);
            this.totalPagesLbl.Text = Deserialize<int>(serializedFilePath).ToString();

            if (totalPagesLbl.Text == "0") this.pageLbl.Text = "0";
            else this.pageLbl.Text = page.ToString();

            RemoteService.RemoteServiceCommand("GetSearchUserPage$" + searchValue + "$" + page + "$" + pageLenght);
            this.allUsersListView.SetSource(Deserialize<List<User>>(serializedFilePath));
        }
        private void ShowCurrentPostPage()
        {
            RemoteService.RemoteServiceCommand("GetPostSearchPagesCount$" + searchValue + "$" + pageLenght);
            this.totalPagesLbl.Text = Deserialize<int>(serializedFilePath).ToString();

            if (totalPagesLbl.Text == "0") this.pageLbl.Text = "0";
            else this.pageLbl.Text = page.ToString();

            RemoteService.RemoteServiceCommand("GetPostSearchPage$" + searchValue + "$" + page + "$" + pageLenght);
            this.allPostsListView.SetSource(Deserialize<List<Post>>(serializedFilePath));
        }
        private void OnOpenUser(ListViewItemEventArgs args)
        {
            User user = (User)args.Value;
            OpenUserDialog dialog = new OpenUserDialog();
            dialog.SetUser(user);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                if (currentUser.id != user.id)
                {
                    MessageBox.ErrorQuery("Delete user", "You're not allowed to delete this user", "Ok");
                    return;
                }
                RemoteService.RemoteServiceCommand("DeleteUser$" + user.id);
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetUserPagesCount$" + pageLenght);
                    int pages = Deserialize<int>(serializedFilePath);
                    if (page > pages && page > 0)
                    {
                        page--;
                        this.ShowCurrentUserPage();
                    }
                    RemoteService.RemoteServiceCommand("GetUserPage$" + page + "$" + pageLenght);
                    allUsersListView.SetSource(Deserialize<List<User>>(serializedFilePath));
                }
                else
                {
                    MessageBox.ErrorQuery("Delete user", "Failed to delete user", "Ok");
                }
            }
            if (dialog.edited)
            {
                if (currentUser.id != user.id)
                {
                    MessageBox.ErrorQuery("Edit user", "You're not allowed to edit this user", "Ok");
                    return;
                }
                currentUser = dialog.GetUser();
                Serialize<User>(dialog.GetUser(), serializedFilePath);
                if (dialog.passwordEdited == true)
                {
                    RemoteService.RemoteServiceCommand("UpdateUser$" + user.id);
                }
                else
                {
                    RemoteService.RemoteServiceCommand("UpdateUserLogin$" + user.id);
                }
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetUserPage$" + page + "$" + pageLenght);
                    allUsersListView.SetSource(Deserialize<List<User>>(serializedFilePath));
                    LoggedInMain();
                }
                else
                {
                    MessageBox.ErrorQuery("Update user", "Failed to update user", "Ok");
                }
            }
        }
        private void OnOpenPost(ListViewItemEventArgs args)
        {
            Post post = (Post)args.Value;
            OpenPostDialog dialog = new OpenPostDialog();
            dialog.SetPost(post);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                if (currentUser.id != post.author.id && currentUser.isModerator != 1)
                {
                    MessageBox.ErrorQuery("Delete post", "You're not allowed to delete this post", "Ok");
                    return;
                }
                RemoteService.RemoteServiceCommand("DeletePost$" + post.id);
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetPostPagesCount$" + pageLenght);
                    int pages = Deserialize<int>(serializedFilePath);
                    if (page > pages && page > 0)
                    {
                        page--;
                        this.ShowCurrentPostPage();
                    }
                    RemoteService.RemoteServiceCommand("GetPostPage$" + page + "$" + pageLenght);
                    allPostsListView.SetSource(Deserialize<List<Post>>(serializedFilePath));
                }
                else
                {
                    MessageBox.ErrorQuery("Delete post", "Failed to delete post", "Ok");
                }
            }
            if (dialog.edited)
            {
                if (currentUser.id != post.author.id)
                {
                    MessageBox.ErrorQuery("Edit post", "You're not allowed to edit this post", "Ok");
                    return;
                }
                Serialize<Post>(dialog.GetPost(), serializedFilePath);
                RemoteService.RemoteServiceCommand("UpdatePost$" + post.id);
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetPostPage$" + page + "$" + pageLenght);
                    allPostsListView.SetSource(Deserialize<List<Post>>(serializedFilePath));
                }
                else
                {
                    MessageBox.ErrorQuery("Update post", "Failed to update post", "Ok");
                }
            }
        }
        private void OnOpenComment(ListViewItemEventArgs args)
        {
            Comment comment = (Comment)args.Value;
            OpenCommentDialog dialog = new OpenCommentDialog(comment);
            dialog.SetComment(comment);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                if (currentUser.id != comment.author.id && currentUser.isModerator != 1)
                {
                    MessageBox.ErrorQuery("Delete comments", "You're not allowed to delete this comment", "Ok");
                    return;
                }
                RemoteService.RemoteServiceCommand("DeleteComment$" + comment.id);
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetCommentPagesCount$" + pageLenght);
                    int pages = Deserialize<int>(serializedFilePath);
                    if (page > pages && page > 0)
                    {
                        page--;
                        this.ShowCurrentCommentPage();
                    }
                    RemoteService.RemoteServiceCommand("GetCommentPage$" + page + "$" + pageLenght);
                    allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));
                }
                else
                {
                    MessageBox.ErrorQuery("Delete comment", "Failed to delete comment", "Ok");
                }
            }
            if (dialog.edited)
            {
                if (currentUser.id != comment.author.id)
                {
                    MessageBox.ErrorQuery("Edit comment", "You're not allowed to edit this comment", "Ok");
                    return;
                }
                comment = dialog.GetComment();
                int postId = comment.post.id;
                comment.post = new Post(postId);
                Serialize<Comment>(dialog.GetComment(), serializedFilePath);
                RemoteService.RemoteServiceCommand("UpdateComment$" + comment.id);
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetCommentPage$" + page + "$" + pageLenght);
                    allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));
                }
                else
                {
                    MessageBox.ErrorQuery("Update comment", "Failed to update comment", "Ok");
                }
            }
        }
        private void OnCreateUserButtonClicked()
        {
            if (currentUser.isModerator == 0)
            {
                MessageBox.ErrorQuery("Create user", "Only moderators can create users", "Ok");
                return;
            }
            CreateUserDialog dialog = new CreateUserDialog();
            Application.Run(dialog);

            if (!dialog._canceled)
            {
                User user = dialog.GetUser();

                Serialize<User>(user, serializedFilePath);
                RemoteService.RemoteServiceCommand("InsertUser$");
                int id = Deserialize<int>(serializedFilePath);
                if (id == -1)
                {
                    MessageBox.ErrorQuery("Create user", "Login is already taken", "Ok");
                    return;
                }
                user.id = id;

                RemoteService.RemoteServiceCommand("GetUserPage$" + page + "$" + pageLenght);
                allUsersListView.SetSource(Deserialize<List<User>>(serializedFilePath));
            }
        }
        public void OnCreateCommentButtonClicked()
        {
            CreateCommentDialog dialog = new CreateCommentDialog();
            Application.Run(dialog);

            if (!dialog._canceled)
            {
                Comment comment = dialog.GetComment();
                comment.author = currentUser;

                Serialize<Comment>(comment, serializedFilePath);
                RemoteService.RemoteServiceCommand("InsertComment$");
                int id = Deserialize<int>(serializedFilePath);

                comment.id = id;
                if (currentUser.comments == null)
                {
                    currentUser.comments = new Comment[] { new Comment(comment.id) };
                }
                else
                {
                    System.Array.Resize(ref currentUser.comments, currentUser.comments.Length + 1);
                    currentUser.comments[currentUser.comments.Length - 1] = new Comment(comment.id);
                }
                Serialize<User>(currentUser, serializedFilePath);

                RemoteService.RemoteServiceCommand("UpdateUserLogin$" + currentUser.id);
                bool res = Deserialize<bool>(serializedFilePath);

                RemoteService.RemoteServiceCommand("GetCommentPage$" + page + "$" + pageLenght);
                allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));

                RemoteService.RemoteServiceCommand("GetCommentPagesCount$" + pageLenght);
                int pages = Deserialize<int>(serializedFilePath);
                string x = totalPagesLbl.Text.ToString();
                if (pages > int.Parse(totalPagesLbl.Text.ToString()))
                {
                    page++;
                    this.ShowCurrentCommentPage();
                }
                RemoteService.RemoteServiceCommand("GetCommentPage$" + page + "$" + pageLenght);
                allCommentsListView.SetSource(Deserialize<List<Comment>>(serializedFilePath));
            }
        }
        private void OnCreatePostButtonClicked()
        {
            CreatePostDialog dialog = new CreatePostDialog();
            Application.Run(dialog);

            if (!dialog._canceled)
            {
                Post post = dialog.GetPost();
                post.author = currentUser;

                Serialize<Post>(post, serializedFilePath);
                RemoteService.RemoteServiceCommand("InsertPost$");
                int id = Deserialize<int>(serializedFilePath);

                post.id = id;
                if (currentUser.posts == null)
                {
                    currentUser.posts = new Post[] { new Post(post.id) };
                }
                else
                {
                    System.Array.Resize(ref currentUser.posts, currentUser.posts.Length + 1);
                    currentUser.posts[currentUser.posts.Length - 1] = new Post(post.id);
                }
                Serialize<User>(currentUser, serializedFilePath);
                RemoteService.RemoteServiceCommand("UpdateUserLogin$" + currentUser.id);
                bool res = Deserialize<bool>(serializedFilePath);

                RemoteService.RemoteServiceCommand("GetPostPagesCount$" + pageLenght);
                int pages = Deserialize<int>(serializedFilePath);
                if (pages > int.Parse(totalPagesLbl.Text.ToString()))
                {
                    page++;
                    this.ShowCurrentPostPage();
                }

                RemoteService.RemoteServiceCommand("GetPostPage$" + page + "$" + pageLenght);
                allPostsListView.SetSource(Deserialize<List<Post>>(serializedFilePath));
            }
        }
        private void OnShowAllUsersButtonClicked()
        {
            RemoteService.RemoteServiceCommand("GetUserPage$" + page + "$" + pageLenght);
            allUsersListView = new ListView(Deserialize<List<User>>(serializedFilePath))
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allUsersListView.OpenSelectedItem += OnOpenUser;

            prevPageBtn = new Button(2, 6, "<");
            prevPageBtn.Clicked += OnPreviousUserPageClicked;

            pageLbl = new Label(page.ToString())
            {
                X = Pos.Right(prevPageBtn) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            RemoteService.RemoteServiceCommand("GetUserPagesCount$" + pageLenght);
            totalPagesLbl = new Label(Deserialize<int>(serializedFilePath).ToString())
            {
                X = Pos.Right(pageLbl) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            nextPageBtn = new Button(">")
            {
                X = Pos.Right(totalPagesLbl) + 2,
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextUserPageClicked;
            this.Add(prevPageBtn, pageLbl, totalPagesLbl, nextPageBtn);

            frameView = new FrameView("Users")
            {
                X = 2,
                Y = 8,
                Width = Dim.Fill() - 4,
                Height = pageLenght + 4,
            };
            frameView.Add(allUsersListView);
            this.Add(frameView);

            closeBtn = new Button("Close")
            {
                X = 3,
                Y = Pos.Bottom(frameView) - 2,
            };
            closeBtn.Clicked += CloseUsersButtonClicked;
            this.Add(closeBtn);

            searchInput = new TextField("")
            {
                X = Pos.Right(nextPageBtn) + 2,
                Y = 6,
                Width = Dim.Fill() - 10,
            };
            searchBtn = new Button("Search")
            {
                X = Pos.Right(searchInput),
                Y = 6
            };
            searchBtn.Clicked += OnSearchUserClicked;
            this.Add(searchInput, searchBtn);
        }
        private void OnShowAllCommentsButtonClicked()
        {
            RemoteService.RemoteServiceCommand("GetCommentPage$" + page + "$" + pageLenght);
            allCommentsListView = new ListView(Deserialize<List<Comment>>(serializedFilePath))
            {
                Width = Dim.Fill(),
                Height = Dim.Fill(),
            };
            allCommentsListView.OpenSelectedItem += OnOpenComment;

            prevPageBtn = new Button(2, 6, "<");
            prevPageBtn.Clicked += OnPreviousCommentPageClicked;

            pageLbl = new Label(page.ToString())
            {
                X = Pos.Right(prevPageBtn) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            RemoteService.RemoteServiceCommand("GetCommentPagesCount$" + pageLenght);
            totalPagesLbl = new Label(Deserialize<int>(serializedFilePath).ToString())
            {
                X = Pos.Right(pageLbl) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            nextPageBtn = new Button(">")
            {
                X = Pos.Right(totalPagesLbl) + 2,
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextCommentPageClicked;
            this.Add(prevPageBtn, pageLbl, totalPagesLbl, nextPageBtn);

            frameView = new FrameView("Comments")
            {
                X = 2,
                Y = 8,
                Width = Dim.Fill() - 4,
                Height = pageLenght + 4,
            };
            frameView.Add(allCommentsListView);
            this.Add(frameView);

            closeBtn = new Button("Close")
            {
                X = 3,
                Y = Pos.Bottom(frameView) - 2,
            };
            closeBtn.Clicked += CloseCommentsButtonClicked;
            this.Add(closeBtn);

            searchInput = new TextField("")
            {
                X = Pos.Right(nextPageBtn) + 2,
                Y = 6,
                Width = Dim.Fill() - 10,
            };
            searchBtn = new Button("Search")
            {
                X = Pos.Right(searchInput),
                Y = 6
            };
            searchBtn.Clicked += OnSearchCommentClicked;
            this.Add(searchInput, searchBtn);
        }
        private void OnShowAllPostsButtonClicked()
        {
            RemoteService.RemoteServiceCommand("GetPostPage$" + page + "$" + pageLenght);
            allPostsListView = new ListView(Deserialize<List<Post>>(serializedFilePath))
            {
                Height = Dim.Fill(),
                Width = Dim.Fill(),
            };
            allPostsListView.OpenSelectedItem += OnOpenPost;

            prevPageBtn = new Button(2, 6, "<");
            prevPageBtn.Clicked += OnPreviousPostPageClicked;

            pageLbl = new Label(page.ToString())
            {
                X = Pos.Right(prevPageBtn) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            RemoteService.RemoteServiceCommand("GetPostPagesCount$" + pageLenght);
            totalPagesLbl = new Label(Deserialize<int>(serializedFilePath).ToString())
            {
                X = Pos.Right(pageLbl) + 2,
                Y = Pos.Top(prevPageBtn),
                Width = 5,
            };

            nextPageBtn = new Button(">")
            {
                X = Pos.Right(totalPagesLbl) + 2,
                Y = Pos.Top(prevPageBtn),
            };
            nextPageBtn.Clicked += OnNextPostPageClicked;
            this.Add(prevPageBtn, pageLbl, totalPagesLbl, nextPageBtn);


            frameView = new FrameView("Posts")
            {
                X = 2,
                Y = 8,
                Width = Dim.Fill() - 4,
                Height = pageLenght + 4,
            };
            frameView.Add(allPostsListView);
            this.Add(frameView);

            closeBtn = new Button("Close")
            {
                X = 3,
                Y = Pos.Bottom(frameView) - 2,
            };
            closeBtn.Clicked += ClosePostsButtonClicked;
            this.Add(closeBtn);

            searchInput = new TextField("")
            {
                X = Pos.Right(nextPageBtn) + 2,
                Y = 6,
                Width = Dim.Fill() - 10,
            };
            searchBtn = new Button("Search")
            {
                X = Pos.Right(searchInput),
                Y = 6
            };
            searchBtn.Clicked += OnSearchPostClicked;
            this.Add(searchInput, searchBtn);
        }
        private void CloseCommentsButtonClicked()
        {
            this.Remove(allCommentsListView);
            this.Remove(closeBtn);
            this.Remove(frameView);
            this.Remove(prevPageBtn);
            this.Remove(nextPageBtn);
            this.Remove(pageLbl);
            this.Remove(totalPagesLbl);
            this.Remove(searchBtn);
            this.Remove(searchInput);
            this.searchValue = "";
            this.page = 1;
        }
        private void CloseUsersButtonClicked()
        {
            this.Remove(allUsersListView);
            this.Remove(closeBtn);
            this.Remove(frameView);
            this.Remove(prevPageBtn);
            this.Remove(nextPageBtn);
            this.Remove(pageLbl);
            this.Remove(totalPagesLbl);
            this.Remove(searchInput);
            this.Remove(searchBtn);
            this.searchValue = "";
            this.page = 1;
        }
        private void ClosePostsButtonClicked()
        {
            this.Remove(allPostsListView);
            this.Remove(closeBtn);
            this.Remove(frameView);
            this.Remove(prevPageBtn);
            this.Remove(nextPageBtn);
            this.Remove(pageLbl);
            this.Remove(totalPagesLbl);
            this.Remove(searchInput);
            this.Remove(searchBtn);
            this.searchValue = "";
            this.page = 1;
        }
        private void OnNextCommentPageClicked()
        {
            RemoteService.RemoteServiceCommand("GetCommentSearchPagesCount$" + searchValue + "$" + pageLenght);
            int totalPages = Deserialize<int>(serializedFilePath);
            if (page >= totalPages)
            {
                return;
            }
            this.page++;
            ShowCurrentCommentPage();
        }
        private void OnPreviousCommentPageClicked()
        {
            if (page <= 1)
            {
                return;
            }
            this.page--;
            ShowCurrentCommentPage();
        }
        private void OnNextPostPageClicked()
        {
            RemoteService.RemoteServiceCommand("GetPostSearchPagesCount$" + searchValue + "$" + pageLenght);
            int totalPages = Deserialize<int>(serializedFilePath);
            if (page >= totalPages)
            {
                return;
            }
            this.page++;
            ShowCurrentPostPage();
        }
        private void OnPreviousPostPageClicked()
        {
            if (page <= 1)
            {
                return;
            }
            this.page--;
            ShowCurrentPostPage();
        }
        private void OnNextUserPageClicked()
        {
            RemoteService.RemoteServiceCommand("GetSearchUserPagesCount$" + searchValue + "$" + pageLenght);
            int totalPages = Deserialize<int>(serializedFilePath);
            if (page >= totalPages)
            {
                return;
            }
            this.page++;
            ShowCurrentUserPage();
        }
        private void OnPreviousUserPageClicked()
        {
            if (page <= 1)
            {
                return;
            }
            this.page--;
            ShowCurrentUserPage();
        }
        private void OnRandomButtonClicked()
        {
            RandomDialog dialog = new RandomDialog();
            Application.Run(dialog);

            if (!dialog._canceled)
            {
                if (!int.TryParse(dialog.numberInput.Text.ToString(), out int number))
                {
                    MessageBox.ErrorQuery("Random", "You have to enter an integer", "Ok");
                }
                string[] ent = new string[] { dialog.entityInput.Text.ToString(), number.ToString() };
                DataGenerator.DataGenerator.Main(ent);
            }
        }
        private void OnSignUpButtonClicked()
        {
            SignUpDialog dialog = new SignUpDialog();
            Application.Run(dialog);
            if (!dialog.canceled)
            {
                string login = dialog.loginInput.Text.ToString();
                string password = dialog.passwordInput.Text.ToString();
                try
                {
                    currentUser = Authorization.SignUp(login, password);
                    LoggedInMain();
                }
                catch (System.Exception)
                {
                    MessageBox.ErrorQuery("Sign up", "Login is already taken", "Ok");
                }
            }
        }
        private void OnLogInButtonClicked()
        {
            LogInDialog dialog = new LogInDialog();
            Application.Run(dialog);
            if (!dialog.canceled)
            {
                string login = dialog.loginInput.Text.ToString();
                string password = dialog.passwordInput.Text.ToString();
                try
                {
                    currentUser = Authorization.LogIn(login, password);
                }
                catch
                {
                    currentUser = null;
                }
                if (currentUser == null || currentUser.login == null)
                {
                    MessageBox.ErrorQuery("Log in", "Invalid login or password", "Ok");
                }
                else
                {
                    LoggedInMain();
                }
            }
        }
        private void LoggedInMain()
        {
            this.RemoveAll();
            page = 1;

            loginBtn = new Button(0, 0, $"{currentUser.login}");
            loginBtn.Clicked += OnProfileClicked;
            this.Add(loginBtn);

            Button logOffBtn = new Button("Log off")
            {
                X = Pos.Right(loginBtn) + 2,
                Y = 0
            };
            logOffBtn.Clicked += OnLogOffClicked;
            this.Add(logOffBtn);

            Button createNewUserBtn = new Button(2, 4, "Create new user");
            createNewUserBtn.Clicked += OnCreateUserButtonClicked;
            this.Add(createNewUserBtn);

            Button showAllUsersBtn = new Button(2, 8, "Show all users");
            showAllUsersBtn.Clicked += OnShowAllUsersButtonClicked;
            this.Add(showAllUsersBtn);

            Button createNewCommentBtn = new Button("Create new comment")
            {
                X = Pos.Right(createNewUserBtn) + 2,
                Y = 4,
            };
            createNewCommentBtn.Clicked += OnCreateCommentButtonClicked;
            this.Add(createNewCommentBtn);

            Button createNewPostBtn = new Button("Create new post")
            {
                X = Pos.Right(createNewCommentBtn) + 2,
                Y = Pos.Top(createNewCommentBtn)
            };
            createNewPostBtn.Clicked += OnCreatePostButtonClicked;
            this.Add(createNewPostBtn);

            Button showAllCommentsBtn = new Button("Show all comments")
            {
                X = Pos.Right(showAllUsersBtn) + 2,
                Y = Pos.Top(showAllUsersBtn),
            };
            showAllCommentsBtn.Clicked += OnShowAllCommentsButtonClicked;
            this.Add(showAllCommentsBtn);

            Button showAllPostsBtn = new Button("Show all posts")
            {
                X = Pos.Right(showAllCommentsBtn) + 2,
                Y = Pos.Top(showAllUsersBtn),
            };
            showAllPostsBtn.Clicked += OnShowAllPostsButtonClicked;
            this.Add(showAllPostsBtn);

            Button randomBtn = new Button(2, 12, "Random");
            randomBtn.Clicked += OnRandomButtonClicked;
            this.Add(randomBtn);
        }
        private void OnProfileClicked()
        {
            OpenUserDialog dialog = new OpenUserDialog();
            dialog.SetUser(currentUser);

            Application.Run(dialog);

            if (dialog.deleted)
            {
                RemoteService.RemoteServiceCommand("DeleteUser$" + currentUser.id);
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetUserPagesCount$" + pageLenght);
                    int pages = Deserialize<int>(serializedFilePath);
                    if (page > pages && page > 0)
                    {
                        page--;
                        this.ShowCurrentUserPage();
                    }
                    RemoteService.RemoteServiceCommand("GetUserPage$" + page + "$" + pageLenght);
                    allUsersListView.SetSource(Deserialize<List<User>>(serializedFilePath));

                    OnLogOffClicked();
                }
                else
                {
                    MessageBox.ErrorQuery("Delete user", "Failed to delete user", "Ok");
                }
            }
            if (dialog.edited)
            {
                currentUser = dialog.GetUser();
                Serialize<User>(dialog.GetUser(), serializedFilePath);
                if (dialog.passwordEdited == true)
                {
                    RemoteService.RemoteServiceCommand("UpdateUser$" + currentUser.id);
                }
                else
                {
                    RemoteService.RemoteServiceCommand("UpdateUserLogin$" + currentUser.id);
                }
                bool res = Deserialize<bool>(serializedFilePath);
                if (res)
                {
                    RemoteService.RemoteServiceCommand("GetUserPage$" + page + "$" + pageLenght);
                    allUsersListView.SetSource(Deserialize<List<User>>(serializedFilePath));
                    LoggedInMain();
                }
                else
                {
                    MessageBox.ErrorQuery("Update user", "Failed to update user", "Ok");
                }
            }
        }
        private void OnLogOffClicked()
        {
            this.RemoveAll();

            logInBtn = new Button("Log in")
            {
                X = Pos.Percent(40),
                Y = 1,
            };
            logInBtn.Clicked += OnLogInButtonClicked;
            this.Add(logInBtn);

            signUpBtn = new Button("Sign up")
            {
                X = Pos.Right(logInBtn) + 2,
                Y = 1,
            };
            signUpBtn.Clicked += OnSignUpButtonClicked;
            this.Add(signUpBtn);
        }
        private void OnSearchUserClicked()
        {
            page = 1;
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentUserPage();
        }
        private void OnSearchPostClicked()
        {
            page = 1;
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentPostPage();
        }
        private void OnSearchCommentClicked()
        {
            page = 1;
            this.searchValue = this.searchInput.Text.ToString();
            this.ShowCurrentCommentPage();
        }
        public static byte[] Serialize<T>(T data, string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            System.IO.File.WriteAllText(filePath, "");
            System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath);
            ser.Serialize(writer, data);
            writer.Close();

            string text = System.IO.File.ReadAllText(filePath);
            byte[] bytes = Encoding.ASCII.GetBytes(text);

            return bytes;
        }
        public static T Deserialize<T>(string filePath)
        {
            XmlSerializer ser = new XmlSerializer(typeof(T));
            StreamReader reader = new StreamReader(filePath);
            T value = (T)ser.Deserialize(reader);
            reader.Close();
            return value;
        }
    }
}