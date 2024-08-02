using System;

namespace Data.Model.Entities
{
    public class Address
    {
        public int AId { get; set; }
        public int User_Id { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string?Country { get; set; }
        public int ZipCode { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public MasterAddress? MasterAddress { get; set; }
    }
}
