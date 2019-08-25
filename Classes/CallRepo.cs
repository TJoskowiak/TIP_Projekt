using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOiP_Communicator.Classes
{
    static class CallRepo
    {
        public static List<CallEntry> GetCallEntries(int userId)
        {
            DBConnection con = DBConnection.Instance();
            string q =
                "(SELECT caller_id as user_id, status, call_date, end_date FROM calls WHERE receiver_id = @user_id " +
                "ORDER BY call_date) " +
                "UNION ALL " +
                "(SELECT receiver_id as user_id, status, call_date, end_date FROM calls WHERE caller_id = @user_id " +
                "ORDER BY call_date) ";

            var callEntries = new List<CallEntry>();
            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@user_id", userId.ToString());

                MySqlDataReader reader = cmd.ExecuteReader();
                var userRepo = new UserRepo();
                while (reader.Read())
                {
                    callEntries.Add(new CallEntry
                    {
                        User_ID = (int)reader["user_id"],
                        Type = (int)reader["status"],
                        Start_Date = reader["call_date"].ToString(),
                        End_Date = reader["end_date"].ToString()
                    });
                }

                con.Close();
                foreach (var entry in callEntries) {
                    entry.Username = userRepo.getColumnByIds(entry.User_ID, "username");
                    entry.TypeToStatus();
               }

                return callEntries;
            }

            throw new Exception("DATABASE PROBLEMS");
        }
    }
}
