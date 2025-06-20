using AutoMapper;
using BLL.DTO;
using BLL.DTO.ActivityTypeDto;
using BLL.DTO.AgreementDto;
using BLL.DTO.CompanieDto;
using BLL.DTO.ResponseDto;
using BLL.DTO.VacancyDto;
using BLL.DTO.WorkerDto;
using DAL.EF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Mapper
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            CreateMap<ActivityType, ActivityTypeResponseDto>();
            CreateMap<ActivityTypeCreateDto, ActivityType>();
            CreateMap<ActivityTypeUpdateDto, ActivityType>();

            CreateMap<Companie, CompanieResponseDto>()
                .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src => src.ActivityType.ActivityName));
            CreateMap<CompanieCreateDto, Companie>();
            CreateMap<CompanieUpdateDto, Companie>();

            CreateMap<Worker, WorkerResponseDto>()
                .ForMember(dest => dest.ActivityTypeName, opt => opt.MapFrom(src => src.ActivityType.ActivityName));
            CreateMap<WorkerCreateDto, Worker>();
            CreateMap<WorkerUpdateDto, Worker>();

            CreateMap<Vacancy, VacancyResponseDto>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Companie.CompanyName));
            CreateMap<VacancyCreateDto, Vacancy>();
            CreateMap<VacancyUpdateDto, Vacancy>();

            CreateMap<Agreement, AgreementResponseDto>()
                .ForMember(dest => dest.WorkerFullName, opt => opt.MapFrom(src =>
                    $"{src.Worker.LastName} {src.Worker.FirstName} {src.Worker.MiddleName}"))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Companie.CompanyName));
            CreateMap<AgreementCreateDto, Agreement>();
            CreateMap<AgreementUpdateDto, Agreement>();

            CreateMap<Response, ResponseResponseDto>()
                .ForMember(dest => dest.WorkerFullName, opt => opt.MapFrom(src =>
                    $"{src.Worker.LastName} {src.Worker.FirstName} {src.Worker.MiddleName}"))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Vacancy.Position));
            CreateMap<ResponseCreateDto, Response>()
                .ForMember(dest => dest.SentAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => ResponseStatus.Pending));
            CreateMap<ResponseUpdateDto, Response>();
        }
    }
}
