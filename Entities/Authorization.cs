using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;
using ServiceLib;

namespace Entities
{
    public class Authorization
    {
        public static User SignUp(string login, string password)
        {
            string databaseFile = "../data/data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source ={databaseFile}");
            connection.Open();
            UserRepo userRepo = new UserRepo(connection, databaseFile);

            User user = new User(login, password);

            user.id = userRepo.Insert(user);
            return user;
        }
        public static User LogIn(string login, string password)
        {
            string databaseFile = "../data/data.db";
            SqliteConnection connection = new SqliteConnection($"Data Source ={databaseFile}");
            connection.Open();
            UserRepo userRepo = new UserRepo(connection, databaseFile);

            User user = userRepo.FindUser(login);
            if (user.login != null)
            {
                SHA256 sha256Hash = SHA256.Create();
                string hashPassword = GetHash(sha256Hash, password);
                if (user.password != hashPassword)
                {
                    user = null;
                }
            }
            return user;
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
    }
}