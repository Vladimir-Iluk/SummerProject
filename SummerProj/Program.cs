using DAL.EF.DbCreating;
using Microsoft.EntityFrameworkCore;
using BLL.Interfaces;
using BLL.Services;
using BLL.Mapper;
using BLL.Validations.Companie;
using BLL.Validations.ActivityType;
using BLL.Validations.Agreement;
using BLL.Validations.Response;
using BLL.Validations.Vacancy;
using BLL.Validations.Worker;
using DAL.EF.Repositories.Interfaces;
using DAL.EF.Repositories;
using DAL.EF.UoW;
using FluentValidation;

namespace SummerProj 
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<SummerDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Реєстрація AutoMapper
            builder.Services.AddAutoMapper(typeof(EntityMappingProfile).Assembly);


            // Реєстрація репозиторіїв
            builder.Services.AddScoped<ICompanieRepository, CompanieRepository>();
            builder.Services.AddScoped<IActivityTypeRepository, ActivityTypeRepository>();
            builder.Services.AddScoped<IAgreementRepository, AgreementRepository>();
            builder.Services.AddScoped<IResponseRepository, ResponseRepository>();
            builder.Services.AddScoped<IVacancyRepository, VacancyRepository>();
            builder.Services.AddScoped<IWorkerRepository, WorkerRepository>();
            builder.Services.AddScoped<IGenericRepository<DAL.EF.Entities.Companie>, GenericRepository<DAL.EF.Entities.Companie>>();
            builder.Services.AddScoped<IGenericRepository<DAL.EF.Entities.ActivityType>, GenericRepository<DAL.EF.Entities.ActivityType>>();
            builder.Services.AddScoped<IGenericRepository<DAL.EF.Entities.Agreement>, GenericRepository<DAL.EF.Entities.Agreement>>();
            builder.Services.AddScoped<IGenericRepository<DAL.EF.Entities.Response>, GenericRepository<DAL.EF.Entities.Response>>();
            builder.Services.AddScoped<IGenericRepository<DAL.EF.Entities.Vacancy>, GenericRepository<DAL.EF.Entities.Vacancy>>();
            builder.Services.AddScoped<IGenericRepository<DAL.EF.Entities.Worker>, GenericRepository<DAL.EF.Entities.Worker>>();

            // Реєстрація Unit of Work
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Реєстрація сервісів
            builder.Services.AddScoped<ICompanieService, CompanieService>();
            builder.Services.AddScoped<IActivityTypeService, ActivityTypeService>();
            builder.Services.AddScoped<IAgreementService, AgreementService>();
            builder.Services.AddScoped<IResponseService, ResponseService>();
            builder.Services.AddScoped<IVacancyService, VacancyService>();
            builder.Services.AddScoped<IWorkerService, WorkerService>();

            // Реєстрація валідаторів
            builder.Services.AddScoped<IValidator<BLL.DTO.CompanieDto.CompanieCreateDto>, CompanieCreateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.CompanieDto.CompanieUpdateDto>, CompanieUpdateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.ActivityTypeDto.ActivityTypeCreateDto>, ActivityTypeCreateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.ActivityTypeDto.ActivityTypeUpdateDto>, ActivityTypeUpdateDtoValitation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.AgreementDto.AgreementCreateDto>, AgreementCreateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.AgreementDto.AgreementUpdateDto>, AgreementUpdateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.ResponseDto.ResponseCreateDto>, ResponseCreateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.ResponseDto.ResponseUpdateDto>, ResponseUpdateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.VacancyDto.VacancyCreateDto>, VacancyCreateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.VacancyDto.VacancyUpdateDto>, VacancyUpdateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.WorkerDto.WorkerCreateDto>, WorkerCreateDtoValidation>();
            builder.Services.AddScoped<IValidator<BLL.DTO.WorkerDto.WorkerUpdateDto>, WorkerUpdateDtoValidation>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
