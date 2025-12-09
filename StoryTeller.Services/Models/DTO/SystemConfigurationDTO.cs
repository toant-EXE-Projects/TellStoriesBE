namespace StoryTeller.Services.Models.DTO
{
    public class SystemConfigurationDTO
    {
        public Guid Id { get; set; }
        public string Key { get; set; } = null!;
        public string Value { get; set; } = null!;

        public DateTime CreatedDate { get; set; }
        public UserMinimalDTO? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public UserMinimalDTO? UpdatedBy { get; set; }
    }
}
