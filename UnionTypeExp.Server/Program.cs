using UnionTypeExp.Server;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


List<Customer> customers =
[
    new(new("Donald Duck"), new Email("donald.duck@example.com")),
    new(new("Mickey Mouse"), new Phone("+1234567890")),
    new(new("Goofy"), new EmailAndPhone(new("goofy@example.com"), new("+19876543210"))),
];

var api = app.MapGroup("/api");
api.MapGet("customers", () => customers)
   .WithName("GetCustomers");

api.MapPost("customers", (Customer customer) =>
    {
        customers.Add(customer);
        return Results.Ok();
    })
    .WithName("AddCustomer");

api.MapPost("notify", (string promotion) =>
    {
        customers.ForEach(customer => NotifyCustomerAboutPromotion(customer, promotion));
        return Results.Ok();
    })
   .WithName("NotifyAllCustomers");


app.MapDefaultEndpoints();

app.UseFileServer();

await app.RunAsync();


IResult NotifyCustomerAboutPromotion(Customer customer, string promotion) =>
    customer.ContactInfo switch
    {
        Email email => NotifyByEmail(email, promotion),
        Phone phone => NotifyByPhone(phone, promotion),
        EmailAndPhone emailAndPhone => NotifyByEmail(emailAndPhone.Email, promotion),
    };

IResult NotifyByEmail(Email email, string promotion)
{
    // Logic to send an email notification
    Console.WriteLine($"Sending email to: {email} about {promotion}");
    return Results.Ok();
}

IResult NotifyByPhone(Phone phone, string promotion)
{
    // Logic to send a phone notification
    Console.WriteLine($"Sending SMS to: {phone} about {promotion}");
    return Results.Ok();
}
