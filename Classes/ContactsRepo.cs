using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Windows;

namespace VOiP_Communicator
{
    class ContactsRepo
    {
        private static ContactsRepo _instance = null;
        public static ContactsRepo Instance()
        {
            if (_instance == null)
                _instance = new ContactsRepo();
            return _instance;
        }

        public void createContact(int owner_id, int subject_id, int is_favourite)
        {
            DBConnection con = DBConnection.Instance();
            string q = "insert into  contacts (owner_id, subject_id, created_date, is_favourite) " +
                "VALUES (@owner_id, @subject_id, now(), @is_favourite);";

            if (con.IsConnect())
            {
                MySqlCommand cmd = con.Connection.CreateCommand();
                cmd.CommandText = q;
                cmd.Parameters.AddWithValue("@owner_id", owner_id);
                cmd.Parameters.AddWithValue("@subject_id", subject_id);
                cmd.Parameters.AddWithValue("@is_favourite", is_favourite);
                cmd.ExecuteNonQuery();
                con.Close();
            }

            else
            {
                MessageBox.Show("Problems with database");
            }
        }

        public bool contactExists(int owner_id, int subject_id)
        {
            DBConnection con = DBConnection.Instance();
            string q = "Select * from contacts where owner_id = " + owner_id + " and subject_id = " + subject_id +";";
            MySqlDataReader reader = con.query(q);
            string result = null;
            while (reader.Read())
            {
                result = reader["contact_id"].ToString();
            }

            con.Close();

            if (result == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public List<string> getAllCurrentContacts()
        {
            DBConnection con = DBConnection.Instance();
            int user_id = Globals.currentUserId;

            string q = "Select subject_id from contacts where owner_id = " + user_id.ToString() + ";";
            MySqlDataReader reader = con.query(q);
            var results = new List<string>();

            while (reader.Read())
            {
                results.Add(reader["subject_id"].ToString());
            }

            con.Close();

            return results;
        }
    }


}
