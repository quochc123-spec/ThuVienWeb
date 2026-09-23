using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ThuVienWeb.Data;
using ThuVienWeb.Models.Domain;
using ThuVienWeb.Models.DTO;

namespace ThuVienWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        public BookController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        //GET: api/Book/get-all-books
        [HttpGet("get-all-books")]
        public IActionResult GetAll()
        {
            var allBooksDomain = _dbContext.Books;
            var allBooksDTO = allBooksDomain.Select(Books => new BookWithAuthorAndPublisherDTO()
            {
                Id = Books.Id,
                Title = Books.Title,
                Description = Books.Description,
                IsRead = Books.IsRead,
                DateRead = Books.IsRead ? Books.DateRead.Value : null,
                Rate = Books.Rate,
                Genre = Books.Genre,
                CoverUrl = Books.CoverUrl,
                PublisherName = Books.Publisher.Name,
                AuthorNames = Books.Book_Authors.Select(n => n.Author.FullName).ToList()
            }).ToList();
            return Ok(allBooksDTO);
        }
        [HttpGet]
        [Route("get-book-by-id/{id:int}")]
        public IActionResult GetBookById([FromRoute] int id)
        {
            var bookDomain = _dbContext.Books.Include(b => b.Publisher).Include(b => b.Book_Authors).
            ThenInclude(ba => ba.Author).FirstOrDefault(b => b.Id == id); ;

            if (bookDomain == null)
            {
                return NotFound();
            }
            var bookDTO = new Models.DTO.BookWithAuthorAndPublisherDTO()
            {
                Id = bookDomain.Id,
                Title = bookDomain.Title,
                Description = bookDomain.Description,
                IsRead = bookDomain.IsRead,
                DateRead = bookDomain.IsRead ? bookDomain.DateRead.Value : null,
                Rate = bookDomain.Rate,
                Genre = bookDomain.Genre,
                CoverUrl = bookDomain.CoverUrl,
                DateAdded = bookDomain.DateAdded,
                PublisherName = bookDomain.Publisher != null ? bookDomain.Publisher.Name : "Unknown",
                AuthorNames = bookDomain.Book_Authors?.Where(y => y.Author != null).Select(y => y.Author.FullName).ToList() ?? new List<string>()
            };
            return Ok(bookDTO);
        }
        [HttpPost("add-book")]
        public IActionResult AddBook([FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            var publisherDomain = _dbContext.Publishers.FirstOrDefault(x => x.Id == addBookRequestDTO.PublisherId);
            if (publisherDomain == null)
            {
                return NotFound(new { message = "No publisher found with the provided ID." });
            }

            var bookDomain = new Models.Domain.Books()
            {
                Title = addBookRequestDTO.Title,
                Description = addBookRequestDTO.Description,
                IsRead = addBookRequestDTO.IsRead,
                DateRead = addBookRequestDTO.DateRead,
                Rate = addBookRequestDTO.Rate,
                Genre = addBookRequestDTO.Genre,
                CoverUrl = addBookRequestDTO.CoverUrl,
                DateAdded = addBookRequestDTO.DateAdded,
                PublisherId = publisherDomain.Id
            };
            _dbContext.Books.Add(bookDomain);
            _dbContext.SaveChanges();
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == authorId);
                if (authorDomain == null)
                {
                    return NotFound(new { message = $"No author found with the provided ID: {authorId}." });
                }
                var bookAuthorDomain = new Models.Domain.Book_Author()
                {
                    BookId = bookDomain.Id,
                    AuthorId = authorDomain.Id
                };
                _dbContext.Book_Authors.Add(bookAuthorDomain);
                _dbContext.SaveChanges();
            }
            return Ok();
        }
        [HttpPut("update-book-by-id/{id:int}")]
        public IActionResult UpdateBookById(int id, [FromBody] AddBookRequestDTO addBookRequestDTO)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(x => x.Id == id);
            if (bookDomain == null)
            {
                return NotFound(new { message = "No book found with the provided ID." });
            }
            else
            {
                bookDomain.Title = addBookRequestDTO.Title;
                bookDomain.Description = addBookRequestDTO.Description;
                bookDomain.IsRead = addBookRequestDTO.IsRead;
                bookDomain.DateRead = addBookRequestDTO.DateRead;
                bookDomain.Rate = addBookRequestDTO.Rate;
                bookDomain.Genre = addBookRequestDTO.Genre;
                bookDomain.CoverUrl = addBookRequestDTO.CoverUrl;
                bookDomain.DateAdded = addBookRequestDTO.DateAdded;
                bookDomain.PublisherId = addBookRequestDTO.PublisherId;
                _dbContext.SaveChanges();
            }
            var existingBookAuthors = _dbContext.Book_Authors.Where(x => x.BookId == id).ToList();
            if (existingBookAuthors != null && existingBookAuthors.Count > 0)
            {
                _dbContext.Book_Authors.RemoveRange(existingBookAuthors);
                _dbContext.SaveChanges();
            }
            foreach (var authorId in addBookRequestDTO.AuthorIds)
            {
                var authorDomain = _dbContext.Authors.FirstOrDefault(x => x.Id == authorId);
                if (authorDomain == null)
                {
                    return NotFound(new { message = $"No author found with the provided ID: {authorId}." });
                }
                var bookAuthorDomain = new Book_Author()
                {
                    BookId = bookDomain.Id,
                    AuthorId = authorDomain.Id
                };
                _dbContext.Book_Authors.Add(bookAuthorDomain);
                _dbContext.SaveChanges();
            }
            return Ok(addBookRequestDTO);
        }
        [HttpDelete("delete-book-by-id/{id:int}")]
        public IActionResult DeleteBookById(int id)
        {
            var bookDomain = _dbContext.Books.FirstOrDefault(x => x.Id == id);
            if (bookDomain == null)
            {
                return NotFound(new { message = "No book found with the provided ID." });
            }
            var existingBookAuthors = _dbContext.Book_Authors.Where(x => x.BookId == id).ToList();
            if (existingBookAuthors != null && existingBookAuthors.Count > 0)
            {
                _dbContext.Book_Authors.RemoveRange(existingBookAuthors);
                _dbContext.SaveChanges();
            }
            _dbContext.Books.Remove(bookDomain);
            _dbContext.SaveChanges();
            return Ok(bookDomain);
        }
    }
}

