using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.Tasks;
using Web_API_Quiz.Models;
using static Web_API_Quiz.Utils.MockDatabase;

namespace Web_API_Quiz.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TasksController : ControllerBase
    {
        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        [EnableRateLimiting("tasksPolicy")]
        public IActionResult GetTasksById(int id)
        {
            var tasks = MockData.Tasks.FirstOrDefault(u => u.Id == id);

            if (tasks == null)
            {
                return NotFound("tasks not found.");
            }

            return Ok(tasks);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet()]
        [EnableRateLimiting("tasksPolicy")]
        public IActionResult GetTasks()
        {
            return Ok(MockData.Tasks);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [EnableRateLimiting("tasksPolicy")]
        public IActionResult DeleteTasks(int id)
        {
            var tasks = MockData.Tasks.FirstOrDefault(u => u.Id == id);

            if (tasks == null)
            {
                return NotFound("tasks not found.");
            }

            MockData.Tasks.Remove(tasks);
            return Ok("Tasks deleted successfully.");
        }

        [Authorize(Roles = "Admin,User")]
        [HttpPost]
        [EnableRateLimiting("tasksPolicy")]
        public IActionResult CreateTask([FromBody] CreateTaskRequest request)
        {
            var newTask = new TaskItem
            {
                Id = MockData.Tasks.Any() ? MockData.Tasks.Max(t => t.Id) + 1 : 1,
                Title = request.Title,
                IsCompleted = request.IsCompleted
            };

            MockData.Tasks.Add(newTask);

            return Ok(newTask);
        }

        [HttpPut("{id}")]
        [EnableRateLimiting("tasksPolicy")]
        public IActionResult UpdateTask(int id, [FromBody] UpdateTaskRequest request)
        {
            var task = MockData.Tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound("Task not found.");
            }

            task.Title = request.Title;
            task.IsCompleted = request.IsCompleted;

            return Ok(task);
        }
    }
}
