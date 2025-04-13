# 📧 Email Address Verification API

A scalable and modular ASP.NET Core Web API that performs advanced validation on email addresses using multiple verification layers. It supports customizable strictness levels and returns a detailed checklist and trust score for each email.

---

## 🔍 Features

- **Strictness-Based Validation**: Choose between low, medium, or high strictness for different verification depths.
- **Scoring System**: Assigns weight to each check and calculates a total trust score.
- **Timeout Management**: Supports per-email timeout to ensure fast responses.
- **Caching**: Uses in-memory caching for TLDs, vulgar words, disposable domains, and whitelisted domains.
- **Rate Limiting**: Token bucket rate limiting on a per-IP basis.
- **Detailed Checklist**: Each result includes a breakdown of passed/failed checks.

---

## 🧪 Validation Layers

| Strictness Level | Checks Performed |
|------------------|------------------|
| Low              | Regex validation, TLD check, MX records, single MX response |
| Medium           | Adds SPF, DMARC, Disposable domain check |
| High             | Adds Whitelist domain check, Vulgar word check, DKIM record |

---

## ⚙️ Tech Stack

- **Framework**: ASP.NET Core Web API
- **DNS Queries**: DnsClient
- **Email Protocols**: SMTP via TCP/SSL sockets
- **Caching**: IMemoryCache
- **Rate Limiting**: System.Threading.RateLimiting
- **Dependency Injection**: Fully modular service registration

---

## 📂 Project Structure

```
📦EmailVerificationAPI
 ┣ 📁Controllers
 ┃ ┗ 📄EmailVerificationController.cs
 ┣ 📁Models
 ┃ ┣ 📄RequestDTO.cs
 ┃ ┣ 📄ResponseDTO.cs
 ┃ ┣ 📄ChecklistElementDTO.cs
 ┃ ┗ 📄EmailStatusCode.cs
 ┣ 📁Services
 ┃ ┣ 📄DomainVerification.cs
 ┃ ┣ 📄SmtpServerVerification.cs
 ┃ ┣ 📄DisposableDomainsCheck.cs
 ┃ ┣ 📄TopLevelDomainVerification.cs
 ┃ ┣ 📄VulgarWordSearch.cs
 ┃ ┗ 📄WhiteListedEmailProvider.cs
 ┣ 📄Program.cs
 ┗ 📄IpRateLimitStore.cs
```

---

## 📥 API Usage

### Endpoint
```
POST /verify
```

### Request Body
```json
[
  {
    "email": "example@domain.com",
    "timeout": 3000,
    "strictness": 2
  }
]
```

### Sample Response
```json
{
  "emailAddress": "example@domain.com",
  "status": true,
  "totalScore": 90,
  "checklistElements": [
    {
      "name": "IsValidRegex",
      "weightageAllocated": 10,
      "obtainedScore": 10,
      "isVerified": "Valid"
    },
    ...
  ]
}
```

---

## 🚀 How to Run

1. Clone the repo:
```bash
git clone https://github.com/yourusername/EmailVerificationAPI.git
```

2. Restore dependencies:
```bash
dotnet restore
```

3. Run the project:
```bash
dotnet run
```

4. Test on Swagger at: `https://localhost:<port>/swagger`

---

## 🧠 Future Enhancements

- Export results to CSV or DB
- Web dashboard for email verification
- Better SMTP fallback and TLS upgrade support
- Admin panel for uploading vulgar/disposable/whitelist lists

---

## 🧑‍💻 Author

Built with ❤️ by [Your Name]

---
