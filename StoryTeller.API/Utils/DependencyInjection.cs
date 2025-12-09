using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using StoryTeller.API.GraphQL;
using StoryTeller.API.Middlewares;
using StoryTeller.Data;
using StoryTeller.Data.Base;
using StoryTeller.Data.Constants;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Logger;
using StoryTeller.Data.Repositories;
using StoryTeller.Data.Repositories.Interfaces;
using StoryTeller.Data.Utils;
using StoryTeller.Services.AutoMapperMapping;
using StoryTeller.Services.Background;
using StoryTeller.Services.Configurations;
using StoryTeller.Services.Implementations;
using StoryTeller.Services.Implementations.AI;
using StoryTeller.Services.Implementations.Providers;
using StoryTeller.Services.Interfaces;
using StoryTeller.Services.Interfaces.AI;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text.Json.Serialization;
using VNPAY.NET;

namespace StoryTeller.API.Utils
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(Policies.StaffOnly, policy =>
                    policy.RequireRole(Roles.Admin, Roles.Moderator));
            });

            return services;
        }

        public static IServiceCollection AddScopedService(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<ILoggerService, LoggerService>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddTransient<CustomTokenProvider<ApplicationUser>>();
            services.AddTransient<IUserTwoFactorTokenProvider<ApplicationUser>, CustomTokenProvider<ApplicationUser>>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<IActivityLogRepository, ActivityLogRepository>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<IStoryTagRepository, StoryTagRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<IUserLibraryRepository, UserLibraryRepository>();
            services.AddScoped<IUserLibraryItemRepository, UserLibraryItemRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
            services.AddScoped<IStoryPublishRequestRepository, StoryPublishRequestRepository>();
            services.AddScoped<ICensoredWordRepository, CensoredWordRepository>();
            services.AddScoped<IIssueReportRepository, IssueReportRepository>();
            services.AddScoped<IBillingRecordRepository, BillingRecordRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<IUserWalletRepository, UserWalletRepository>();
            services.AddScoped<IUserWalletTransactionRepository, UserWalletTransactionRepository>();
            services.AddScoped<ISystemConfigurationRepository, SystemConfigurationRepository>();
            services.AddScoped<INotificationReadRepository, NotificationReadRepository>();
            services.AddScoped<IUserWalletTransactionRepository, UserWalletTransactionRepository>();


            // Services
            services.AddScoped<Validator>();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddSingleton<IBackgroundTaskToggle, BackgroundTaskToggle>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IStoryService, StoryService>();
            services.AddScoped<IActivityLogService, ActivityLogService>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<IUserLibraryService, UserLibraryService>();
            services.AddScoped<IEmailService, EmailService>();
            //services.AddScoped<IStoryTagRepository>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddScoped<ICensorService, CensorService>();
            services.AddScoped<IIssueReportService, IssueReportService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IUserWalletService, UserWalletService>();
            services.AddScoped<IUserInitializationService, UserInitializationService>();
            services.AddScoped<IMigrationService, MigrationService>();
            services.AddScoped<IBillingRecordService, BillingRecordService>();
            services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
            services.AddScoped<IUserWalletTransactionService, UserWalletTransactionService>();

            //services.AddHttpClient<OpenAiChatProvider>();
            services.AddHttpClient();
            services.AddScoped<GeminiChatProvider>();
            services.AddScoped<IPollinationAIService, PollinationAIService>();
            services.AddScoped<IViettelAIService, ViettelAIService>();
            services.AddScoped<IChatProviderFactory, ChatProviderFactory>();
            services.AddScoped<IElevenLabsService, ElevenLabsService>();
            services.AddScoped<AIChatService>();

            services.AddScoped<IVnpay, Vnpay>();
            services.AddScoped<IVNPayService, VNPayService>();

            // BG Service
            services.AddHostedService<SubscriptionExpirationBGService>();
            services.AddHostedService<SubscriptionEmailReminderBGService>();
            services.AddHostedService<BillingRecordBGService>();
            
            
            services.AddMemoryCache();
            services.AddScoped<IUsageLimiter, UsageLimiter>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();


            services.AddSingleton<IDictionary<string, IKeyRotator>>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var dict = new Dictionary<string, IKeyRotator>();

                var elevenKeys = config.GetSection("ElevenLabs:ApiKeys").Get<List<string>>() ?? new();
                if (elevenKeys.Count > 0)
                    dict["ElevenLabs"] = new KeyRotator(elevenKeys, "ElevenLabs");

                return dict;
            });


            services.AddGraphQLServer()
            //
            .AddAuthorization()
            .AddFiltering()
            .AddSorting()
            .RegisterDbContextFactory<StoryTellerContext>()
            //.ModifyRequestOptions(options =>
            //{
            //    options.IncludeExceptionDetails = true;
            //    options.EnableSchemaFileSupport = true;
            //})
            .AddQueryType(d => d.Name("Query"))
                .AddTypeExtension<UserQuery>()
                .AddTypeExtension<ActivityLogQuery>()
                .AddTypeExtension<StoryQuery>()

            //.AddTypeExtension<StoryQuery>()
            //.AddMutationType(d => d.Name("Mutation"))
            //    .AddTypeExtension<UserMutation>()
            //    .AddTypeExtension<StoryMutation>()
            ;

            return services;
        }
        //builder.Services
        public static IServiceCollection AddWebAPIService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
            });
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(
                option =>
                {
                    ////JWT Config
                    option.DescribeAllParametersInCamelCase();
                    option.ResolveConflictingActions(conf => conf.First());
                    option.CustomSchemaIds(type => type.FullName);
                    option.SwaggerDoc("v1", new OpenApiInfo { Title = "StoryTeller API", Version = "v1" });
                    option.OperationFilter<SecurityRequirementsOperationFilter>();
                    option.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        In = ParameterLocation.Header,
                        Description = "Please enter a valid token",
                        Name = "Authorization",
                        Type = SecuritySchemeType.Http,
                        BearerFormat = "JWT",
                        Scheme = "Bearer"
                    });
                    option.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            new string[]{}
                        }
                    });
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = System.IO.Path.Combine(AppContext.BaseDirectory, xmlFile);
                    option.IncludeXmlComments(xmlPath);
                }
            );

            var tokenSettings = configuration.GetSection("TokenOptions");
            var tokenConfig = tokenSettings.Get<TokenSettings>() ?? new TokenSettings();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<StoryTellerContext>()
                .AddDefaultTokenProviders()
                .AddApiEndpoints();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings.
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedEmail = true;

                // Reset Password, Verify Password tokens
                options.Tokens.ProviderMap.Add(tokenConfig.Provider, new TokenProviderDescriptor(typeof(CustomTokenProvider<ApplicationUser>)));
                options.Tokens.EmailConfirmationTokenProvider = tokenConfig.Provider;
                options.Tokens.PasswordResetTokenProvider = tokenConfig.Provider;
            });

            services.AddHttpContextAccessor();
            services.AddCors(options => options.AddDefaultPolicy(policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

            services.AddAutoMapper(typeof(AutoMapperProfile));
            //services.AddAutoMapper(typeof(AutoMapperProfile));
            services.AddSingleton<GlobalExceptionMiddleware>();
            services.AddScopedService();

            services.AddHttpContextAccessor();

            //Configs

            services.Configure<VnPaySettings>(configuration.GetSection("VNPay"));

            services.AddPolicies();
            return services;
        }

        //app.
        public static WebApplication AddWebApplicationMiddleware(this WebApplication webApplication)
        {
            webApplication.UseMiddleware<GlobalExceptionMiddleware>();
            webApplication.UseMiddleware<AuthorizationMiddleware>();

            webApplication.MapGraphQL();

            //var schema = webApplication.Services.GetRequiredService<ISchema>();
            //File.WriteAllText("schema.graphql", schema.ToString());

            return webApplication;
        }
    }
}
