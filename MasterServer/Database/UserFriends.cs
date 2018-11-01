using SqlKata.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Sessions;

namespace MasterServer.Database
{
    public static class UserFriends
    {
        private static QueryFactory db = new QueryFactory(Adapter.Connection, Adapter.Compiler);

        public static List<Friend> GetAll(uint playerId)
        {
            List<Friend> friendList = new List<Friend>();
            //SELECT user.id, user.name, user_friends.status FROM user_friends JOIN user ON user_friends.friend_id = user.id WHERE user_friends.user_id = ?
            var result = db.Query("user_friends")
                .Select("user.id", "user.name", "user_friends.status", "user.avatar")
                .Join("user", "user.id", "user_friends.friend_id")
                .Where("user_id", playerId)
                .Get();

            if (result.ToList().Count > 0)
            {
                foreach(var friend in result.ToList())
                {
                    friendList.Add(new Friend
                    {
                        Id = friend.id,
                        Avatar = friend.avatar,
                        Name = friend.name,
                        Status = friend.status,
                        Activity = "Playing", //for now
                        OnlineStatus = SessionManager.Exists(friend.id)
                    });
                }
            }
            return friendList;
        }
    }
}
