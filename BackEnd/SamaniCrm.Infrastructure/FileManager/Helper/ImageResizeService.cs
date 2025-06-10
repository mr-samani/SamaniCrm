using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Infrastructure.FileManager.Helper;

public class ImageResizeService
{
    private string[] ImageExtentions = ["jpg", "png", "jpeg", "gif", "bmp", "hiec", "tiff"];
    private readonly Dictionary<string, (int width, int height)> _sizes = new()
    {
        { "small",  (150, 150) },
        { "medium", (400, 400) },
        { "large",  (800, 800) }
    };
    public async Task<List<Thumbnail>?> GenerateThumbnailsAsync(Guid imageId, Stream inputStream, string extension,string path, CancellationToken cancellationToken = default)
    {
         
        if(ImageExtentions.Contains(extension) == false)
        {
            return null;
        }
        if (!inputStream.CanSeek)
        {
            var mem = new MemoryStream();
            await inputStream.CopyToAsync(mem, cancellationToken);
            mem.Position = 0;
            inputStream = mem;
        }

        var outputPaths = new List<Thumbnail>();

        inputStream.Position = 0;
        using var image = await Image.LoadAsync(inputStream, cancellationToken);

        foreach (var size in _sizes)
        {
            inputStream.Position = 0;

            var resized = image.Clone(ctx =>
            {
                ctx.Resize(new ResizeOptions
                {
                    Size = new Size(size.Value.width, size.Value.height),
                    Mode = ResizeMode.Max
                });
            });

            var fileName = $"{imageId}_{size.Key}.{extension}";
            var savePath = Path.Combine(path,"thumbnails", size.Key);
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            var fullPath = Path.Combine(savePath, fileName);

            await resized.SaveAsync(fullPath, cancellationToken);
            outputPaths.Add( new Thumbnail()
            {
                Size = size.Key,
                Path = fullPath
            });
        }

        return outputPaths;
    }
}


public class Thumbnail
{
    public string Size { get; set; }
    public string Path { get; set; }
}
