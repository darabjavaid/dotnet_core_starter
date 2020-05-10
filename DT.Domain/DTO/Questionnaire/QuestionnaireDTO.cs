using System;
using System.Collections.Generic;
using System.Text;

namespace DT.Domain.DTO.Questionnaire
{
    public class QuestionnaireDTO
    {
        public string Id { get; set; }
        public string Question { get; set; }
        public string SubjectCode { get; set; }
        public string SubjectName { get; set; }
        public string Answer { get; set; }        
        public bool IsMock { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string Explanation { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}
