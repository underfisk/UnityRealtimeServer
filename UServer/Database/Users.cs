using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UNetwork.Utility;

namespace UServer.Database
{
    public static class Users
    {
        private static QueryFactory db = new QueryFactory(Adapter.Connection, Adapter.Compiler);
        private static readonly string table_name = "user";

        public static IEnumerable<dynamic> GetAll()
        {
            return db.Query(table_name).Get();
        }

        public static IEnumerable<dynamic> GetByUsername(string username)
        {
            return db.Query(table_name)
                    .Where("username", username)
                    .Limit(1)
                    .Get();
        }

        public static IEnumerable<dynamic> GetById(int id)
        {
            return db.Query(table_name)
                    .Where("id", id)
                    .Limit(1)
                    .Get();
        }

        /// <summary>
        /// Retrieves the user profile with the given auth credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ProfileData GetProfileWithAuth(string username, string password)
        {
            ProfileData profile = null;

            var result = db.Query(table_name)
                    .WhereRaw("username=?", username)
                    .Limit(1)
                    .Get();

            if (result.ToList().Count > 0)
            {
                var acc = result.ToList()[0];
                var decryptedPwd = Cipher.Decrypt(acc.password,"acc-pwd-256");
                if (decryptedPwd == password)
                {
                    return profile = new ProfileData
                    {
                        Id = acc.id,
                        Level = acc.level,
                        Experience = acc.exp,
                        Avatar = acc.avatar,
                        Name = acc.name,
                        Shards = acc.shards,
                        Gold = acc.gold,
                        CardBack = acc.cardback
                    };
                }
            }

            return profile;
        }
    }
}
