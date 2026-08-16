namespace Luxira.Application.Interfaces;

// The per-poll-cycle business logic for Module B's auto-reply background
// job, split out from AutoReplyBackgroundService itself so it's testable the
// same way as every other Service class (mocked IUnitOfWork) rather than
// needing a real DI scope/timer in a test.
public interface IAutoReplyProcessor
{
    Task ProcessDueRepliesAsync();
}
