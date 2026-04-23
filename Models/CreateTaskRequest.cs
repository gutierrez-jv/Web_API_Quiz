namespace Web_API_Quiz.Models
{
    public class CreateTaskRequest
    {
        public string Title { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
    }
}
