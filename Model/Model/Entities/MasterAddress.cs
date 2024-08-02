using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Entities
{
    public class MasterAddress
    {
    
            public int AId { get; set; }
            public string AType { get; set; }

            // Navigation properties
            public ICollection<Address> Addresses { get; set; }
        }
}



