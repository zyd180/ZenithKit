using System.IO;

namespace MiCodeAutoToolBox.App.Services;

using MiCodeAutoToolBox.App.Models;

public interface IDiffService
{
    Task<IReadOnlyList<DiffPair>> DiffAsync(string leftPath, string rightPath, CancellationToken cancellationToken = default);
}

public sealed class DiffService : IDiffService
{
    public Task<IReadOnlyList<DiffPair>> DiffAsync(string leftPath, string rightPath, CancellationToken cancellationToken = default)
    {
        return Task.Run<IReadOnlyList<DiffPair>>(() =>
        {
            if (string.IsNullOrWhiteSpace(leftPath) || string.IsNullOrWhiteSpace(rightPath))
                throw new ArgumentException("Both paths required");
            if (!File.Exists(leftPath) || !File.Exists(rightPath))
                throw new FileNotFoundException("File not found");

            var left = File.ReadAllLines(leftPath);
            var right = File.ReadAllLines(rightPath);
            var max = Math.Max(left.Length, right.Length);
            var list = new List<DiffPair>(max);

            for (int i = 0; i < max; i++)
            {
                var l = i < left.Length ? left[i] : string.Empty;
                var r = i < right.Length ? right[i] : string.Empty;
                list.Add(new DiffPair
                {
                    LineNumber = i + 1,
                    Left = l,
                    Right = r,
                    IsDifferent = !string.Equals(l, r, StringComparison.Ordinal)
                });
            }

            return list;
        }, cancellationToken);
    }
}
