using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VOiP_Communicator.Classes
{
    static class CallRepo
    {
        public static List<CallEntry> GetCallEntries(int userId)
        {
            DBConnection con = DBConnection.Instance();
            string q =
                "(SELECT caller_id, receiver_id, status, call_date, end_date FROM calls WHERE receiver_id = @user_id OR caller_id = @user_id ORDER BY call_date) ";
            var callEntries = new List<CallEntry>();
            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@user_id", userId.ToString());

                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                  
                    callEntries.Add(new CallEntry
                    {
                        Caller_ID = (int)reader["caller_id"],
                        Receiver_ID = (int)reader["receiver_id"],
                        Type = (int)reader["status"],
                        Start_Date = reader["call_date"].ToString(),
                        End_Date = reader["end_date"].ToString()
                    });
                    
                }

                con.Close();
                foreach (var entry in callEntries) {
                    if (Globals.currentUserId == entry.Caller_ID)
                    {
                        entry.Username = UserRepo.getColumnByIds(entry.Receiver_ID, "username");
                        entry.User_ID = entry.Receiver_ID;
                    }       
                    else if (Globals.currentUserId == entry.Receiver_ID)
                    {
                        entry.Username = UserRepo.getColumnByIds(entry.Caller_ID, "username");
                        entry.Type += 1;
                        entry.User_ID = entry.Caller_ID;
                    }
                    entry.TypeToStatus();
               }

                return callEntries;
            }

            throw new Exception("DATABASE PROBLEMS");
        }

        public static void createCall(int callerId, int receiverId, string callDate, int status)
        {
            DBConnection con = DBConnection.Instance();
            string q = "insert into calls (caller_id, receiver_id, call_date, end_date, status) " +
                "VALUES (@caller_id, @receiver_id, @call_date, now(), @status);";

            if (con.IsConnect())
            {
                MySqlCommand cmd = con.Connection.CreateCommand();
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@caller_id", callerId);
                cmd.Parameters.AddWithValue("@receiver_id", receiverId);
                cmd.Parameters.AddWithValue("@call_date", callDate);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.ExecuteNonQuery();
                con.Close();
            }

            else
            {
                MessageBox.Show("Problems with database");
            }
        }

    }
}
