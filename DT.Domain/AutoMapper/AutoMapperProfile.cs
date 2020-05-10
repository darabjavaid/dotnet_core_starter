using AutoMapper;
using DT.Domain.Entities;
using DT.Domain.DTO.Users;
using DT.Domain.DTO.Questionnaire;

namespace DT.Domain.AutoMapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();


            CreateMap<QuestionnaireDTO, Questionnaire>();
                //.ForMember(x => x.QuestionnaireDetail.Option1, opt => opt.MapFrom(src => src.QuestionnaireDetail.Option1));

            CreateMap<Questionnaire, QuestionnaireDTO>()
             .ForMember(x => x.Option1, opt => opt.MapFrom(src => src.QuestionnaireDetail.Option1))
             .ForMember(x => x.Option2, opt => opt.MapFrom(src => src.QuestionnaireDetail.Option2))
             .ForMember(x => x.Option3, opt => opt.MapFrom(src => src.QuestionnaireDetail.Option3))
             .ForMember(x => x.Option4, opt => opt.MapFrom(src => src.QuestionnaireDetail.Option4))
             .ForMember(x => x.Explanation, opt => opt.MapFrom(src => src.QuestionnaireDetail.Explanation));

            CreateMap<QuestionnaireDetail, Questionnaire>();
            CreateMap<QuestionnaireCreateDTO, Questionnaire>();
            CreateMap<QuestionnaireCreateDTO, QuestionnaireDetail>();
        }
    }
}