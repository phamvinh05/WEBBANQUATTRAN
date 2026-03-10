using Microsoft.AspNetCore.Identity;

var hasher = new PasswordHasher<string>();
string password = "12345";
string hashed = hasher.HashPassword(null, password);

Console.WriteLine(hashed);
