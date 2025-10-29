# UserManagementOtpVerfiyApp

Important security & production notes (read carefully)

OTP storage: In this simple sample OTP is stored as plain text. In production, store a hash of OTP or use a short-lived cache (Redis) and delete after use. This prevents DB leaks from exposing valid OTPs.

ReturnOtpInResponse should be false in production.

OTP expiry of 15 seconds is very short for real users (but you asked). Typical expiry is 2–5 minutes. If you truly want 15 seconds mimic some microflow or test.

Rate limiting: Add rate-limits per mobile and IP to prevent OTP flood. Use libraries or API gateway.

SMS provider costs: Real SMS costs money — use Twilio or a local provider; test with their sandbox numbers.

Clock skew: We used ClockSkew = TimeSpan.Zero. Ensure server clocks are synced (NTP).

JWT key: Store secret securely (Azure Key Vault / environment variables), not in source.

Quick tips & tweaks

To change expiry: update Otp:ExpirySeconds in appsettings.json.

To change DB to SQL Server: replace UseSqlite with UseSqlServer(config.GetConnectionString("SqlServerConnection")) and add package Microsoft.EntityFrameworkCore.SqlServer.

To disable returning OTP: set "Otp:ReturnOtpInResponse": "false".
