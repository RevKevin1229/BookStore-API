using System;
using System.Collections.Generic;
using System.Composition.Convention;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with th Authors in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    // Use this when all functions need to be authorized.
    //[Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorRepository _authorRepository;

        private readonly ILoggerService _logger;

        private readonly IMapper _mapper;

        public AuthorsController(IAuthorRepository authorRepository,
            ILoggerService logger, IMapper mapper)
        {
            _authorRepository = authorRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Authors
        /// </summary>
        /// <returns>List of Authors</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthors()
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Retrieving all Authors.");
                var authors = await _authorRepository.FindAll();
                var response = _mapper.Map<IList<AuthorDTO>>(authors);
                _logger.LogInfo($"{location}: Successfully got all Authors.");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Get an Author by Id
        /// </summary>
        /// <returns>Author</returns>
        [HttpGet("id")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAuthor(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Retrieving an Author with id: {id}");
                var author = await _authorRepository.FindById(id);

                if(author == null)
                {
                    _logger.LogWarn($"{location}: Author with id: {id} not found.");
                    return NotFound();
                }

                var response = _mapper.Map<AuthorDTO>(author);
                _logger.LogInfo($"{location}: Successfully got Author with id: {id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Creates an Author
        /// </summary>
        /// <param name="authorDTO"></param>
        /// <returns>Author Object</returns>
        [HttpPost]
        [Authorize(Roles ="Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] AuthorCreateDTO authorDTO)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogWarn($"Submitting Author request.");

                if (authorDTO == null)
                {
                    _logger.LogWarn($"{location}: Empty Author request was submitted.");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Author Data submitted was incomplete.");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Create(author);

                if (isSuccess)
                {
                    _logger.LogInfo($"{location}: Author created.");
                    return Created("Create", new { author });
                }
                else
                {
                    return InternalError($"{location}: Creation of Author failed.");
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Update an Author
        /// </summary>
        /// <param name="id"></param>
        /// <param name="authorDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] AuthorUpdateDTO authorDTO)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Submitting Author Update request for id:{id}.");

                if (id < 1 || authorDTO == null || id != authorDTO.Id)
                {
                    _logger.LogWarn($"{location}: Author Update for id:{id} failed because of bad Data.");
                    return BadRequest();
                }

                var findAuthor = await _authorRepository.FindById(id);

                if (findAuthor == null)
                {
                    _logger.LogWarn($"{location}: Update Author with an id: {id} could not be found.");
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Author Update data for id:{id} was incomplete.");
                    return BadRequest(ModelState);
                }

                var author = _mapper.Map<Author>(authorDTO);
                var isSuccess = await _authorRepository.Update(author);

                if (!isSuccess)
                {
                    _logger.LogWarn($"{location}: Updating Author with id: {id} failed.");
                    return InternalError($"Update Author Operation failed.");
                }

                _logger.LogInfo($"{location}: Author with id: {id} was successfully Updated.");
                return NoContent();
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Delete an Author
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Deleting Author with id: {id} ");
                if (id < 1)
                {
                    _logger.LogError($"{location}: Delete Author with id: {id} failed because of bad data.");
                    return BadRequest();
                }

                var findAuthor = await _authorRepository.FindById(id);

                if (findAuthor == null)
                {
                    _logger.LogError($"{location}: Removing Author with an id: {id} could not be found.");
                    return NotFound();
                }

                var author = await _authorRepository.FindById(id);

                if (author == null)
                {
                    _logger.LogError($"{location}: Delete Author with id: {id} could not be found.");
                    return NotFound();
                }

                var isSuccess = await _authorRepository.Delete(author);

                if (isSuccess)
                {
                    _logger.LogInfo($"{location}: The Author with id: {id} was successfully removed.");
                    return NoContent();
                }
                else
                {
                    return InternalError($"{location}: Author with id: {id} could not be deleted.");
                }
            }
            catch (Exception e)
            {
                _logger.LogInfo($"{location}: Deleted Author with id: {id} .");
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        private string GetControllerActionNames()
        {
            var controller = ControllerContext.ActionDescriptor.ControllerName;
            var action = ControllerContext.ActionDescriptor.ActionName;

            return $"{controller} - {action}";
        }

        private ObjectResult InternalError(string message)
        {
            _logger.LogError(message);
            return StatusCode(500, "Something went wrong. Please contact the System Administrator.");
        }
    }
}
