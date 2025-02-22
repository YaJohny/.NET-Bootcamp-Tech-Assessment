using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Entities;
using DataAccess.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization; // your DbContext namespace

namespace YourNamespace.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class BooksController : ControllerBase
	{
		private readonly BookDB _db;

		public BooksController(BookDB db)
		{
			_db = db;
		}


		[HttpGet("books")]
		public async Task<ActionResult<IEnumerable<Book>>> GetBooks(
	[FromQuery] int pageNumber = 1,
	[FromQuery] int pageSize = 10)
		{
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 10;

			var books = await _db.Books
				.Where(b => !b.IsDeleted)
				.OrderBy(b => b.Title) // Sorting by title (you can modify this)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.AsNoTracking()
				.ToListAsync();

			return Ok(new
			{
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalRecords = await _db.Books.CountAsync(b => !b.IsDeleted),
				Data = books
			});
		}


		[HttpGet("{id}")]
		public async Task<ActionResult<Book>> GetBook(int id)
		{
			var book = await _db.Books.FindAsync(id);

			// Check if book is found and not deleted
			if (book == null || book.IsDeleted)
			{
				return NotFound(new { message = "Book not found" });
			}

			// Increment the ViewCount each time details are accessed
			book.ViewCount++;
			await _db.SaveChangesAsync();

			// Return details with popularity score
			

			return Ok(book);
		}

		[HttpGet("ranking")]
		public async Task<ActionResult<IEnumerable<string>>> GetRanking(
	[FromQuery] int pageNumber = 1,
	[FromQuery] int pageSize = 10)
		{
			if (pageNumber < 1) pageNumber = 1;
			if (pageSize < 1) pageSize = 10;

			int currentYear = DateTime.UtcNow.Year;
			var titles = await _db.Books
				.Where(b => !b.IsDeleted)
				.OrderByDescending(b => b.ViewCount + ((currentYear - b.PublicationYear) * 2)) // Dynamic Sorting
				.Select(b => b.Title)
				.Skip((pageNumber - 1) * pageSize)
				.Take(pageSize)
				.ToListAsync();

			//way 2 client calculation
			//var titles =  _db.Books
			//	.Where(b => !b.IsDeleted)
			//	.AsNoTracking()
			//	.AsEnumerable() 
			//	.OrderByDescending(b => b.PopularityScore)
			//	.Select(b => b.Title)
			//  .Skip((pageNumber - 1) * pageSize)
			//	.ToList();

			return Ok(new
			{
				PageNumber = pageNumber,
				PageSize = pageSize,
				TotalRecords = await _db.Books.CountAsync(b => !b.IsDeleted),
				Data = titles
			});
		}


		[Authorize(Roles = "admin")]
		[HttpPost("single")]
		public async Task<IActionResult> CreateBook([FromBody] BookRequest request)
		{
			if (await _db.Books.AnyAsync(b => b.Title == request.Title && !b.IsDeleted))
			{
				return BadRequest(new { message = "A book with this title already exists." });
			}

			var book = new Book
			{
				Title = request.Title,
				PublicationYear = request.PublicationYear,
				AuthorName = request.AuthorName
			};

			var validationContext = new ValidationContext(book, serviceProvider: null, items: null);
			var validationResults = new List<ValidationResult>();
			var isValid = Validator.TryValidateObject(book, validationContext, validationResults, validateAllProperties: true);

			if (!isValid)
			{
				var errors = validationResults.Select(r => r.ErrorMessage).ToList();
				return BadRequest(new { message = errors });
			}

			await _db.AddAsync(book);
			await _db.SaveChangesAsync();

			
			return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
		}

		[Authorize(Roles = "admin")]
		[HttpPost("bulk")]
		public async Task<ActionResult<IEnumerable<Book>>> CreateBooksBulk([FromBody] List<BookRequest> request)
		{

			var incomingTitles = request.Select(b => b.Title).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();

			var existingTitles = await _db.Books
				.Where(b => incomingTitles.Contains(b.Title) && !b.IsDeleted)
				.Select(b => b.Title)
				.ToListAsync();

			var duplicates = existingTitles.Intersect(incomingTitles).ToList();
			if (duplicates.Any())
			{
				return BadRequest(new { message = $"The following title(s) already exist: {string.Join(", ", duplicates)}" });
			}

			List<Book>? booksToAdd = [];
			foreach (var bookToCheck in request)
			{
				var book = new Book
				{
					Title = bookToCheck.Title,
					PublicationYear = bookToCheck.PublicationYear,
					AuthorName = bookToCheck.AuthorName
				};
				var validationContext = new ValidationContext(book, serviceProvider: null, items: null);
				var validationResults = new List<ValidationResult>();
				var isValid = Validator.TryValidateObject(book, validationContext, validationResults, validateAllProperties: true);

				if (!isValid)
				{
					var errors = validationResults.Select(r => r.ErrorMessage).ToList();
					return BadRequest(new { message = errors, book = book});
				}
				booksToAdd.Add(book);
			}

			_db.Books.AddRange(booksToAdd);
			await _db.SaveChangesAsync();

			return Created(string.Empty, booksToAdd);
		}

		[Authorize(Roles = "admin")]
		[HttpPut("{id}")]
		public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest request)
		{
			var book = await _db.Books.FindAsync(id);
			if (book == null || book.IsDeleted)
			{
				return NotFound(new { message = "Book not found" });
			}

			if (book.Title != request.Title)
			{
				bool exists = await _db.Books
					.AnyAsync(b => b.Title == request.Title && !b.IsDeleted && b.Id != id);
				if (exists)
				{
					return BadRequest(new { message = "A book with this title already exists." });
				}
			}

			if (request.Title != null) book.Title = request.Title;
			if (request.PublicationYear != null) book.PublicationYear = (int)request.PublicationYear;
			if (request.AuthorName != null) book.AuthorName = request.AuthorName;

			var validationContext = new ValidationContext(book, serviceProvider: null, items: null);
			var validationResults = new List<ValidationResult>();
			var isValid = Validator.TryValidateObject(book, validationContext, validationResults, validateAllProperties: true);

			if (!isValid)
			{
				var errors = validationResults.Select(r => r.ErrorMessage).ToList();
				return BadRequest(new { message = errors });
			}


			_db.Books.Update(book);
			await _db.SaveChangesAsync();

			return NoContent();
		}

		[Authorize(Roles = "admin")]
		[HttpDelete("{id}")]
		public async Task<IActionResult> SoftDeleteBook(int id)
		{
			var book = await _db.Books.FindAsync(id);
			if (book == null || book.IsDeleted)
			{
				return NotFound(new { message = "Book not found or already deleted" });
			}

			book.IsDeleted = true;
			_db.Books.Update(book);
			await _db.SaveChangesAsync();
			return NoContent();
		}

		[Authorize(Roles = "admin")]
		[HttpDelete("bulk")]
		public async Task<IActionResult> SoftDeleteBooks([FromBody] List<int> ids)
		{
			var books = await _db.Books
				.Where(b => ids.Contains(b.Id) && !b.IsDeleted)
				.ToListAsync();

			if (!books.Any())
			{
				return NotFound(new { message = "No matching books found to delete." });
			}

			foreach (var b in books)
			{
				b.IsDeleted = true;
			}

			_db.Books.UpdateRange(books);
			await _db.SaveChangesAsync();

			return NoContent();
		}
	}

	public record BookRequest(string Title, int PublicationYear, string? AuthorName);
	public record UpdateBookRequest(string? Title, int? PublicationYear, string? AuthorName);
	
}
