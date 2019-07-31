using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Windows;
using VOiP_Communicator.Classes;

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

        public List<Contact> getAllCurrentContacts()
        {
            DBConnection con = DBConnection.Instance();
            int user_id = Globals.currentUserId;

            string q = "Select u.username, c.subject_id, c.is_favourite, u.ip_address, u.status, u.profile_picture from contacts c " +
                "join users u on c.subject_id = u.user_id where c.owner_id = " + user_id.ToString() + ";";

            MySqlDataReader reader = con.query(q);
            var results = new List<Contact>();

            while (reader.Read())
            {
                Contact c = new Contact();
                c.Username = reader["username"].ToString();
                c.SubjectId =  Int32.Parse(reader["subject_id"].ToString());
                c.IsFavourite = Convert.ToBoolean(reader["is_favourite"].ToString());
                c.Ip = reader["ip_address"].ToString();
                c.Status = Int32.Parse(reader["status"].ToString());
                c.Photo = null;

                results.Add(c);
            }

            con.Close();

            return results;
        }
    }


}
