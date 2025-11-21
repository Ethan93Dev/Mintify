using Microsoft.EntityFrameworkCore;
using MintifyApi.Data;
using MintifyApi.Models;
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

    return Results.Created($"/users/{user.Id}", new
    {
        user.Id,
        user.Username,
        user.Email,
        message = "Created new user successful!"
    });
});

app.MapPost("/login", async (LoginModel login, AppDbContext db) =>
{
    // Find user by email
    var user = await db.Users.FirstOrDefaultAsync(u => u.Email == login.Email);

    if (user == null)
    {
        return Results.Unauthorized();
    }

    // Verify password
    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash);
    if (!isPasswordValid)
    {
        return Results.Unauthorized();
    }

    // Successful login - return user info (or token if you add JWT)
    return Results.Ok(new 
    {
        user.Id,
        user.Username,
        user.Email,
        message = "Login successful! Welcome back 👋"
    });
    
});


app.Run();