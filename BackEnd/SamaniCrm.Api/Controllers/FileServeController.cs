using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using SamaniCrm.Application.FileManager.Interfaces;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.FileManager;
using System.Net;

namespace SamaniCrm.Api.Controllers
{
    [Route("/file")]
    public class FileServeController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IWebHostEnvironment _env;
        private readonly IFileManagerService _fileManagerService;
        private readonly IConfiguration _configuration;
        private string publicRootPath;

        public FileServeController(ApplicationDbContext dbContext, IWebHostEnvironment env, IFileManagerService fileManagerService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _env = env;
            _fileManagerService = fileManagerService;
            _configuration = configuration;
            var settings = configuration.GetSection("FileManager").Get<FileManagerSetting>() ?? new FileManagerSetting();
            publicRootPath = settings.PublicFolderPath;
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            var fileEntity = await _fileManagerService.GetFileInfo(id, cancellationToken);

            if (fileEntity == null)
                return NotFound("File not found.");

            var physicalPath = Path.Combine(_env.ContentRootPath, publicRootPath, fileEntity.RelativePath);

            if (!System.IO.File.Exists(physicalPath))
                return NotFound("File not found on disk.");

            var stream = System.IO.File.OpenRead(physicalPath);
            var fileLength = stream.Length;
            var contentType = fileEntity.ContentType ?? "application/octet-stream";

            // Handle range requests (video seek support)
            if (Request.Headers.ContainsKey("Range"))
            {
                var rangeHeader = Request.Headers["Range"].ToString();
                var range = RangeHeaderValue.Parse(rangeHeader);
                var from = range.Ranges.First().From ?? 0;
                var to = range.Ranges.First().To ?? fileLength - 1;
                var length = to - from + 1;

                stream.Seek(from, SeekOrigin.Begin);
                var partialStream = new MemoryStream();
                await stream.CopyToAsync(partialStream, (int)length, cancellationToken);
                partialStream.Position = 0;

                Response.StatusCode = (int)HttpStatusCode.PartialContent;
                Response.Headers.ContentRange = $"bytes {from}-{to}/{fileLength}";
                Response.ContentLength = length;
                Response.ContentType = contentType;

                return File(partialStream, contentType, enableRangeProcessing: true);
            }

            Response.Headers.CacheControl = "public,max-age=604800"; // 7 days cache
            return File(stream, contentType, fileEntity.Name, enableRangeProcessing: true);
        }
    }
}
