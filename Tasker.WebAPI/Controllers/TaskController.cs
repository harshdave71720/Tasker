using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Tasker.Application.Repositories;
using Tasker.WebAPI.Models;
using System.Linq;
using System.Collections.Generic;
using TaskAggregate = Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.Aggregates.TaskAggregate;
using Tasker.Core.TaskWorkerOrdering.Factories;
using Microsoft.AspNetCore.Authorization;

namespace Tasker.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class TaskController : ControllerBase
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IWorkerOrdererFactory _workerOrdererFactory;

        public TaskController(ITaskRepository taskRepository, IUserRepository userRepository, IWorkerOrdererFactory workerOrdererFactory)
        { 
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _workerOrdererFactory = workerOrdererFactory;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> Create(CreateTaskModel taskModel)
        {
            var workers = (await _userRepository.GetAll(taskModel.WorkerIds))
                            .Select(u => new TaskWorker(u.Id, u.FirstName, u.LastName, u.WorkerStatus))
                            .ToHashSet();
            if (!workers.Any())
                return BadRequest();
            var currentWorker = (await _userRepository.Get(taskModel.CurrentWorkerId));
            if (currentWorker == null)
                return BadRequest();
            var workerPool = new WorkerPool(
                workers, 
                _workerOrdererFactory, 
                taskModel.OrderingScheme ?? Core.Constants.WorkerOrderingScheme.AscendingNameScheme
            );
            var task = new TaskAggregate.Task(
                taskModel.Name, 
                workerPool, 
                new TaskWorker(currentWorker.Id, currentWorker.FirstName, currentWorker.LastName, currentWorker.WorkerStatus)
            );
            await _taskRepository.Save(task);
            return Ok(new Response<TaskAggregate.Task>(task));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        { 
            await _taskRepository.Delete(id);
            return Ok();
        }
    }
}
