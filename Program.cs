using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

class User
{
    public string Username { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
}

class Program
{
    private static readonly List<User> users = new List<User>();
    private const string UsersFilePath = "users.txt";

    static void Main()
    {
        LoadUsersFromFile();

        while (true)
        {
            Console.WriteLine("1. Register");
            Console.WriteLine("2. Login");
            Console.WriteLine("3. Exit");

            Console.Write("Enter your choice: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    Register();
                    break;
                case "2":
                    Login();
                    break;
                case "3":
                    SaveUsersToFile();
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Invalid choice. Try again.");
                    break;
            }
        }
    }

    static void LoadUsersFromFile()
    {
        if (File.Exists(UsersFilePath))
        {
            using (StreamReader reader = new StreamReader(UsersFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] parts = line.Split('|');
                    users.Add(new User { Username = parts[0], HashedPassword = parts[1] });
                }
            }
        }
    }

    static void SaveUsersToFile()
    {
        using (StreamWriter writer = new StreamWriter(UsersFilePath))
        {
            foreach (User user in users)
            {
                writer.WriteLine($"{user.Username}|{user.HashedPassword}");
            }
        }
    }

    static void Register()
    {
        Console.Write("Enter a username: ");
        string username = Console.ReadLine();

        if (users.Exists(u => u.Username == username))
        {
            Console.WriteLine("Username already exists. Choose a different one. Press Enter to try again.");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        if (!IsUsernameValid(username))
        {
            Console.WriteLine("Invalid username. Usernames cannot contain numbers. Press Enter to try again.");
            Console.ReadKey();
            Console.Clear();
            return;
        }

        Console.Write("Enter a password: ");
        string password = Console.ReadLine();

 
        string hashedPassword = HashPassword(password);

        users.Add(new User { Username = username, HashedPassword = hashedPassword });

        Console.WriteLine("Registration successful!");
    }

    static void Login()
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();

        Console.Write("Enter your password: ");
        string password = Console.ReadLine();

        User user = users.Find(u => u.Username == username && VerifyPassword(password, u.HashedPassword));

        if (user != null)
        {
            Console.WriteLine("Login successful!");
            Thread.Sleep(700);
            Console.Clear();
        }
        else
        {
            Console.WriteLine("Invalid username or password. Please try again. Press Enter.");
            Console.ReadKey();
            Console.Clear();
            return;
        }
    }

    static string HashPassword(string? password)
    {
        if (password == null)
            return string.Empty;

        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }
    }


    static bool VerifyPassword(string? inputPassword, string hashedPassword)
    {
        if (inputPassword == null)
            return false;

        return HashPassword(inputPassword) == hashedPassword;
    }


    static bool IsUsernameValid(string? username)
    {
        if (username == null)
            return false;

        foreach (char c in username)
        {
            if (char.IsDigit(c))
            {
                return false;
            }
        }
        return true;
    }
}
