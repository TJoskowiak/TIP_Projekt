using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOiP_Communicator.Classes
{
    
    class CallEntry
    { 
    
        public int User_ID { get; set; }

        public string Username { get; set; }

        public string Status { get; set; }

        public int Type { get; set; }

        public string Start_Date { get; set; }

        public string End_Date { get; set; }


        public void TypeToStatus()
        {
            switch(Type)
            {
                case 1:
                    Status = "Outgoing";
                    break;
                case 2:
                    Status = "Incoming";
                    break;
                case 5:
                    Status = "Outgoing Blocked";
                    break;
                case 6:
                    Status = "Incoming Blocked";
                    break;
            }
        }
    }
}
