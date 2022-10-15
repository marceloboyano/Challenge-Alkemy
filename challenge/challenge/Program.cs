using challenge.Config;
using challenge.Services;
using DataBase;
using DataBase.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
   .AddJwtBearer(opt =>
   {
       opt.TokenValidationParameters = new TokenValidationParameters
       {
           ValidIssuer = builder.Configuration["Jwt:Issuer"],
           ValidAudience = builder.Configuration["Jwt:Audience"],
           ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 },
           IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
           ValidateIssuer = true,
           ValidateAudience = true,
           ValidateLifetime = false,
           ValidateIssuerSigningKey = true
       };
   });

builder.Services.AddAuthorization();





builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    
    setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        
        Description = @"JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
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

            Array.Empty<string>()
        }
    });

    var xmlSummary = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlFile = Path.Combine(AppContext.BaseDirectory, xmlSummary);

    setupAction.IncludeXmlComments(xmlFile);
});

//builder.Services.AddDbContext<DisneyContext>(options => options.UseInMemoryDatabase("TestDB"));
builder.Services.AddDbContext<DisneyContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DisneyConnection")));

// Agrego los repositorios
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICharactersRepository, CharactersRepository>();
builder.Services.AddScoped<IGenerRepository, GenerRepository>();

// Agrego los servicios
builder.Services.AddScoped<IMovieService, MovieService>();
builder.Services.AddScoped<ICharacterService, CharacterService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IImageService, ImageService>();

//MAPPER
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Agregamos politica de CORS
builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(o => o
       .AllowAnyHeader()
       .AllowAnyOrigin()
       .AllowAnyMethod());
});


var app = builder.Build();

app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>();



app.MapControllers();
//probando en memoria

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<DisneyContext>();
    context.AddRange(
            new Gener()
            {
                Name = "Fantasia",              
           
            },
            new Gener()
            {
                Name = "Accion",               
                Movies = new[]
                {
                    new Movie()
            {
                Title="Terminator",
                Image = "test",
                Characters = new[]
                {
                    new Character()
                    {
                        Age=40,
                        History="robot malo que se vuelve bueno",
                        Image = "test",
                        Name="T-800",
                        Weight=80.0M
                    },
                    new Character()
                    {
                        Age=43,
                        History="robot malo que se vuelve bueno",
                        Name="T-1000",
                        Image = "test",
                        Weight=70.0M
                    }
                }
            }
                }
            },
            new Gener()
            {
                Name = "comedia",                
                Movies = new[]
                {
                    new Movie()
            {
                Title="ROMPEBODAS",
                Image = "test",
                Characters = new[]
                {
                    new Character()
                    {
                        Age=40,
                        History="SE COLAN EN LAS BODAS",
                        Image = "test",
                        Name="OWEN WILSON",
                        Weight=80.0M
                    },
                    new Character()
                    {
                        Age=43,
                        History="LOCO MALO",
                        Name="WILL FERREL",
                        Image = "test",
                        Weight=70.0M
                    }
                }
            }
                }
            }
        );

    context.Users.Add(new()
    {
        Password = "123456",
       Username = "marcelo",
       Email = "marcelo@gmail.com"
    });

   //context.SaveChanges();
}

app.Run();
