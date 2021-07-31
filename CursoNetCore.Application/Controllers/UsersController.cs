using AutoMapper;
using CursoNetCore.Domain.Dtos.User;
using CursoNetCore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CursoNetCore.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMapper _mapper;

        public UsersController(IUserService service, IMapper mapper)
        {
            _service = service;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> List()
        {
            var users = await _service.GetAll();
            return Ok(_mapper.Map<IEnumerable<UserDto>>(users));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> Get(Guid id)
        {
            var user = await _service.GetById(id);
            return Ok(_mapper.Map<UserDto>(user));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Post([FromBody] SaveUserDto saveUserDto)
        {
            var createdUser = await _service.Save(saveUserDto);
            return CreatedAtAction(nameof(Get), new { createdUser.Id }, _mapper.Map<UserDto>(createdUser));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            await _service.Update(id, updateUserDto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _service.Delete(id);
            return Ok();
        }
    }
}
