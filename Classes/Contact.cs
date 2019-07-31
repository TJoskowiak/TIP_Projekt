using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VOiP_Communicator.Classes
{
    class Contact
    {
        public string Username { get; set; }

        public int SubjectId { get; set; }

        public bool IsFavourite { get; set; }

        public string Ip { get; set; }
        
        public int Status { get; set; }

        public byte[] Photo { get; set; }
    }
}
