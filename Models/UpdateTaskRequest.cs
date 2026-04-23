namespace Web_API_Quiz.Models
{
    public class UpdateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
