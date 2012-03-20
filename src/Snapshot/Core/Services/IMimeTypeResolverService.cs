using System;
namespace Core.Services
{
    public interface IMimeTypeResolverService
    {
        string GetMIMEType(string filePath);
        bool TryGetMIMETypeFromRegistry(string filePath, out string mimeType);
    }
}
