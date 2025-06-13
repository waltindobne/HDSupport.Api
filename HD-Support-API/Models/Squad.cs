using HD_Support_API.Migrations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HD_Support_API.Models
{
    [Table("TAB_Squads")]    
    public class Squad
    {
        public int Id { get; set; }

        public string Nme_Squad { get; set; }

        public string Img_Squad { get; set; }

        public string Local_Squad { get; set; }

        public DateTime Dta_Squad { get; set; }
    }
}
