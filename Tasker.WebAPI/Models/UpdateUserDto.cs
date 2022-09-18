using System.ComponentModel.DataAnnotations;
using Tasker.Core.Constants;

namespace Tasker.WebAPI.Models
{
    public class UpdateUserDto
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }
        
        [Required]
        public WorkerStatus WorkerStatus { get; set; }
    }
}
