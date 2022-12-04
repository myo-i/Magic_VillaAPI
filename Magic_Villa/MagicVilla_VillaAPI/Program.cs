// using Serilog;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

/* serilogを使用したログのファイルへの書き込み　※本当はserilogを使わないならパッケージも削除したほうが良い！！
// new LoggerConfiguration()でログの構成
// MinimunLevelでエラーの最小レベルを定義することができ、それ以上のものがログとして記録される（MinimumLevel.でインテリセンスを見るとFatal, Warningなどが見れる）
// WriteTo.File("log/villaLogs.txt", rollingInterval:r)はどのファイルにいつ書き込むのかを定義している
//Log.Logger = new LoggerConfiguration().MinimumLevel.Debug()
//    .WriteTo.File("log/villaLogs.txt", rollingInterval:RollingInterval.Day).CreateLogger();

//// ログの構成を定義したらこのログ構成を使用することを伝える必要がある
//builder.Host.UseSerilog();
*/

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
