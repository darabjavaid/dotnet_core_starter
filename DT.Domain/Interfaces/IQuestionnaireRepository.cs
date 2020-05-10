using DT.Domain.Entities;
using SD.BuildingBlocks.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DT.Domain.Interfaces
{
    public interface IQuestionnaireRepository : IRepository<Questionnaire>
    {
        Task<QuestionnaireDetail> GetQuestionnaireDetail(string id);
        Task AddQuestionnaireDetail(QuestionnaireDetail questionnaireDetail);
        Task UpdateQuestionnaireDetail(QuestionnaireDetail questionnaireDetail);
        Task DeleteQuestionnaireDetail(QuestionnaireDetail questionnaireDetail);
        Task<List<Questionnaire>> GetAllQuestionsAsync();
    }
}
