using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StudentEnrollmentSystem.Data;
using StudentEnrollmentSystem.Models;
using StudentEnrollmentSystem.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDistributedMemoryCache();

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IProgramService, ProgramService>();
builder.Services.AddScoped<ISemesterService, SemesterService>();

// Add Newtonsoft.Json
builder
    .Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Add database context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Add Identity
builder
    .Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 8;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure cookie policy
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1000);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DataInitializer.Initialize(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Add route for Account management
app.MapControllerRoute(
    name: "account",
    pattern: "Account/{action=Index}/{id?}",
    defaults: new { controller = "Account" }
);

// Add route for Course management
app.MapControllerRoute(
    name: "courses",
    pattern: "Courses/{action=Index}/{id?}",
    defaults: new { controller = "Courses" }
);

// Add route for Enrollment management
app.MapControllerRoute(
    name: "enrollment",
    pattern: "Enrollment/{action=Index}/{id?}",
    defaults: new { controller = "Enrollment" }
);

// Add route for Payment management
app.MapControllerRoute(
    name: "payment",
    pattern: "Payment/{action=Index}/{id?}",
    defaults: new { controller = "Payment" }
);

// Add route for Statements
app.MapControllerRoute(
    name: "statements",
    pattern: "Statements/{action=Index}/{id?}",
    defaults: new { controller = "Statements" }
);

// Add route for Enquiry system
app.MapControllerRoute(
    name: "enquiry",
    pattern: "Enquiry/{action=Index}/{id?}",
    defaults: new { controller = "Enquiry" }
);

// Add route for Course Schedule
app.MapControllerRoute(
    name: "courseSchedule",
    pattern: "CourseSchedule/{action=Index}/{id?}",
    defaults: new { controller = "CourseSchedule" }
);

// Default route should be last
app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
