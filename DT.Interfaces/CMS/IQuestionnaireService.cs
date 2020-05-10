using DT.Domain.DTO.Questionnaire;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DT.Interfaces.CMS
{
    public interface IQuestionnaireService
    {
        Task<List<QuestionnaireDTO>> GetAll();

        Task<QuestionnaireDTO> GetById(string id);
        Task Create(QuestionnaireCreateDTO model, string username);
        Task Update(string id, QuestionnaireDTO model, string username);
        Task Delete(string id);
    }
}
