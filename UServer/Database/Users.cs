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

        public static IEnumerable<dynamic> GetAll()
        {
            return db.Query("users").Get();
        }

        public static IEnumerable<dynamic> GetByUsername(string username)
        {
            return db.Query("users")
                    .Where("username", username)
                    .Limit(1)
                    .Get();
        }

        public static IEnumerable<dynamic> GetById(int id)
        {
            return db.Query("users")
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

            var result = db.Query("users")
                    .Select("id","username","password", "register_date","ingame_name","ingame_avatar","ingame_shards","ingame_gold","ingame_cardback")
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
                        username = acc.username,
                        id = acc.id
                    };
                }
            }

            return profile;
        }
    }
}
