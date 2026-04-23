namespace Web_API_Quiz.Models
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;
    }
}
