using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using SamaniCrm.Application.Common.Interfaces;
using SamaniCrm.Application.FileManager.Interfaces;
using SamaniCrm.Infrastructure;
using SamaniCrm.Infrastructure.FileManager;
using System.Net;

namespace SamaniCrm.Api.Controllers
{
    [Route("/file")]
    public class FileServeController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private readonly IFileManagerService _fileManagerService;
        private string publicRootPath;

        public FileServeController(
            IWebHostEnvironment env,
            IFileManagerService fileManagerService,
            IOptions<FileManagerSettings> settings)
        {
            _env = env;
            _fileManagerService = fileManagerService;
            publicRootPath = settings.Value.PublicFolderPath;
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
            var fileName = fileEntity.Name;

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
                Response.Headers.ContentType = contentType;
                var cd = new ContentDispositionHeaderValue("attachment");
                cd.SetHttpFileName(fileName);
                Response.Headers.ContentDisposition = cd.ToString();
                // Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"";

                return File(partialStream, contentType, enableRangeProcessing: true);
            }
            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(fileName);
            Response.Headers.ContentDisposition = contentDisposition.ToString();
            // Response.Headers.ContentDisposition = $"attachment; filename=\"{fileName}\"";
            Response.Headers.CacheControl = "public,max-age=604800"; // 7 days cache
            return File(stream, contentType, fileEntity.Name, enableRangeProcessing: true);
        }
    }
}
