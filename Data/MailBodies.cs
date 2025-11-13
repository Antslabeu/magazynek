using System.Net;
using System.Text.Encodings.Web;
using Magazynek.Entities.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Magazynek.Data.Mailer
{
  public static class MailBodies
  {
    static string GenerateHeader(string title1, string title2)
    {
      return $@"<!doctype html>
        <html lang=""pl"">
        <head>
        <meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>{title1}</title>
        </head>
        <body style=""margin:0;padding:0;background:#0b0f14;color:#e5e7eb;font-family:Inter,Segoe UI,Roboto,Arial,sans-serif;"">
          <!-- preheader (ukryty w wielu klientach) -->
          <div style=""display:none;max-height:0;overflow:hidden;opacity:0;"">{title2}</div>

          <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""background:#0b0f14;"">
            <tr><td align=""center"" style=""padding:24px 12px;"">
              <table role=""presentation"" width=""600"" cellspacing=""0"" cellpadding=""0"" border=""0"" style=""max-width:600px;width:100%;"">
                <tr>
                  <td style=""padding:0 8px 16px 8px;text-align:center;color:#9ca3af;font-size:12px;"">
                    Jeśli nie widzisz przycisku, skopiuj link z dołu wiadomości.
                  </td>
                </tr>
                <tr>
                  <td style=""padding:0 8px 24px 8px;"">
                    <table role=""presentation"" width=""100%"" cellspacing=""0"" cellpadding=""0"" border=""0""
                          style=""background:#0f1624; 
                                  background-image:linear-gradient(135deg, rgba(255,255,255,0.06), rgba(255,255,255,0) 40%);
                                  border:1px solid #1f2a37;border-radius:14px;"">
                      <tr>
                        <td style=""padding:24px 24px 8px 24px;"">
                          <div style=""display:inline-flex;align-items:center;gap:10px;"">
                            <span style=""display:inline-block;width:10px;height:10px;border-radius:999px;background:#3b82f6;""></span>
                            <span style=""color:#ffffff;font-weight:700;font-size:18px;"">Magazynek</span>
                          </div>
                        </td>
                      </tr>";
    }
    static string GenerateFooter()
    {
      return $@"<tr>
                <td style=""padding:8px 24px 24px 24px;border-top:1px solid #1f2a37;"">
                  <p style=""margin:12px 0 0 0;color:#64748b;font-size:11px;"">
                    Jeśli to nie Ty jesteś adresatem, zignoruj tę wiadomość.
                  </p>
                </td>
              </tr>
            </table>
          </td>
        </tr>
        <tr>
          <td style=""padding:0 8px;text-align:center;color:#475569;font-size:11px;"">
            © {DateTime.UtcNow:yyyy} Magazynek • mail.antslab.eu
          </td>
        </tr>
      </table>
    </td></tr>
  </table>
</body>
</html>";
    }
    public static string BuildActivationEmailHtml(NewAccountModel accountModel, Guid activatorGuid, NavigationManager nav)
    {
      var serverAddress = nav.BaseUri;
      var enc = HtmlEncoder.Default;
      var safeName = enc.Encode(accountModel?.Name ?? "");
      var safeLogin = enc.Encode(accountModel?.Login ?? "");
      var token = activatorGuid.ToString("N");
      var link = $"{serverAddress.TrimEnd('/')}/new-account/activate?code={token}&login={WebUtility.UrlEncode(accountModel?.Login ?? "")}";
      var safeLink = enc.Encode(link);

      string htmlContent = GenerateHeader("Aktywuj konto", "Aktywuj swoje konto w Magazynek") +
      $@"<tr>
          <td style=""padding:8px 24px 0 24px;"">
            <h1 style=""margin:0 0 10px 0;color:#ffffff;font-size:20px;line-height:1.35;"">Aktywuj konto</h1>
            <p style=""margin:0 0 12px 0;color:#cbd5e1;font-size:14px;line-height:1.6;"">
              Cześć <strong style=""color:#ffffff;"">{safeName}</strong>, utworzyliśmy konto dla <strong style=""color:#ffffff;"">{safeLogin}</strong>.
            </p>
            <p style=""margin:0 0 20px 0;color:#94a3b8;font-size:13px;line-height:1.6;"">
              Aby dokończyć rejestrację, kliknij przycisk poniżej.
            </p>
          </td>
        </tr>
        <tr>
          <td style=""padding:0 24px 24px 24px;"" align=""left"">
            <a href=""{safeLink}""
                style=""display:inline-block;padding:12px 18px;border-radius:12px;
                      background:#3b82f6;background-image:linear-gradient(180deg,#3b82f6,#2563eb);
                      color:#ffffff;text-decoration:none;font-weight:700;font-size:14px;"">
              Aktywuj konto
            </a>
          </td>
        </tr>
        <tr>
          <td style=""padding:0 24px 16px 24px;"">
            <p style=""margin:0;color:#94a3b8;font-size:12px;line-height:1.6;"">
              Jeśli przycisk nie działa, skopiuj ten adres do przeglądarki:
            </p>
            <p style=""margin:6px 0 0 0;word-break:break-all;color:#7aa2ff;font-size:12px;line-height:1.5;"">{safeLink}</p>
          </td>
        </tr>" +
        GenerateFooter();

      return htmlContent;
    }

    public static string BuildRevertPasswordHtml(string name, string login, Guid activatorGuid, NavigationManager nav)
    {
      var serverAddress = nav.BaseUri;
      var enc = HtmlEncoder.Default;
      var safeName = enc.Encode(name);
      var safeLogin = enc.Encode(login);
      var token = activatorGuid.ToString("N");
      var link = $"{serverAddress.TrimEnd('/')}/new-account/revert-password?code={token}&login={WebUtility.UrlEncode(login)}";
      var safeLink = enc.Encode(link);

      string htmlContent = GenerateHeader("Resetuj hasło", "Resetuj hasło w Magazynek") +
      $@"<tr>
        <td style=""padding:8px 24px 0 24px;"">
          <h1 style=""margin:0 0 10px 0;color:#ffffff;font-size:20px;line-height:1.35;"">Ustaw nowe hasło</h1>
          <p style=""margin:0 0 12px 0;color:#cbd5e1;font-size:14px;line-height:1.6;"">
            Cześć <strong style=""color:#ffffff;"">{safeName}</strong>, otrzymaliśmy prośbę o zresetowanie hasła dla
            <strong style=""color:#ffffff;"">{safeLogin}</strong>.
          </p>
          <p style=""margin:0 0 20px 0;color:#94a3b8;font-size:13px;line-height:1.6;"">
            Kliknij przycisk poniżej, aby przejść do ustawienia nowego hasła.
          </p>
        </td>
      </tr>
      <tr>
        <td style=""padding:0 24px 24px 24px;"" align=""left"">
          <a href=""{safeLink}""
            style=""display:inline-block;padding:12px 18px;border-radius:12px;
                    background:#3b82f6;background-image:linear-gradient(180deg,#3b82f6,#2563eb);
                    color:#ffffff;text-decoration:none;font-weight:700;font-size:14px;"">
            Zresetuj hasło
          </a>
        </td>
      </tr>
      <tr>
        <td style=""padding:0 24px 16px 24px;"">
          <p style=""margin:0;color:#94a3b8;font-size:12px;line-height:1.6;"">
            Jeśli przycisk nie działa, skopiuj ten adres do przeglądarki:
          </p>
          <p style=""margin:6px 0 0 0;word-break:break-all;color:#7aa2ff;font-size:12px;line-height:1.5;"">{safeLink}</p>
        </td>
      </tr>"
      + GenerateFooter();

      return htmlContent;
    }
  }
}