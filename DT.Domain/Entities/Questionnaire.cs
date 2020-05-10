using SD.BuildingBlocks.Infrastructure;
using System.ComponentModel.DataAnnotations.Schema;

namespace DT.Domain.Entities
{
    [Table("Questionnaire", Schema = "dtt")]
    public class Questionnaire : BaseEntity
    {
        public string Question { get; set; }
        //[ForeignKey("SubjectCode")]        
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Answer { get; set; }
        public bool? IsMock { get; set; }
        public virtual QuestionnaireDetail QuestionnaireDetail { get; set; }
    }
}
