using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Model.Dto
{
    public class EmailModel
    {

        public string To { get; set; }
        public string Subject { get; set; }
      

        public EmailModel(string to, string subject)
        {
            To = to;
            Subject = subject;
           
        }

    }
}
