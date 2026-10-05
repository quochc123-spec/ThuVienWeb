using ThuVienWeb.Models.Domain;

namespace ThuVienWeb.Repositories
{
    public interface IImageRepository
    {
        Image Upload(Image image);
        List<Image> GetAllInfoImages();
        (byte[], string, string) DownloadFile(int id);
    }
}
