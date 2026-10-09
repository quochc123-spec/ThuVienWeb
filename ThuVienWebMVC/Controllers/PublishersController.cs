using Microsoft.AspNetCore.Mvc;
using ThuVienWebMVC.Models.DTO;

namespace ThuVienWebMVC.Controllers
{
    public class PublishersController : Controller
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<PublishersController> logger;

        public PublishersController(IHttpClientFactory httpClientFactory, ILogger<PublishersController> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var client = httpClientFactory.CreateClient("Api");
            var resp = await client.GetAsync("api/Publishers/get-all-publisher");
            if (!resp.IsSuccessStatusCode) return View("Error");
            var items = await resp.Content.ReadFromJsonAsync<IEnumerable<publisherDTO>>();
            return View(items);
        }

        public async Task<IActionResult> Details(int id)
        {
            var client = httpClientFactory.CreateClient("Api");
            var resp = await client.GetAsync($"api/Publishers/get-publisher-by-id?id={id}");
            if (!resp.IsSuccessStatusCode) return View("Error");
            var item = await resp.Content.ReadFromJsonAsync<publisherDTO>();
            return View(item);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] publisherNoIdDTO model)
        {
            var client = httpClientFactory.CreateClient("Api");
            var add = new { Name = model.Name };
            var resp = await client.PostAsJsonAsync("api/Publishers/add-publisher", add);
            if (!resp.IsSuccessStatusCode) return View("Error");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = httpClientFactory.CreateClient("Api");
            var resp = await client.GetAsync($"api/Publishers/get-publisher-by-id?id={id}");
            if (!resp.IsSuccessStatusCode) return View("Error");
            var item = await resp.Content.ReadFromJsonAsync<publisherDTO>();
            return View(item);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [FromForm] publisherDTO model)
        {
            var client = httpClientFactory.CreateClient("Api");
            var update = new { Name = model.Name };
            var resp = await client.PutAsJsonAsync($"api/Publishers/update-publisher-by-id/{id}", update);
            if (!resp.IsSuccessStatusCode) return View("Error");
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = httpClientFactory.CreateClient("Api");
            var resp = await client.DeleteAsync($"api/Publishers/delete-publisher-by-id/{id}");
            if (!resp.IsSuccessStatusCode) return View("Error");
            return RedirectToAction("Index");
        }
    }
}
