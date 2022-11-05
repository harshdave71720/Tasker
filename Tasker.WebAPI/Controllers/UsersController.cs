using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tasker.Application.Repositories;
using Tasker.Core.Aggregates.UserAggregate;
using Tasker.WebAPI.Models;
using Newtonsoft.Json;

namespace Tasker.WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        { 
            _userRepository = userRepository;
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var user = await _userRepository.Get(id);
            if (user == null)
                return NotFound();
            return Ok(new Response<User>(user));
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAll();
            System.Console.WriteLine(JsonConvert.SerializeObject(users));
            return Ok(new Response<IEnumerable<User>>(users));
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userRepository.Get(id);
            if (user == null)
                return NotFound();
            await _userRepository.Delete(user.Id);
            return Ok(new Response<User>(user));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Update(int id, UpdateUserDto userDto)
        {
            var user = await _userRepository.Get(id);
            if (user == null)
                return NotFound();
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.WorkerStatus = userDto.WorkerStatus;
            return Ok(new Response<User>(await _userRepository.Save(user)));
        }
    }
}
