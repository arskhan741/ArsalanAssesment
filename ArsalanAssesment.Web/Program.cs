using ArsalanAssesment.Web.Configurations;
using ArsalanAssesment.Web.Data;
using ArsalanAssesment.Web.Repository;
using ArsalanAssesment.Web.Repository.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add SQL Connection
builder.Services.AddDbContext<ApplicationDBContext>(option =>
{
    option.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection"));
});

//Register for swagger Controller
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    //Add Security Defination
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter into field the word 'Bearer' followed by a space of your token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    //Add Security Requirment
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                Array.Empty<String>()
            }
        });
});



// For Identity  
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
                {
                    options.User.RequireUniqueEmail = false;
                })
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDBContext>()
                .AddDefaultTokenProviders();


// Add services to the container.
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MapperConfig));
builder.Services.AddTransient<ISaleRepository, SaleRepository>();
builder.Services.AddTransient<IDashBoardMetricsRepository, DashBoardMetricsRepository>();
builder.Services.AddTransient<IUserAuthRepository, UserAuthRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//For Authentication and JWT Token
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateActor = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        RequireExpirationTime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration.GetSection("JWT:Issuer").Value,
        ValidAudience = builder.Configuration.GetSection("JWT:Audience").Value,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("JWT:Key").Value!))
    };
    // Hook into the JWT validation process
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            // Custom logic after the token is successfully validated
            // You can access the user claims, HttpContext, etc.

            var user = context.Principal; // The validated user
            var claims = user.Claims; // Access user claims if needed

            // Example: Log the token validation
            Console.WriteLine($"Token validated for user: {user.Identity.Name}");


            CheckUserLoggedInMiddleware.IsLoggedIn = true;

            // Continue with the request
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            // Logic when authentication fails (e.g., token is invalid)
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");

            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            // Logic when the JWT challenge is issued (e.g., missing or expired token)
            Console.WriteLine("JWT challenge issued.");

            return Task.CompletedTask;
        }
    };

});

//End settings for auth

var app = builder.Build();

// Seed the admin user and role
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    await SeedAdminUserAsync(userManager, roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Register the custom middleware before authentication
app.UseMiddleware<CheckUserLoggedInMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();


// Method to seed the admin user and role
async Task SeedAdminUserAsync(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
{
    const string adminEmail = "admin@domain.com";
    const string adminUsername = "admin";
    const string adminPassword = "Nesl@admin123";
    const string adminRole = "admin";

    // Ensure the admin role exists
    if (!await roleManager.RoleExistsAsync(adminRole))
    {
        await roleManager.CreateAsync(new IdentityRole(adminRole));
    }

    // Check if the admin user exists
    var adminUser = await userManager.FindByNameAsync(adminUsername);

    if (adminUser == null)
    {
        // Create the admin user
        adminUser = new IdentityUser
        {
            UserName = adminUsername,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var createUserResult = await userManager.CreateAsync(adminUser, adminPassword);

        if (createUserResult.Succeeded)
        {
            // Assign the admin role to the user
            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
        else
        {
            // Handle any errors during user creation
            throw new Exception("Failed to create the admin user");
        }
    }
}


public class CheckUserLoggedInMiddleware
{
    private readonly RequestDelegate _next;
    public static bool IsLoggedIn = false;

    public CheckUserLoggedInMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // List of paths that can be accessed without authentication
        var bypassPaths = new[] { "/api/users/login", "/api/users/register", "/swagger" };

        // Check if the current path is in the bypass list
        if (bypassPaths.Any(path => context.Request.Path.StartsWithSegments(path, StringComparison.OrdinalIgnoreCase)))
        {
            // Bypass the authentication check for these paths
            await _next(context);
            return;
        }

        // Check if the user is authenticated for all other paths
        if (!IsLoggedIn)
        {
            // Return 401 Unauthorized if the user is not authenticated
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("User is not logged in.");
            return;
        }

        // Proceed to the next middleware if the user is authenticated
        await _next(context);
    }
}


