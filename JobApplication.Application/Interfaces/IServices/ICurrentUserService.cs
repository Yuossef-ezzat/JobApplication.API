namespace JobApplication.Application.Interfaces.IServices
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        bool IsAuthenticated { get; }
        bool IsRecruiter { get; }
        bool IsCandidate { get; }
    }
}
