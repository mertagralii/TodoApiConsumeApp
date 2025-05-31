
# 📧 ASP.NET Core + SMTP4DEV ile E-Posta Gönderimi

## Email Notları

### 1. SMTP4DEV Kurulumu

İlk olarak Komut İstemini aç ve aşağıdaki komutu yapıştır:

```bash
dotnet tool install -g smtp4dev
```

Ardından komut satırına bunu yaz ve smtp4dev'i çalıştır:

```bash
smtp4dev
```

### 2. Gerekli NuGet Paketlerini Yükle

Aşağıdaki iki kütüphaneyi indir:

```bash
dotnet add package MailKit
dotnet add package MimeKit
```

### 3. EmailSender Servisini Oluştur

Projende `Services` klasörü oluştur ve içine `EmailSender.cs` dosyasını yaz:

```csharp
using MailKit.Net.Smtp;
using MimeKit;
using System.Threading.Tasks;

public interface IEmailSender
{
    Task SendEmailAsync(string toEmail, string subject, string body);
}

public class EmailSender : IEmailSender
{
    private readonly string _smtpServer = "localhost"; // SMTP4Dev default
    private readonly int _smtpPort = 25;               // SMTP4Dev default port
    private readonly string _fromEmail = "no-reply@todoapp.local";

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_fromEmail));
        email.To.Add(MailboxAddress.Parse(toEmail));
        email.Subject = subject;
        email.Body = new TextPart(MimeKit.Text.TextFormat.Plain) { Text = body };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.Auto);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);
    }
}
```

### 4. Program.cs Dosyasına Servisi Ekle

```csharp
builder.Services.AddScoped<IEmailSender, EmailSender>();
```

### 5. Controller'a Enjekte Et

```csharp
private readonly IEmailSender _emailSender;

public TodoController(IEmailSender emailSender)
{
    _emailSender = emailSender;
}
```

### 6. Örnek Mail Gönderme

```csharp
private readonly IEmailSender _emailSender;

public TodoController(IMapper mapper, AppDbContext context, UserManager<ApplicationUser> userManager, IEmailSender emailSender)
{
    _mapper = mapper;
    _context = context;
    _userManager = userManager;
    _emailSender = emailSender;
}

[HttpPost("[action]")]
public async Task<ActionResult<AddTodoDto>> AddTodo([FromBody] AddTodoDto todoDto)
{
    if (!ModelState.IsValid)
    {
        return BadRequest();
    }

    await _emailSender.SendEmailAsync("test@example.com", "Yeni Görev", "Yeni bir görev eklendi.");

    return Ok(todoDto);
}
```
