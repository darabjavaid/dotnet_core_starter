using System.ComponentModel.DataAnnotations;

namespace DT.Domain.DTO.Questionnaire
{
    public class QuestionnaireCreateDTO
    {
        [Required(ErrorMessage = "Question is required")]
        public string Question { get; set; }

        [StringLength(4)]
        [Required(ErrorMessage = "SubjectCode is required")]
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }

        [Required(ErrorMessage = "Answer required")]
        public string Answer { get; set; }
        public bool IsMock { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Explanation { get; set; }
    }
}
