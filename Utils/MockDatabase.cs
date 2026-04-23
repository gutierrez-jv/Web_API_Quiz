using Web_API_Quiz.Models;

namespace Web_API_Quiz.Utils
{
    public class MockDatabase
    {
        public static class MockData {
            public static List<User> Users = new()
        {
            new User { Id = 1, Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Id = 2, Username = "user", Password = "user123", Role = "User" },
        };

            public static List<TaskItem> Tasks = new()
        {
            new TaskItem { Id = 1, Title = "Sample Task 1", IsCompleted = true },
            new TaskItem { Id = 2, Title = "Sample Task 2", IsCompleted = false }
        };
        }
    }
}
