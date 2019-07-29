using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using UserLoginWithTwiloo.Models;

namespace UserLoginWithTwiloo.Classes
{
    public class DatabaseSpExec
    {
        TwilooTestContext _db;
        public DatabaseSpExec(TwilooTestContext db)
        {
            _db = db;
        }

        public User GetUser(int id)
        {
            return _db.User.FirstOrDefault(x=> x.Id == id);
        }
        public int CreateVerification(Verification verification)
        {
            var result = new SqlParameter
            {
                ParameterName = "result",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            };
            _db.Database.ExecuteSqlCommand($"exec CreateVerification @p0,@p1,@result out", verification.Code, verification.ValidUntil, result);
            return (int)result.Value;
        }
        public int Register(User user)
        {
            var result = new SqlParameter
            {
                ParameterName = "result",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            };
            string password = HashAndSalt.GenerateHash(user.Password);
            _db.Database.ExecuteSqlCommand($"exec Register @p0,@p1,@p2,@result out", user.Username, password, user.Phone, result);
            return (int)result.Value;
        }
        public int Login(string userName, string password)
        {
            var result = new SqlParameter
            {
                ParameterName = "result",
                DbType = System.Data.DbType.Int32,
                Direction = System.Data.ParameterDirection.Output
            };
            password = HashAndSalt.GenerateHash(password);
            _db.Database.ExecuteSqlCommand($"exec Login @p0,@p1,@result out", userName, password, result);
            return (int)result.Value;
        }
        public string GetVerificationCode(int verificationId)
        {
            var result = new SqlParameter
            {
                ParameterName = "result",
                SqlDbType = System.Data.SqlDbType.NVarChar,
                Size = 5,
                Direction = System.Data.ParameterDirection.Output
            };
            _db.Database.ExecuteSqlCommand($"exec GetVerification @p0,@result out", verificationId, result);
            return result.Value.ToString();
        }
    }
}
