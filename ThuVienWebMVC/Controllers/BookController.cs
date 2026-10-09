using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using ThuVienWebMVC.Models.DTO;

namespace ThuVienWebMVC.Controllers
{
    public class BookController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<BookController> logger;
        public BookController(IHttpClientFactory httpClientFactory, ILogger<BookController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }
        public async Task<IActionResult> Index([FromQuery] string filterOn = null, string filterQuery = null, string sortBy = null, bool? isAscending = true)
        {
            List<BookDTO> response = new List<BookDTO>(); // tạo đối tượng với model Book
         try        
         { 
            // Use the named HttpClient "Api" (BaseAddress configured in Program.cs)
            var client = httpClientFactory.CreateClient("Api"); //khởi tạo Client 
            // Call the API route defined in ThuVienWeb: api/Book/get-all-books
            var qFilterOn = Uri.EscapeDataString(filterOn ?? string.Empty);
            var qFilterQuery = Uri.EscapeDataString(filterQuery ?? string.Empty);
            var qSortBy = Uri.EscapeDataString(sortBy ?? string.Empty);
            var qIsAscending = (isAscending ?? true).ToString();
            var query = $"api/Book/get-all-books?filterOn={qFilterOn}&filterQuery={qFilterQuery}&sortBy={qSortBy}&isAscending={qIsAscending}";
            var httpResponseMess = await client.GetAsync(query);  // lấy dữ liệu Get books from API với url từ API 
            httpResponseMess.EnsureSuccessStatusCode(); // kiểm tra mã trạng thái trả về 200
            response.AddRange(await httpResponseMess.Content.ReadFromJsonAsync<IEnumerable<BookDTO>>());
            // đổi kiểu dữ liệu từ Json sang mảng đối tượng BookDTO 
         } 
     catch (Exception ex) 
       {
                // Log the full exception for troubleshooting
                logger.LogError(ex, "Error calling Books API");
                // Provide a short message to the Error view in development
                ViewData["ApiErrorMessage"] = ex.Message;
                return View("Error");
        } 
      return View(response); //truyền dữ liệu sang View thông qua biến response 

        }
        [HttpGet]
        public async Task<IActionResult> AddBook()
        {
            // Populate authors and publishers for the dropdowns
            try
            {
                var client = httpClientFactory.CreateClient("Api");
                var authorsResp = await client.GetAsync("api/Authors/get-all-author");
                var publishersResp = await client.GetAsync("api/Publishers/get-all-publisher");
                if (authorsResp.IsSuccessStatusCode)
                {
                    var authors = await authorsResp.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>();
                    ViewBag.listAuthor = authors ?? Enumerable.Empty<authorDTO>();
                }
                else
                {
                    ViewBag.listAuthor = Enumerable.Empty<authorDTO>();
                }
                if (publishersResp.IsSuccessStatusCode)
                {
                    var publishers = await publishersResp.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>();
                    ViewBag.listPublisher = publishers ?? Enumerable.Empty<publisherDTO>();
                }
                else
                {
                    ViewBag.listPublisher = Enumerable.Empty<publisherDTO>();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error populating AddBook dropdowns");
                ViewBag.listAuthor = Enumerable.Empty<authorDTO>();
                ViewBag.listPublisher = Enumerable.Empty<publisherDTO>();
                ViewBag.Error = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddBook(addBookDTO addBookDTO)
        {
            try
            {
                var client = httpClientFactory.CreateClient("Api");
                var httpResponseMess = await client.PostAsJsonAsync("api/Book/add-book", addBookDTO);
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Book");
            }
            catch (Exception ex)
            {
                // Repopulate lists so the view can render
                try
                {
                    var client = httpClientFactory.CreateClient("Api");
                    var authors = await client.GetFromJsonAsync<IEnumerable<authorDTO>>("api/Authors/get-all-author");
                    var publishers = await client.GetFromJsonAsync<IEnumerable<publisherDTO>>("api/Publishers/get-all-publisher");
                    ViewBag.listAuthor = authors ?? Enumerable.Empty<authorDTO>();
                    ViewBag.listPublisher = publishers ?? Enumerable.Empty<publisherDTO>();
                }
                catch { ViewBag.listAuthor = Enumerable.Empty<authorDTO>(); ViewBag.listPublisher = Enumerable.Empty<publisherDTO>(); }
                ViewBag.Error = ex.Message;
            }
            return View(addBookDTO);
        }
        public async Task<IActionResult> ListBookById(int id)
        {
            BookDTO response = new BookDTO();
            try
            {
                var client = httpClientFactory.CreateClient("Api");
                var httpResponseMess = await client.GetAsync($"api/Book/get-book-by-id/{id}");
                httpResponseMess.EnsureSuccessStatusCode();
                var stringResponseBody = await httpResponseMess.Content.ReadAsStringAsync();
                response = await httpResponseMess.Content.ReadFromJsonAsync<BookDTO>();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View(response);
        }
        [HttpGet]
        public async Task<IActionResult> EditBook(int id)
        {
            BookDTO response = new BookDTO();
            try
            {
                var client = httpClientFactory.CreateClient("Api");
                var bookResp = await client.GetAsync($"api/Book/get-book-by-id/{id}");
                if (!bookResp.IsSuccessStatusCode)
                {
                    ViewBag.Error = "Unable to load book.";
                    return View();
                }
                response = await bookResp.Content.ReadFromJsonAsync<BookDTO>();

                // populate lists
                var authors = await client.GetFromJsonAsync<IEnumerable<authorDTO>>("api/Authors/get-all-author");
                var publishers = await client.GetFromJsonAsync<IEnumerable<publisherDTO>>("api/Publishers/get-all-publisher");
                ViewBag.listAuthor = authors ?? Enumerable.Empty<authorDTO>();
                ViewBag.listPublisher = publishers ?? Enumerable.Empty<publisherDTO>();
                ViewBag.Book = response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error loading EditBook");
                ViewBag.Error = ex.Message;
                ViewBag.listAuthor = Enumerable.Empty<authorDTO>();
                ViewBag.listPublisher = Enumerable.Empty<publisherDTO>();
            }
            return View(response);
        }

        [HttpPost]
        public async Task<IActionResult> EditBook(int id, editBookDTO BookDTO)
        {
            try
            {
                var client = httpClientFactory.CreateClient("Api");
                var httpResponseMess = await client.PutAsJsonAsync($"api/Book/edit-book/{id}", BookDTO);
                httpResponseMess.EnsureSuccessStatusCode();
                var respone = await httpResponseMess.Content.ReadFromJsonAsync<editBookDTO>();
                if (Response != null)
                {
                    return RedirectToAction("Index", "Book");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                // Repopulate lists so the view can render
                try
                {
                    var client = httpClientFactory.CreateClient("Api");
                    var authors = await client.GetFromJsonAsync<IEnumerable<authorDTO>>("api/Authors/get-all-author");
                    var publishers = await client.GetFromJsonAsync<IEnumerable<publisherDTO>>("api/Publishers/get-all-publisher");
                    ViewBag.listAuthor = authors ?? Enumerable.Empty<authorDTO>();
                    ViewBag.listPublisher = publishers ?? Enumerable.Empty<publisherDTO>();
                    // Try to load the current BookDTO to supply the view with the expected model type
                    try
                    {
                        var book = await client.GetFromJsonAsync<BookDTO>($"api/Book/get-book-by-id/{id}");
                        ViewBag.Book = book ?? new BookDTO();
                    }
                    catch
                    {
                        ViewBag.Book = new BookDTO();
                    }
                }
                catch { ViewBag.listAuthor = Enumerable.Empty<authorDTO>(); ViewBag.listPublisher = Enumerable.Empty<publisherDTO>(); }
            }
            // The view expects a BookDTO model; provide one by loading it from the API if possible
            var clientFinal = httpClientFactory.CreateClient("Api");
            BookDTO modelToReturn;
            try
            {
                modelToReturn = await clientFinal.GetFromJsonAsync<BookDTO>($"api/Book/get-book-by-id/{id}");
            }
            catch
            {
                modelToReturn = new BookDTO();
            }
            return View(modelToReturn);
        }
        [HttpGet]
        public async Task<IActionResult> DeleteBook([FromRoute] int id)
        {
            try
            {
                var client = httpClientFactory.CreateClient("Api");
                var httpResponseMess = await client.DeleteAsync($"api/Book/delete-book-by-id/{id}");
                httpResponseMess.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Book");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
            }
            return View("Book/Index");
        }
    }
}
