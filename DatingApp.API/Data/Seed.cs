using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    public class Seed
    {
        public static void SeedUsers(ApplicationDataContext context) {

            if(!context.Users.Any()) {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users) {
                    byte[] passwordhash,passwordsalt;
                    CreatePasswordHash("whatever",out passwordhash,out passwordsalt);
                    user.PasswordHash=passwordhash;
                    user.PasswordSalt =passwordsalt;
                    user.UserName=user.UserName.ToLower();
                    context.Users.Add(user);                    
                }
                context.SaveChanges();
            }
        }
         private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {  
             using(var hmac=new System.Security.Cryptography.HMACSHA512()){
                 passwordSalt=hmac.Key;
                 passwordHash=hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
             }         
        }
    }
}