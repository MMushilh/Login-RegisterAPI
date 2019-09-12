using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace UserRE.API.Data
{
    public class AuthRepository  :IAuthRepository
    {
        private readonly  DataContext _context;
        public AuthRepository (DataContext context)
        {
          _context = context;
        }

        public async Task<Model.User> Register (Model.User user, string password)
        {
            byte[] passwordSalt,passwordHash;
            CreatePasswordHash(password, out passwordSalt, out passwordHash);
            user.PasswordSalt=passwordSalt;
            user.PasswordHash = passwordHash;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }
        private void CreatePasswordHash (string password, out byte[] passwordSalt, out byte[] passwordHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
        
        public async Task<Model.User> Login (string username , string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x=>x.UserName==username);
            if(user==null) return null;
            if(!VerifyPasswordHash(password,user.PasswordSalt,user.PasswordHash))
            return null;
            return user;
        }
        private bool VerifyPasswordHash(string password , byte[] passwordSalt, byte[] passwordHash)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i=0; i<computedHash.Length; i++)
                {
                    if(computedHash[i]!=passwordHash[i])
                    {
                        return false ;
                    }
                    
                }
                        return true;
            }
           
        }

        public async Task<bool> UserExsist (string username)
        {
            if(await _context.Users.AnyAsync(x=>x.UserName==username))
            return true;
            return false;
        }
    }
}