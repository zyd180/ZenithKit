using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ZenithKit.Core.Services;

namespace ZenithKit.App.Services;

public interface IImageToolsService
{
    string SaveDirectory { get; set; }
    Task<string> ConvertAsync(string sourcePath, string format, int? maxWidth = null, int? maxHeight = null, long? quality = null, CancellationToken cancellationToken = default);
}

#pragma warning disable CA1416 // Windows-only APIs (GDI+ encoders, bitmaps)
public sealed class ImageToolsService : IImageToolsService
{
    private readonly IStorageService _storageService;

    public string SaveDirectory { get; set; }

    public ImageToolsService(IStorageService storageService)
    {
        _storageService = storageService;
        SaveDirectory = Path.Combine(_storageService.CurrentPath, "images");
    }

    public Task<string> ConvertAsync(string sourcePath, string format, int? maxWidth = null, int? maxHeight = null, long? quality = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (string.IsNullOrWhiteSpace(sourcePath)) throw new ArgumentException("sourcePath required", nameof(sourcePath));
            if (!File.Exists(sourcePath)) throw new FileNotFoundException("Source image not found", sourcePath);

            using var image = Image.FromFile(sourcePath);
            var targetSize = GetTargetSize(image.Size, maxWidth, maxHeight);
            using var resized = new Bitmap(image, targetSize);

            var imagesDir = string.IsNullOrWhiteSpace(SaveDirectory)
                ? Path.Combine(_storageService.CurrentPath, "images")
                : SaveDirectory;
            Directory.CreateDirectory(imagesDir);
            var ext = format.ToLowerInvariant() switch
            {
                "png" => ".png",
                "jpg" or "jpeg" => ".jpg",
                "bmp" => ".bmp",
                _ => ".png"
            };
            var outPath = Path.Combine(imagesDir, $"image_{DateTime.Now:yyyyMMdd_HHmmss}{ext}");

            var encoder = GetEncoder(ext);
            if (encoder is not null && quality.HasValue)
            {
                using var encoderParams = new EncoderParameters(1);
                encoderParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, quality.Value);
                resized.Save(outPath, encoder, encoderParams);
            }
            else if (encoder is not null)
            {
                resized.Save(outPath, encoder, null);
            }
            else
            {
                resized.Save(outPath);
            }

            return outPath;
        }, cancellationToken);
    }

    private static Size GetTargetSize(Size original, int? maxWidth, int? maxHeight)
    {
        int width = original.Width;
        int height = original.Height;
        if (maxWidth.HasValue && width > maxWidth.Value)
        {
            height = (int)(height * (maxWidth.Value / (double)width));
            width = maxWidth.Value;
        }
        if (maxHeight.HasValue && height > maxHeight.Value)
        {
            width = (int)(width * (maxHeight.Value / (double)height));
            height = maxHeight.Value;
        }
        return new Size(Math.Max(1, width), Math.Max(1, height));
    }

    private static ImageCodecInfo? GetEncoder(string ext)
    {
        var format = ext.ToLowerInvariant();
        return ImageCodecInfo.GetImageEncoders().FirstOrDefault(e => e.FilenameExtension!.Contains(format.ToUpperInvariant()));
    }
}
#pragma warning restore CA1416
