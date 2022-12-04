var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(option => {
    // 下記のオプションをコメントするとMediaTypeがtext/plainでも想定しているエラーが出力されるが
    // オプションを付けるとMediaTypeがapplication/jsonの場合でしか想定しているエラーが出ず、それ以外だと406が発生してしまう
    //option.ReturnHttpNotAcceptable = true;
}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
