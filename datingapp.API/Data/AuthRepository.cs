using System;
using System.Linq;
using System.Threading.Tasks;
using datingapp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace datingapp.API.Data
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext db;
        public AuthRepository(DataContext context)
        {
            db = context;

        }
        public async Task<bool> IsUserExists(string UserName)
        {
               if(await db.Users.AnyAsync(x=>x.UserName == UserName))
               return true;
               else
               return false;
        }

        public async Task<User> Login(string UserName, string password)
        {
            var user = await db.Users.FirstOrDefaultAsync(x => x.UserName == UserName);
            if(user == null)
            {
                return null;
            }

                if(!VerifyHashPassword(password,user.PasswordHash,user.PasswordSalt)){
                return null;
                }

                return user;
        }

        private bool VerifyHashPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
             using( var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
           {
               var computedHash =  hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
               for(int i = 0;i< computedHash.Length ; i++)
               {
                   if(computedHash[i] != passwordHash[i])
                   {
                       return false;
                   }
               }
           }
           return true;
        }

        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash,passwordSalt;
            CreatePasswordHash(password,out passwordHash,out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt  = passwordSalt;
            await db.Users.AddAsync(user);
            await db.SaveChangesAsync();
            return  user;

        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
           using( var hmac = new System.Security.Cryptography.HMACSHA512())
           {
               passwordSalt = hmac.Key;
               passwordHash  = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
           }
        }
    }
}