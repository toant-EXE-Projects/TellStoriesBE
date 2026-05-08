
# 📖 StoryTeller Backend

This repository contains the **StoryTeller Backend API** and **CDN service** that power the StoryTeller mobile and web applications.  

The backend is built with **.NET 8**, uses **Entity Framework Core**, and supports JWT authentication, email notifications, wallet transactions, subscriptions, and external service integrations (TTS, AI, Payments).

---

## ⚡ Quickstart

```sh
# 1. Clone the repository
git clone https://github.com/your-org/StoryTellerBE.git
cd StoryTellerBE

# 2. Update configuration
# - Edit appsettings.json in StoryTeller.API and StoryTellerCDN
# - Set DB connection, JWT, Email, and 3rd-party keys

# 3. Apply migrations
dotnet ef database update --project StoryTeller.API

# 4. Run API
dotnet run --project StoryTeller.API --launch-profile https

# 5. Run CDN
dotnet run --project StoryTellerCDN --launch-profile https
```

---

## 1. Setup Backend API

### Step 1: Install Prerequisites
Make sure you have installed:
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB or full)
- PowerShell (or any terminal)

### Step 2: Configure Application Settings

Edit `appsettings.json` in the `StoryTeller.API` project.

#### Database Connection
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=StoryTeller;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
}
```

#### JWT Authentication
```json
"JwtSettings": {
  "Issuer": "StoryTellerApi",
  "Audience": "StoryTellerMobile",
  "SecretKey": "your-strong-secret-key",
  "LifeTimeMinutes": "1440"
}
```

#### Email (SMTP) Settings

Fill in with Gmail\* or another SMTP provider.  
\*For Gmail: [create an app password](https://support.google.com/accounts/answer/185833).

```json
"EmailSettings": {
  "SmtpServer": "smtp.gmail.com",
  "Port": 587,
  "SenderName": "StoryTeller App",
  "SenderEmail": "YourEmail@gmail.com",
  "Username": "YourEmail@gmail.com",
  "Password": "reallystrongpassword123",
  "SupportEmail": "YourEmail@gmail.com"
}
```

#### (Optional) 3rd Party Service Connections
```json
"ElevenLabs": {
  "ApiKeys": [ "your-api-key" ]
},
"PollinationAI": {
  "ApiKey": "your-api-key"
},
"Gemini": {
  "ApiKey": "your-api-key"
},
"ViettelAI": {
  "ApiKey": "your-api-key"
},
"VNPay": {
  "TmnCode": "your-tmnCode",
  "HashSecret": "your-HashSecret",
  "BaseUrl": "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html",
  "ReturnUrl": "https://storyteller.runasp.net/api/Vnpay/callback"
}
```

### Step 3: Build & Run the Application

Open PowerShell in the root **BE Project** folder, e.g.:

```sh
cd D:\StoryTellerBE\StoryTellerBE
dotnet build
dotnet run --project StoryTeller.API --launch-profile https
```

The API should be available at:

* [https://localhost:7248/swagger/index.html](https://localhost:7248/swagger/index.html)
* [http://localhost:5172/swagger/index.html](http://localhost:5172/swagger/index.html)

---

## 2. Setup Backend CDN

### Step 1: Install Prerequisites

Same as for API:

* .NET 8 SDK
* SQL Server

### Step 2: Configure Application Settings

Edit `appsettings.json` in the `StoryTellerCDN` project.

#### Database Connection
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=StoryTellerCDN;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
}
```

#### JWT Settings

**⚠ Must match the API’s JWT configuration. ⚠**

```json
"JwtSettings": {
  "Issuer": "StoryTellerApi",
  "Audience": "StoryTellerMobile",
  "SecretKey": "your-strong-secret-key"
}
```

### Step 3: Build & Run the Application

```sh
cd D:\StoryTellerBE\StoryTellerCDN
dotnet build
dotnet run --project StoryTellerCDN --launch-profile https
```

The CDN should be available at:

* [https://localhost:7203/swagger/index.html](https://localhost:7203/swagger/index.html)
* [http://localhost:5070/swagger/index.html](http://localhost:5070/swagger/index.html)

---

## Notes

* Ensure **API and CDN use the same JWT settings** for authorization.
* Use strong, unique values for `SecretKey` and database credentials.
* For production, configure HTTPS with a trusted SSL certificate.

<!-- ---

## 📌 License

This project is licensed under the MIT License. See [LICENSE](./LICENSE) for details. -->
