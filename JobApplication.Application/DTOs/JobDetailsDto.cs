namespace JobApplication.Application.DTOs
{
    public class JobDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int RecruiterId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
    }
}
