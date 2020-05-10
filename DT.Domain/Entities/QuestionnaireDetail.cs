using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DT.Domain.Entities
{
    [Table("QuestionnaireDetail", Schema = "dtt")]
    public class QuestionnaireDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Explanation { get; set; }

        [ForeignKey("ID")]
        public string QuestionID { get; set; }
        public Questionnaire Questionnaire { get; set; }

    }
}
