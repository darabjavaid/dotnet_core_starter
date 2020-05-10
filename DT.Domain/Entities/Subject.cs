using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DT.Domain.Entities
{
    [Table("Subject", Schema = "dtt")]
    public class Subject
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [StringLength(4)]
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
    }
}
