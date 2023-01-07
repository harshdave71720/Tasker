using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Tasker.Core.Constants;

namespace Tasker.WebAPI.Models
{
    public class CreateTaskModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        public WorkerOrderingScheme? OrderingScheme { get; set; }

        [Required]
        public List<int> WorkerIds { get; set; }

        [Required]
        public int CurrentWorkerId { get; set; }
    }
}
