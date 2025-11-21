using Microsoft.EntityFrameworkCore;
using MintifyApi.Data;
using MintifyApi.Models;  // Make sure to import the correct namespace
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

// Add DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseHttpsRedirection();

// Signup endpoint:
app.MapPost("/signup", async (SignUpModel signUp, AppDbContext db) =>
{
    var user = new User
    {
        Username = signUp.Username,
        Email = signUp.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(signUp.Password)
    };

    db.Users.Add(user);  // Use the correct DbSet name here
    await db.SaveChangesAsync();

    return Results.Created($"/users/{user.Id}", user);
});

app.Run();