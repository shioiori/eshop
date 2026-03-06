using Marten.Schema;

namespace Catalog.API.Data
{
  public class CatalogInitialData : IInitialData
  {
    public async Task Populate(IDocumentStore store, CancellationToken cancellation)
    {
      using var session = store.LightweightSession();
      if (await session.Query<Product>().AnyAsync())
      {
        return;
      }
      session.Store<Product>(GetPreconfiguredProducts());
      await session.SaveChangesAsync();
    }

    private static IEnumerable<Product> GetPreconfiguredProducts() => new List<Product>()
    {
      new Product()
      {
        Id = Guid.NewGuid(),
        Name = "Iphone 16",
        Description = "iPhone 16 is built for Apple Intelligence, the personal intelligence system that helps you write, express yourself, and get things done effortlessly. With groundbreaking privacy protections, it gives you peace of mind that no one else can access your data — not even Apple.",
        Category = new List<string>(){"electric device", "smart phone", "apple" },
        ImageFile = "iphone16.png",
        Price = 500M
      },
      new Product()
      {
        Id = Guid.NewGuid(),
        Name = "Sony ZV-E10",
        Description = "Sony ZV E10 là một sản phẩm máy ảnh kỹ thuật số nổi tiếng thuộc thương hiệu Sony được thiết kế đặc biệt dành cho những người yêu thích nhiếp ảnh và quay phim. Máy ảnh được trang bị khả năng quay video 4K chất lượng cao, lấy nét tự động nhanh và chính xác, cùng với nhiều tính năng hỗ trợ quay vlog, ZV-E10 sẽ giúp bạn tạo ra những video chuyên nghiệp ngay cả khi bạn là người mới bắt đầu. Với thiết kế nhỏ gọn và tính năng chuyên nghiệp, chiếc máy ảnh Sony này hứa hẹn sẽ mang đến những trải nghiệm tuyệt vời cho người dùng. ",
        Category = new List<string>(){"sony", "digital camera"},
        ImageFile = "sonyzve10.png",
        Price = 320M
      },
      new Product()
      {
        Id = Guid.NewGuid(),
        Name = "Huawei Watch GT4",
        Description = "Huawei Watch GT 4 gây ấn tượng với người dùng bởi 2 phiên bản 46mm và 41mm thiết kế sang trọng và màu sắc đa dạng: xanh, xám, nâu, đen, trắng bạc. Bên cạnh đó, thời lượng pin sử dụng lên đến 14 ngày với bản 46mm và 7 ngày với bản 41mm giúp nâng cao trải nghiệm sử dụng mà không cần lo lắng gián đoạn giữa chừng. Các tính năng khác nổi bật phải kể đến như: huấn luyện viên thông minh với hơn 100 bài luyện tập, theo dõi giấc ngủ TruSleep, phát hiện nhịp tim bất thường, dự đoán trước chu kỳ và thời gian rụng trứng ở nữ giới,...",
        Category = new List<string>(){"watch", "female-oriented"},
        ImageFile = "huaweiwatchgt4.png",
        Price = 47M
      },
      new Product()
      {
        Id = Guid.NewGuid(),
        Name = "Laptop HP Pavilion 15-EG3111TU",
        Description = "aptop HP Pavilion 15 EG3111TU 8U6L8PA được xem là phù hợp để sử dụng trong các tác vụ học tập, làm việc với CPU I5-1335U, 2 thanh RAM 8GB bộ nhớ trong 512GB SSD. Việc giải trí trên máy cũng được tối ưu với kích thước màn hình 15.6 inch, hỗ trợ tấm nền IPS cùng độ phân giải 1920 x 1080 pixels. Sản phẩm laptop HP Pavilion này cũng sử dụng phiên bản Windows 11 mới nhất, hỗ trợ nhiều tiện ích cũng như tính năng mới.",
        Category = new List<string>(){"laptop"},
        ImageFile = "hppav15.png",
        Price = 248M
      },
      new Product()
      {
        Id = Guid.NewGuid(),
        Name = "iPad Air 6 M2 11 inch Wifi 128GB",
        Description = "iPad Air 6 M2 11 inch sở hữu màn hình Retina 11 inch với công nghệ IPS cùng dải màu P3 hỗ trợ hiển thị hình ảnh sống động. iPad với màn hình độ sáng cao tới 500 nits cùng với lớp phủ oleophobia chống dấu vân tay vượt trội. Cùng với đó iPad Air 6 M2 2024 này hoạt động trên con chip Apple M2 cùng dung lượng RAM 8GB.",
        Category = new List<string>(){"electric device", "apple"},
        ImageFile = "ipadair6.png",
        Price = 478M
      },
    };
  }
}
