namespace Luxira.Application.Interfaces;

// Abstracts email sending (SMTP relay today, swappable for a different
// transactional provider later) behind a plain contract - no provider-specific
// types leak into this layer. Implementations must never throw on send
// failure - a failed email must never fail the caller's primary flow (e.g.
// checkout); implementations log and swallow instead, same "degrade
// gracefully" pattern already used by IGeoIpLookup when its data file is
// missing.
public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody);
}
