using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StoryTeller.API.Utils;
using StoryTeller.Data.DBContext;
using StoryTeller.Data.Entities;
using StoryTeller.Data.Seed;
using StoryTeller.Services.Models.ResponseModel;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddWebAPIService(builder.Configuration);

builder.Services.AddDbContext<StoryTellerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//builder.Services
//    .AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<StoryTellerContext>()
//    .AddDefaultTokenProviders()
//    .AddApiEndpoints();

//builder.Services.Configure<IdentityOptions>(options =>
//{
//    // Password settings.
//    options.Password.RequireDigit = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireNonAlphanumeric = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequiredLength = 6;
//    options.Password.RequiredUniqueChars = 1;

//    // Lockout settings.
//    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//    options.Lockout.MaxFailedAccessAttempts = 5;
//    options.Lockout.AllowedForNewUsers = true;

//    // User settings.
//    options.User.AllowedUserNameCharacters =
//    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//    options.User.RequireUniqueEmail = false;
//});

//builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
//builder.Services.AddScoped<AuthService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"];

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!))
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                var errorMsg = context.AuthenticateFailure?.Message ?? "Authentication token is missing or invalid.";
                var response = new APIResponse<string>
                {
                    Message = "Unauthorized",
                    Errors = { errorMsg }
                };

                var jsonResponse = JsonSerializer.Serialize(response);
                return context.Response.WriteAsync(jsonResponse);
            },
            OnForbidden = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";
                var response = new APIResponse<string>
                {
                    Message = "Forbidden",
                    Errors = { "You do not have permission to access this resource." }
                };
                var jsonResponse = JsonSerializer.Serialize(response);
                return context.Response.WriteAsync(jsonResponse);
            },
        };

    });

//builder.Services.AddSwaggerGen(option =>
//{
//    ////JWT Config
//    option.DescribeAllParametersInCamelCase();
//    option.ResolveConflictingActions(conf => conf.First());
//    option.CustomSchemaIds(type => type.FullName);
//    option.SwaggerDoc("v1", new OpenApiInfo { Title = "StoryTeller API", Version = "v1" });
//    option.OperationFilter<SecurityRequirementsOperationFilter>();
//    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "Please enter a valid token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    option.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//        {
//            new OpenApiSecurityScheme
//            {
//                Reference = new OpenApiReference
//                {
//                    Type=ReferenceType.SecurityScheme,
//                    Id="Bearer"
//                }
//            },
//            new string[]{}
//        }
//    });
//});


var app = builder.Build();

app.AddWebApplicationMiddleware();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "StoryTeller API v1");
    });
//}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapGroup("/api")
    .WithTags("Identity")
    .MapIdentityApiFilterable<ApplicationUser>(new IdentityApiEndpointRouteBuilderOptions()
    {
        ExcludeRegisterPost = true,
        ExcludeLoginPost = true,
        ExcludeRefreshPost = true,
        ExcludeConfirmEmailGet = true,
        ExcludeResendConfirmationEmailPost = true,
        ExcludeForgotPasswordPost = true,
        ExcludeResetPasswordPost = true,
        ExcludeManageGroup = true,
        Exclude2faPost = true,
        ExcludegInfoGet = true,
        ExcludeInfoPost = true,
    });

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<StoryTellerContext>();
    db.Database.Migrate(); // Apply any pending migrations
    var services = scope.ServiceProvider;
    await SeedRoles.SeedAsync(services);
    await SeedCensoredWords.SeedAsync(services);
    await SeedSystemConfigs.SeedAsync(services);
}

app.Run();
