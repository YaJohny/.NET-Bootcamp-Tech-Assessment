using DataAccess.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.Entities;
using System.Security.Claims;
using System.Text;


namespace BookManagementApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			var configuration = builder.Configuration;
            var services = builder.Services;

            var connectionString = configuration["Database:ConnectionString"];

            services.AddDbContext<BookDB>(options => options.UseSqlServer(connectionString));
			services.AddIdentity<User, IdentityRole>()
					.AddEntityFrameworkStores<BookDB>()
					.AddDefaultTokenProviders();
			services.AddControllers();
            services.AddOpenApi();
			services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
			.AddJwtBearer(options =>
			{
				options.TokenValidationParameters = new TokenValidationParameters
				{
					ValidateIssuerSigningKey = true,
					IssuerSigningKey = new SymmetricSecurityKey(
						Encoding.UTF8.GetBytes(configuration["Jwt:Secret"] ?? "SuperSecretKeyHere")
					),
					ValidateIssuer = true,
					ValidateAudience = true,
					ValidIssuer = configuration["Jwt:Issuer"],
					ValidAudience = configuration["Jwt:Audience"],
					// This ensures the role claims are correctly mapped:
					RoleClaimType = ClaimTypes.Role
				};
			});
			services.AddSwaggerGen(c =>
			{
				c.EnableAnnotations();

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Enter 'Bearer YOUR_TOKEN' in the text input below."
				});

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
						new string[] { }
					}
				});
			});


			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var serviceProvider = scope.ServiceProvider;
				await SeedRoles(serviceProvider);
				await SeedAdminUser(serviceProvider, configuration);
			}


			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
				app.UseSwagger();
				app.MapOpenApi();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
					c.RoutePrefix = string.Empty; // Swagger UI will be at the root URL (localhost:5000/)
				});
			}

			app.UseRouting();
			app.UseHttpsRedirection();

			app.UseAuthentication();
			app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync();
        }

		private static async Task SeedRoles(IServiceProvider serviceProvider)
		{
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			string[] roleNames = { "admin", "user"};
			foreach (var roleName in roleNames)
			{
				var roleExists = await roleManager.RoleExistsAsync(roleName);
				if (!roleExists)
				{
					await roleManager.CreateAsync(new IdentityRole(roleName));
				}
			}
		}

		private static async Task SeedAdminUser(IServiceProvider serviceProvider, IConfiguration configuration)
		{
			var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
			var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

			var adminRole = "admin";
			if (!await roleManager.RoleExistsAsync(adminRole))
			{
				await roleManager.CreateAsync(new IdentityRole(adminRole));
			}

			var adminEmail = configuration["DefaultAdminUser:Email"];
			var adminPassword = configuration["DefaultAdminUser:Password"];

			if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(adminPassword))
			{
				throw new Exception("Admin credentials are missing in user secrets.");
			}

			var adminUser = await userManager.FindByEmailAsync(adminEmail);
			if (adminUser == null)
			{
				adminUser = new User
				{
					UserName = adminEmail,
					Email = adminEmail,
					EmailConfirmed = true,
					FullName = "Administrator"
				};

				var result = await userManager.CreateAsync(adminUser, adminPassword);
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, adminRole);
				}
			}
		}
	}
}
