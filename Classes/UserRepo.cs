using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VOiP_Communicator.Classes
{
    static class UserRepo
    {
        static public string GetColumnValueByUsername(string username, string column)
        {
            DBConnection con = DBConnection.Instance();
            string q = "Select * from users where username like @username;";
            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@username", username);

                MySqlDataReader reader = cmd.ExecuteReader();
                string result = null;
                while (reader.Read())
                {
                    result = reader[column].ToString();
                }

                con.Close();

                return result;
            }
            throw new Exception("PROBLEMS WITH DATABASE");

        }

        static public string getColumnByIds(int userId, string column)
        {
            DBConnection con = DBConnection.Instance();
            string q = "Select * from users where user_id = @userId;";
            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@userId", userId.ToString());

                MySqlDataReader reader = cmd.ExecuteReader();

                string result = null;
                while (reader.Read())
                {
                    result = reader[column].ToString();
                }
                con.Close();

                return result;
            }
            throw new Exception("PROBLEMS WITH DATABASE");
        }

        static public List<string> getUsernamesByIds(List<string> ids)
        {
            DBConnection con = DBConnection.Instance();
            string q = "Select username from users where  FIND_IN_SET(user_id, @ids) != 0;";
            List<string> result = new List<string>();
            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("ids", string.Join(",", ids));

                MySqlDataReader reader = cmd.ExecuteReader();
            
     

            while (reader.Read())
            {
                result.Add(reader["username"].ToString());
            }

            con.Close();
            }
            return result;
        }

        static public List<SearchResult> getSimiliarUsers(string username)
        {
            DBConnection con = DBConnection.Instance();
            string q = "Select * from users where username like @username;";
            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@username", "%" + username + "%");
                MySqlDataReader reader = cmd.ExecuteReader();

                var results = new List<SearchResult>();
                while (reader.Read())
                {
                    results.Add(new SearchResult
                    {
                        Username = reader["username"].ToString(),
                        Email = reader["email"].ToString(),
                        Ip = reader["ip_address"].ToString(),
                        Last_Login_Date = reader["last_login_date"].ToString()
                    });
                }

                con.Close();

                return results;
            }

            throw new Exception("DATABASE PROBLEMS");
        }

        static public Tuple<string, string> GetSaltAndPassowrdByUsername(string username)
        {
            DBConnection con = DBConnection.Instance();
            string q = "Select * from users where username like @username;";

            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@username", username);

                MySqlDataReader reader = cmd.ExecuteReader();
                Tuple<string, string> t = null;
                while (reader.Read())
                {
                    t = new Tuple<string, string>(reader["salt"].ToString(), reader["password"].ToString());
                }

                con.Close();

                return t;
            }

            throw new Exception("DATABASE PROBLEMS");
        }

        static public void updateLogin(string username, string ipAddress)
        {
            DBConnection con = DBConnection.Instance();
            string q = "UPDATE users set ip_address = @ipAddress, last_login_date = now(), status = status | 1 where username like @username";

            if (con.IsConnect())
            {
                MySqlCommand cmd = con.Connection.CreateCommand();
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        static public void setUserOffline(string username)
        {
            DBConnection con = DBConnection.Instance();
            string q = "UPDATE users set status = status & ~1 where username like @username";

            if (con.IsConnect())
            {
                MySqlCommand cmd = con.Connection.CreateCommand();
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@username", username);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        static public void createUser(string username, string email, string password, string salt, string ipAddress, int status)
        {
            DBConnection con = DBConnection.Instance();
            string q = "insert into  users (username, email, password, salt, ip_address, status, last_login_date, created_date) " +
                "VALUES (@username, @email, @password, @salt, @ipAddress, @status, now(), now());";

            if (con.IsConnect())
            {
                MySqlCommand cmd = con.Connection.CreateCommand();
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@email", email);
                cmd.Parameters.AddWithValue("@password", password);
                cmd.Parameters.AddWithValue("@salt", salt);
                cmd.Parameters.AddWithValue("@ipAddress", ipAddress);
                cmd.Parameters.AddWithValue("@status", status);
                cmd.ExecuteNonQuery();
                con.Close();
            }

            else
            {
                MessageBox.Show("Problems with database");
            }
        }

        static public void savePhotoForUser(byte[] imageData)
        {
            DBConnection con = DBConnection.Instance();
            string q = "update users set profile_picture = @imageData where user_id = @userId";

            if (con.IsConnect())
            {
                MySqlCommand cmd = con.Connection.CreateCommand();
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@userId", Globals.currentUserId);
                cmd.Parameters.AddWithValue("@imageData", imageData);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        static public byte[] fetchPhotoByUsername(int userId)
        {
            DBConnection con = DBConnection.Instance();
            string q = "select profile_picture from users where user_id = @userId";
            object imageData = null;
            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@userId", userId.ToString());
                
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    imageData = reader["profile_picture"];
                }

                con.Close();
                return (byte[])imageData;
            }

            throw new Exception("DATABASE PROBLEMS");
        }

        static public void updateCrypto(byte[] passData, byte[] saltData)
        {
            DBConnection con = DBConnection.Instance();
            string q = "update users set crypto_pass = @crypto_pass, crypto_salt = @crypto_salt where user_id = @userId";

            if (con.IsConnect())
            {
                MySqlCommand cmd = con.Connection.CreateCommand();
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@userId", Globals.currentUserId);
                cmd.Parameters.AddWithValue("@crypto_pass", passData);
                cmd.Parameters.AddWithValue("@crypto_salt", saltData);
                cmd.ExecuteNonQuery();
                con.Close();
            }
        }

        static public Tuple<byte[], byte[]> fetchUsersCrypto(string username)
        {
            DBConnection con = DBConnection.Instance();
            string q = "Select * from users where username like @username;";

            if (con.IsConnect())
            {
                MySqlCommand cmd = new MySqlCommand(q, con.Connection);
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@username", username);

                MySqlDataReader reader = cmd.ExecuteReader();
                Tuple<byte[], byte[]> t = null;
                while (reader.Read())
                {
                    t = new Tuple<byte[], byte[]>(Encoding.ASCII.GetBytes(reader["crypto_pass"].ToString()),
                        Encoding.ASCII.GetBytes(reader["crypto_salt"].ToString()));
                }

                con.Close();

                return t;
            }

            throw new Exception("DATABASE PROBLEMS");
        }
    }
}
