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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DAL.EF.Entities;
using Microsoft.OpenApi.Models;
using System.Security.Claims;

namespace SummerProj 
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Summer Project API", Version = "v1" });

                // Додаємо конфігурацію для JWT автентифікації в Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddDbContext<SummerDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Identity configuration
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 8;
            })
            .AddEntityFrameworkStores<SummerDbContext>()
            .AddDefaultTokenProviders();

            // JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration")))
                };

                // Додаємо обробку подій для діагностики
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine($"Token validated successfully for user: {context.Principal?.Identity?.Name}");
                        var roles = context.Principal?.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
                        Console.WriteLine($"User roles: {string.Join(", ", roles ?? Array.Empty<string>())}");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"Challenge occurred: {context.Error}, {context.ErrorDescription}");
                        return Task.CompletedTask;
                    }
                };
            });

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
            builder.Services.AddScoped<JwtService>();

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

            // Додаємо авторизацію
            builder.Services.AddAuthorization();

            var app = builder.Build();

            // Ініціалізація бази даних при запуску
            using (var scope = app.Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SummerDbContext>();
                
                try
                {
                    // Застосовуємо міграції
                    Console.WriteLine("Застосування міграцій...");
                    context.Database.Migrate();
                    Console.WriteLine("Міграції успішно застосовані");
                    
                    // Перевіряємо чи база порожня
                    var hasActivityTypes = context.ActivityTypes.Any();
                    var hasCompanies = context.Companies.Any();
                    var hasWorkers = context.Workers.Any();
                    var hasVacancies = context.Vacancies.Any();
                    var hasResponses = context.Responses.Any();
                    var hasAgreements = context.Agreements.Any();
                    
                    Console.WriteLine($"Статус таблиць: ActivityTypes={hasActivityTypes}, Companies={hasCompanies}, Workers={hasWorkers}, Vacancies={hasVacancies}, Responses={hasResponses}, Agreements={hasAgreements}");
                    
                    // Ініціалізуємо тестові дані, якщо база порожня
                    if (!hasActivityTypes && !hasCompanies && !hasWorkers && !hasVacancies && !hasResponses && !hasAgreements)
                    {
                        Console.WriteLine("Починаємо ініціалізацію тестових даних...");
                        context.SeedData();
                        Console.WriteLine("База даних успішно ініціалізована тестовими даними");
                    }
                    else
                    {
                        Console.WriteLine("База даних вже містить дані, ініціалізація пропущена");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"КРИТИЧНА ПОМИЛКА при ініціалізації бази даних: {ex.Message}");
                    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                    
                    // Перевіряємо підключення до бази
                    try
                    {
                        var canConnect = context.Database.CanConnect();
                        Console.WriteLine($"Можливість підключення до БД: {canConnect}");
                    }
                    catch (Exception connEx)
                    {
                        Console.WriteLine($"Помилка підключення до БД: {connEx.Message}");
                    }
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Важливо: UseAuthentication має бути перед UseAuthorization
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Ініціалізація ролей
            using (var scope = app.Services.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var roles = new[] { "Admin", "User" };

                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new IdentityRole(role));
                    }
                }
            }

            await app.RunAsync();
        }
    }
}
