using AutoMapper;
using DT.Domain.DTO.Questionnaire;
using DT.Domain.Entities;
using DT.Domain.Exceptions;
using DT.Domain.Interfaces;
using DT.Interfaces.CMS;
using SD.BuildingBlocks.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DT.Services.Services.CMS
{
    public class QuestionnnaireService : IQuestionnaireService
    {
        private readonly IQuestionnaireRepository _questionnaireRepository;
        private readonly IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        public QuestionnnaireService(IQuestionnaireRepository questionnaireRepository, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _questionnaireRepository = questionnaireRepository;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
        public async Task<List<QuestionnaireDTO>> GetAll()
        {
            var questionnaireList = await _questionnaireRepository.GetAllQuestionsAsync();

            if (questionnaireList == null)
                throw new AppException("Not Found");

            return _mapper.Map<List<QuestionnaireDTO>>(questionnaireList);
        }

        public async Task<QuestionnaireDTO> GetById(string id)
        {
            var questionnaire = await _questionnaireRepository.GetAsync(x => x.ID == id);
            if (questionnaire == null)
                throw new AppException("Not Found");

            questionnaire.QuestionnaireDetail = await _questionnaireRepository.GetQuestionnaireDetail(questionnaire.ID);

            return _mapper.Map<QuestionnaireDTO>(questionnaire);
        }

        public async Task Create(QuestionnaireCreateDTO model, string username)
        {
            if (model == null)
                throw new AppException("Bad Request");

            var questionnaire = _mapper.Map<Questionnaire>(model);
            var questionnaireDetail = _mapper.Map<QuestionnaireDetail>(model);

            questionnaire.ID = Guid.NewGuid().ToString();
            questionnaire.CreatedBy = username;
            questionnaire.CreatedDate = DateTime.Now;

            questionnaireDetail.QuestionID = questionnaire.ID;
            _questionnaireRepository.Add(questionnaire);            
            await _questionnaireRepository.AddQuestionnaireDetail(questionnaireDetail);
            _unitOfWork.Commit();
        }

        public async Task Update(string id, QuestionnaireDTO model, string username)
        {
            if (string.IsNullOrEmpty(id) || model == null)
                throw new AppException("Bad Request, Invalid Model or ID");

            var questionnaire = _questionnaireRepository.Get(x => x.ID == id);
            if (questionnaire == null)
                throw new AppException("No Entity Found For Update");

            var questionnaireDetail = await _questionnaireRepository.GetQuestionnaireDetail(questionnaire.ID);

            questionnaire.Question = model.Question;
            questionnaire.SubjectCode = model.SubjectCode;
            questionnaire.Answer = model.Answer;
            questionnaire.IsMock = model.IsMock;
            questionnaire.UpdatedBy = username;
            questionnaire.UpdatedDate = DateTime.Now;

            _questionnaireRepository.AddOrUpdate(questionnaire);
            if (questionnaireDetail != null)
            {
                questionnaireDetail.Option1 = model.Option1;
                questionnaireDetail.Option2 = model.Option2;
                questionnaireDetail.Option3 = model.Option3;
                questionnaireDetail.Option4 = model.Option4;
                questionnaireDetail.Explanation = model.Explanation;

                await _questionnaireRepository.UpdateQuestionnaireDetail(questionnaireDetail);
            }
            _unitOfWork.Commit();
        }

        public async Task Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
                throw new AppException("Bad Request! Invalid question id");

            var questionnaire = _questionnaireRepository.Get(x => x.ID == id);
            if (questionnaire != null)
            {
                var detail = _questionnaireRepository.GetQuestionnaireDetail(questionnaire.ID).Result;
                _questionnaireRepository.Remove(questionnaire);
                if (detail != null)
                    await _questionnaireRepository.DeleteQuestionnaireDetail(detail);
                await _unitOfWork.CommitAsync();
            }
        }
    }
}
