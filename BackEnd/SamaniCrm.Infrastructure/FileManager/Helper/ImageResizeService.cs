
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;
using System.Drawing;

namespace SamaniCrm.Infrastructure.FileManager.Helper;

using System.Drawing;
using System.Drawing.Imaging;

public class ImageResizeService
{
    private readonly string[] ImageExtensions = ["jpg", "png", "jpeg", "gif", "bmp"];
    private readonly Dictionary<string, (int width, int height)> _sizes = new()
    {
        { "small",  (150, 150) },
        { "medium", (400, 400) },
        { "large",  (800, 800) }
    };

    public async Task<List<Thumbnail>?> GenerateThumbnailsAsync(
        Guid imageId,
        Stream inputStream,
        string extension,
        string path,
        CancellationToken cancellationToken = default)
    {
        if (!ImageExtensions.Contains(extension.ToLower()))
            return null;

        var outputPaths = new List<Thumbnail>();

        using var original = Image.FromStream(inputStream);

        foreach (var size in _sizes)
        {
            var resized = ResizeImage(original, size.Value.width, size.Value.height);

            var fileName = $"{imageId}_{size.Key}.{extension}";
            var savePath = Path.Combine(path, "thumbnails", size.Key);

            if (!Directory.Exists(savePath))
                Directory.CreateDirectory(savePath);

            var fullPath = Path.Combine(savePath, fileName);
            resized.Save(fullPath, GetImageFormat(extension));

            outputPaths.Add(new Thumbnail
            {
                Size = size.Key,
                Path = fullPath
            });

            resized.Dispose();
        }

        return outputPaths;
    }

    private Image ResizeImage(Image original, int width, int height)
    {
        var ratioX = (double)width / original.Width;
        var ratioY = (double)height / original.Height;
        var ratio = Math.Min(ratioX, ratioY);

        var newWidth = (int)(original.Width * ratio);
        var newHeight = (int)(original.Height * ratio);

        var resized = new Bitmap(newWidth, newHeight);
        using var graphics = Graphics.FromImage(resized);
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
        graphics.DrawImage(original, 0, 0, newWidth, newHeight);

        return resized;
    }

    private ImageFormat GetImageFormat(string extension)
    {
        return extension.ToLower() switch
        {
            "jpg" or "jpeg" => ImageFormat.Jpeg,
            "png" => ImageFormat.Png,
            "gif" => ImageFormat.Gif,
            "bmp" => ImageFormat.Bmp,
            _ => ImageFormat.Png
        };
    }
}



public class Thumbnail
{
    public required string Size { get; set; }
    public required string Path { get; set; }
}
