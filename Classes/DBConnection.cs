﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Configuration;
using MySql.Data;
using MySql.Data.MySqlClient;

namespace VOiP_Communicator.Classes
{
    public class DBConnection
    {
        private DBConnection()
        {
        }

        private string databaseName = string.Empty;
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        public string Password { get; set; }
        private MySqlConnection connection = null;

        public MySqlConnection Connection
        {
            get { return connection; }
        }

        private static DBConnection _instance = null;
        public static DBConnection Instance()
        {
            if (_instance == null)
                _instance = new DBConnection();
            return _instance;
        }

        public bool IsConnect()
        {
            if (Connection == null)
            {
                connection = new MySqlConnection(ConfigurationManager.ConnectionStrings["VOIPdata"].ConnectionString);   
            }
            connection.Open();
            return true;
        }

        public void Close()
        {
            connection.Close();
        }

        public MySqlDataReader query(string q)
        {
            if (IsConnect())
            {
                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = q;
          
                MySqlDataReader reader = cmd.ExecuteReader();
                
                return reader;
            }

            else
            {
                MessageBox.Show("Problems with database");

                return null;
            }
        }
       
    }
}
