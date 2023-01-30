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
    public class TaskController : TaskerControllerBase
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

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = HttpContext.User;
            var task = await _taskRepository.Get(id);
            if(task == null)
                return NotFound();
            return Ok(task);
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> Get()
        {
            var tasks = await _taskRepository.GetAll();   
            return Ok(tasks);
        }

        [HttpPut]
        [Route("")]
        public async Task<IActionResult> Update()
        {
            return null;
        }

        [HttpPost]
        [Route("{id}/Done")]
        public async Task<IActionResult> Done(int id)
        {
            if (!(await _taskRepository.Exists(id)))
                return NotFound();
            var task = await _taskRepository.Get(id);
            task.MarkDone();
            await _taskRepository.Save(task);
            return Ok(task);
        }

        [HttpPost]
        [Route("{id}/Skip")]
        public async Task<IActionResult> Skip(int id)
        {
            if (!(await _taskRepository.Exists(id)))
                return NotFound();
            var task = await _taskRepository.Get(id);
            task.Skip();
            await _taskRepository.Save(task);
            return Ok(task);
        }
    }
}
