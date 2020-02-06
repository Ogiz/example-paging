using NPoco;

namespace PagingExample.Models
{
    public class Contact
    {
        [Column("sfppos_index")]
        public int Code { get; set; }
        
        [Column("sfppos_ime")]
        public string FirstName { get; set; }
        
        [Column("sfppos_priimek")]
        public string LastName { get; set; }
        
        [Column("sfppa_kopis")]
        public string Company { get; set; }
    }
}