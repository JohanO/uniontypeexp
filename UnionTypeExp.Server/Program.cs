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


List<Customer> customers = new()
{
    new(new("Donald Duck"), new EmailAddress("donald.duck@example.com")),
    new(new("Mickey Mouse"), new PhoneNumber("+1234567890")),
    new(new("Goofy"), new EmailAndPhone(new("goofy@example.com"), new("+19876543210"))),
};

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

app.Run();


IResult NotifyCustomerAboutPromotion(Customer customer, string promotion) =>
    customer.ContactInfo switch 
    {
        EmailAddress email => NotifyByEmail(email, promotion),
        PhoneNumber phone => NotifyByPhone(phone, promotion),
        EmailAndPhone emailAndPhone => NotifyByEmail(emailAndPhone.Email, promotion),
        null => throw new InvalidOperationException("Internal server error: Customer has no contact information. This should not happen.")
    };

IResult NotifyByEmail(EmailAddress email, string promotion)
{
    // Logic to send an email notification
    Console.WriteLine($"Sending email to: {email} about {promotion}");
    return Results.Ok();
}

IResult NotifyByPhone(PhoneNumber phoneNumber, string promotion)
{
    // Logic to send a phone notification
    Console.WriteLine($"Sending SMS to: {phoneNumber} about {promotion}");
    return Results.Ok();
}
