using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SRPM.Data
{
    // Cần phải triển khai IDesignTimeDbContextFactory để hỗ trợ tạo DbContext ở thời gian thiết kế
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            // Lấy cấu hình từ file appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // Đảm bảo đúng đường dẫn tới thư mục dự án
                .AddJsonFile("appsettings.json")
                .Build();

            // Cấu hình DbContext với chuỗi kết nối PostgreSQL
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));

            // Trả về DbContext mới được cấu hình
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
