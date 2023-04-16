using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Library_project.Models
{
    public class historians
    {
        //Historian Name
        public string? fname { get; set; }
        public string? mname { get; set; }
        public string? lname { get; set; }
        public DateOnly birthday { get; set; }

        //Historian Data
        [Key] 
        public int historianid { get; set; }
        public string? expertise { get; set; }
        public string? education { get; set; }

        [ForeignKey("students")]
        public int library_card_number { get; set; }
        public students? student { get; set; }

        //Relationships
        public List<students>? studentstosee { get; set; }
    }

}
