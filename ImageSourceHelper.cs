using Microsoft.Maui.Controls;

namespace MatteoAPP1
{
    internal static class ImageSourceHelper
    {
        private static readonly string[] KnownExtensions = [".png", ".jpg", ".jpeg", ".webp", ".gif", ".svg"];

        public static ImageSource? Resolve(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();

            if (Uri.TryCreate(trimmed, UriKind.Absolute, out var absoluteUri))
            {
                if (absoluteUri.IsFile && File.Exists(absoluteUri.LocalPath))
                {
                    return ImageSource.FromFile(absoluteUri.LocalPath);
                }

                return ImageSource.FromUri(absoluteUri);
            }

            if (File.Exists(trimmed))
            {
                return ImageSource.FromFile(trimmed);
            }

            var normalized = trimmed.Replace('\\', '/');
            if (normalized.StartsWith("Resources/Images/", StringComparison.OrdinalIgnoreCase))
            {
                normalized = Path.GetFileName(normalized);
            }

            if (HasKnownExtension(normalized))
            {
                return ImageSource.FromFile(normalized);
            }

            foreach (var extension in KnownExtensions)
            {
                var candidate = normalized + extension;
                if (BundledImageExists(candidate))
                {
                    return ImageSource.FromFile(candidate);
                }
            }

            return BundledImageExists(normalized) ? ImageSource.FromFile(normalized) : null;
        }

        private static bool HasKnownExtension(string path)
        {
            var extension = Path.GetExtension(path);
            return KnownExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase);
        }

        private static bool BundledImageExists(string fileName)
        {
            var fullPath = Path.Combine(AppContext.BaseDirectory, fileName);
            if (File.Exists(fullPath))
            {
                return true;
            }

            var resourcesPath = Path.Combine(AppContext.BaseDirectory, "Resources", "Images", fileName);
            if (File.Exists(resourcesPath))
            {
                return true;
            }

            var projectImagePath = Path.Combine(FileSystem.AppDataDirectory, fileName);
            return File.Exists(projectImagePath);
        }
    }
}
