using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace VOiP_Communicator
{
    class UserRepo
    {
        private static UserRepo _instance = null;
        public static UserRepo Instance()
        {
            if (_instance == null)
                _instance = new UserRepo();
            return _instance;
        }

        public string GetByUsername(string username)
        {
            DBConnection con = DBConnection.Instance();
            string q = "Select * from users where username like '" + username + "';"; 
            MySqlDataReader reader = con.query(q);
            string result = null;
            while (reader.Read())
            {
                result = reader["username"].ToString();
            }

            con.Close();

            return result;
        }

        public void createUser(string username, string email, string password, string salt, string ipAddress, int status)
        {
            DBConnection con = DBConnection.Instance();
            string q = "insert into  users (username, email, password, salt, ip_address, status, last_login_date, created_date) VALUES (@username, @email, @password, @salt, @ipAddress, @status, now(), now());";
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
    }
}
