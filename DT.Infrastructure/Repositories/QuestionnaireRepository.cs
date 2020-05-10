using DT.Domain.Entities;
using DT.Domain.Interfaces;
using DT.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SD.BuildingBlocks.Repository;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DT.Infrastructure.Repositories
{
    public class QuestionnaireRepository : RepositoryEF<Questionnaire>, IQuestionnaireRepository
    {
        private readonly AppDBContext _dbContext;
        public IConfiguration Configuration;

        public QuestionnaireRepository(AppDBContext appDBContext, IConfiguration configuration) : base(appDBContext)
        {
            _dbContext = appDBContext;
            Configuration = configuration;
        }

        public async Task<List<Questionnaire>> GetAllQuestionsAsync()
        {
            return await _dbContext.Questionnaires.Include(x => x.QuestionnaireDetail).ToListAsync();
        }

        public async Task<QuestionnaireDetail> GetQuestionnaireDetail(string id)
        {
            return await _dbContext.QuestionnaireDetails.FirstOrDefaultAsync(x => x.QuestionID == id);
        }

        public async Task AddQuestionnaireDetail(QuestionnaireDetail questionnaireDetail)
        {
            await _dbContext.QuestionnaireDetails.AddAsync(questionnaireDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateQuestionnaireDetail(QuestionnaireDetail questionnaireDetail)
        {
            _dbContext.Update(questionnaireDetail);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteQuestionnaireDetail(QuestionnaireDetail questionnaireDetail)
        {
            _dbContext.Remove(questionnaireDetail);
            await _dbContext.SaveChangesAsync();
        }
    }
}
