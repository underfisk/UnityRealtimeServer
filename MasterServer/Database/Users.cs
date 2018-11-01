using SqlKata;
using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Networking.Utility.Security;

namespace MasterServer.Database
{
    /// <summary>
    /// Users database model class
    /// </summary>
    public static class Users
    {
        /// <summary>
        /// Database QueryFactory Instance
        /// </summary>
        private static QueryFactory db = new QueryFactory(Adapter.Connection, Adapter.Compiler);

        /// <summary>
        /// Selects a user by username
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> Select(string username)
        {
            return db.Query("user")
                    .Where("username", username)
                    .Limit(1)
                    .Get();
        }

        /// <summary>
        /// Selects a user by id (primary key)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> Select(uint id)
        {
            return db.Query("user")
                    .Where("id", id)
                    .Limit(1)
                    .Get();
        }

        /// <summary>
        /// Selects all daatabase user within a limit
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static IEnumerable<dynamic> SelectAll(int limit = 100)
        {
            return db.Query("user").Limit(limit).Get();
        }

        /// <summary>
        /// Retrieves the user profile with the given auth credentials
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static ProfileData Select(string username, string password)
        {
            ProfileData profile = null;

            var result = db.Query("user")
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


        public static bool Update(uint id, params Tuple<dynamic, dynamic>[] fieldsAndValues)
        {
            return false;
        }

        public static bool Delete(uint id)
        {
            return false;
        }

        public static uint Insert(params dynamic[] data)
        {
            return 0;
        }
    }
}
