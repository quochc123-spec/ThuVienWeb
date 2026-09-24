using ThuVienWeb.Models.Domain;
using ThuVienWeb.Models.DTO;

namespace ThuVienWeb.Repositories
{
    public interface IPublisherRepository
    {
        List<PublisherDTO> GetAllPublishers();
        PublisherNoIdDTO? GetPublisherById(int id);
        AddPublisherRequestDTO AddPublisher(AddPublisherRequestDTO addPublisherRequestDTO);
        PublisherNoIdDTO? UpdatePublisherById(int id, PublisherNoIdDTO publisherNoIdDTO);
        Publisher? DeletePublisherById(int id);

    }
}
