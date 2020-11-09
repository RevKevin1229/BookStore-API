using AutoMapper;
using BookStore_API.Contracts;
using BookStore_API.Data;
using BookStore_API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore_API.Controllers
{
    /// <summary>
    /// Endpoint used to interact with th Authors in the book store's database
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [ProducesResponseType(StatusCodes.Status200OK)]

    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;

        private readonly ILoggerService _logger;

        private readonly IMapper _mapper;

        public BooksController(IBookRepository bookRepository,
        ILoggerService logger, IMapper mapper)
        {
            _bookRepository = bookRepository;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// Get All Books
        /// </summary>
        /// <returns>A List of Books</returns>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBooks()
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Attempting to get all Books.");
                var books = await _bookRepository.FindAll();
                var response = _mapper.Map<IList<BookDTO>>(books);
                _logger.LogInfo($"{location}: Successfully got all Books.");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Gets a particular book according to id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Book Record</returns>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBook(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Attempting to get a Book with id: {id}");
                var book = await _bookRepository.FindById(id);

                if (book == null)
                {
                    _logger.LogWarn($"{location}: Book with id: {id} not found.");
                    return NotFound();
                }

                var response = _mapper.Map<IList<BookDTO>>(book);
                _logger.LogInfo($"{location}: Successfully got Book with id: {id}");
                return Ok(response);
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Creates a New Book
        /// </summary>
        /// <param name="bookDTO"></param>
        /// <returns>Book Object</returns>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Create([FromBody] BookCreateDTO bookDTO)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogWarn($"Submitting Book request.");

                if (bookDTO ==null)
                {
                    _logger.LogWarn($"{location}: Empty Book request was submitted.");
                    return BadRequest(ModelState);
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Book Data submitted was incomplete.");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Create(book);

                if (isSuccess)
                {

                    _logger.LogInfo($"{location}: Book created.");
                    return Created("Create", new { book });
                }
                else
                {
                    return InternalError($"{location}: Creation of Book failed.");
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Update a Book
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bookDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Update(int id, [FromBody] BookUpdateDTO bookDTO)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Submitting Book Update request for id:{id}.");

                if (id < 1 || bookDTO == null || id != bookDTO.Id)
                {
                    _logger.LogWarn($"{location}: Book Update request for id:{id} failed because of bad Data.");
                    return BadRequest();
                }

                var findBook = await _bookRepository.FindById(id);

                if (findBook == null)
                {
                    _logger.LogWarn($"{location}: Update Book with an id: {id} could not be found.");
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarn($"{location}: Book Update with an id: {id} data was incomplete.");
                    return BadRequest(ModelState);
                }

                var book = _mapper.Map<Book>(bookDTO);
                var isSuccess = await _bookRepository.Update(book);

                if (isSuccess)
                {
                    _logger.LogInfo($"{location}: Book with id: {id} was successfully Updated.");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarn($"{location}: Updating Book with id: {id} failed.");
                    return InternalError($"Update Book Operation failed.");
                }
            }
            catch (Exception e)
            {
                return InternalError($"{location}: {e.Message} - {e.InnerException}");
            }
        }

        /// <summary>
        /// Delete a Book
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            var location = GetControllerActionNames();

            try
            {
                _logger.LogInfo($"{location}: Deleting Book with id: {id} ");
                if (id < 1)
                {
                    _logger.LogError($"{location}: Delete Book with id: {id} failed because of bad data.");
                    return BadRequest();
                }

                var findBook = await _bookRepository.FindById(id);

                if (findBook == null)
                {
                    _logger.LogError($"{location}: Removing Book with an id: {id} could not be found.");
                    return NotFound();
                }

                var book = await _bookRepository.FindById(id);

                if (book == null)
                {
                    _logger.LogError($"{location}: Delete Book with id: {id} could not be found.");
                    return NotFound();
                }

                var isSuccess = await _bookRepository.Delete(book);

                if (isSuccess)
                {
                    _logger.LogInfo($"{location}: The Book with id: {id} was successfully removed.");
                    return NoContent();
                }
                else
                {
                    return InternalError($"{location}: Book with id: {id} could not be deleted.");
                }
            }
            catch (Exception e)
            {
                _logger.LogInfo($"{location}: Deleted Book with id: {id} .");
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
