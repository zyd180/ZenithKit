using System.IO;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace MiCodeAutoToolBox.App.Services;

public interface IPdfToolsService
{
    Task<string> MergeAsync(IEnumerable<string> pdfPaths, string? outputPath = null, CancellationToken cancellationToken = default);
    Task SplitAsync(string pdfPath, string outputFolder, CancellationToken cancellationToken = default);
}

public sealed class PdfToolsService : IPdfToolsService
{
    private readonly string _basePath;

    public PdfToolsService()
    {
        _basePath = Directory.GetCurrentDirectory();
    }

    public Task<string> MergeAsync(IEnumerable<string> pdfPaths, string? outputPath = null, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            var list = pdfPaths?.ToList() ?? new List<string>();
            if (list.Count == 0) throw new ArgumentException("No pdf paths provided", nameof(pdfPaths));
            outputPath ??= Path.Combine(_basePath, $"merged_{DateTime.Now:yyyyMMdd_HHmmss}.pdf");

            using var outDoc = new PdfDocument();
            foreach (var path in list)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var input = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                for (int i = 0; i < input.PageCount; i++)
                {
                    outDoc.AddPage(input.Pages[i]);
                }
            }
            outDoc.Save(outputPath);
            return outputPath;
        }, cancellationToken);
    }

    public Task SplitAsync(string pdfPath, string outputFolder, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            if (!File.Exists(pdfPath)) throw new FileNotFoundException("Pdf not found", pdfPath);
            Directory.CreateDirectory(outputFolder);
            using var input = PdfReader.Open(pdfPath, PdfDocumentOpenMode.Import);
            for (int i = 0; i < input.PageCount; i++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                using var doc = new PdfDocument();
                doc.AddPage(input.Pages[i]);
                var outPath = Path.Combine(outputFolder, $"page_{i + 1}.pdf");
                doc.Save(outPath);
            }
        }, cancellationToken);
    }
}
