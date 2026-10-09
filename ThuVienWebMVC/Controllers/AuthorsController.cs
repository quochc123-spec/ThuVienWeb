using Microsoft.AspNetCore.Mvc;
using ThuVienWebMVC.Models.DTO;

namespace ThuVienWebMVC.Controllers
{
    public class AuthorsController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<AuthorsController> logger;

        public AuthorsController(IHttpClientFactory httpClientFactory, ILogger<AuthorsController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient("Api");
            var resp = await client.GetAsync("api/Authors/get-all-author");
            if (!resp.IsSuccessStatusCode) return View("Error");
            var authors = await resp.Content.ReadFromJsonAsync<IEnumerable<authorDTO>>();
            return View(authors);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = httpClientFactory.CreateClient("Api");
            var resp = await client.GetAsync($"api/Authors/get-author-by-id/{id}");
            if (!resp.IsSuccessStatusCode) return View("Error");
            var author = await resp.Content.ReadFromJsonAsync<authorDTO>();
            return View(author);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] authorDTO model)
        {
            var client = httpClientFactory.CreateClient("Api");
            var add = new { FullName = model.Name };
            var resp = await client.PostAsJsonAsync("api/Authors/add-author", add);
            if (!resp.IsSuccessStatusCode) return View("Error");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = httpClientFactory.CreateClient("Api");
            var resp = await client.GetAsync($"api/Authors/get-author-by-id/{id}");
            if (!resp.IsSuccessStatusCode) return View("Error");
            var author = await resp.Content.ReadFromJsonAsync<authorDTO>();
            return View(author);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromForm] authorDTO model)
        {
            var client = httpClientFactory.CreateClient("Api");
            var update = new { FullName = model.Name };
            var resp = await client.PutAsJsonAsync($"api/Authors/update-author-by-id/{id}", update);
            if (!resp.IsSuccessStatusCode) return View("Error");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = httpClientFactory.CreateClient("Api");
            var resp = await client.DeleteAsync($"api/Authors/delete-author-by-id/{id}");
            if (!resp.IsSuccessStatusCode) return View("Error");
            return RedirectToAction("Index");
        }
    }
}
