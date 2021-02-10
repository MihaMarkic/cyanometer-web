using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Cyanometer.Web.Utilities
{
    using SignatureValue = ImmutableArray<ImmutableArray<byte>>;

    public static class Extensions
    {
        public static ImmutableArray<byte> AddRange(this ImmutableArray<byte> source, params int[] args)
        {
            return source.AddRange(args.Select(i => Convert.ToByte(i)));
        }
    }
    public static class FileHelpers
    {
        // If you require a check on specific characters in the IsValidFileExtensionAndSignature
        // method, supply the characters in the _allowedChars field.
        static readonly byte[] allowedChars = { };
        // For more file signatures, see the File Signatures Database (https://www.filesignatures.net/)
        // and the official specifications for the file types you wish to add.
        static readonly ImmutableDictionary<string, SignatureValue> fileSignature =
            new Dictionary<string, SignatureValue>
                {
                    {".gif", SignatureValue.Empty
                            .Add(ImmutableArray<byte>.Empty.AddRange(0x47, 0x49, 0x46, 0x38)) },
                    {".png", SignatureValue.Empty
                            .Add(ImmutableArray<byte>.Empty.AddRange(0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A)) },
                    { ".jpeg", SignatureValue.Empty
                            .Add(ImmutableArray<byte>.Empty.AddRange(0xFF, 0xD8, 0xFF, 0xE0))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0xFF, 0xD8, 0xFF, 0xE2))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0xFF, 0xD8, 0xFF, 0xE3))
                    },
                    { ".jpg", SignatureValue.Empty
                            .Add(ImmutableArray<byte>.Empty.AddRange(0xFF, 0xD8, 0xFF, 0xE0))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0xFF, 0xD8, 0xFF, 0xE1))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0xFF, 0xD8, 0xFF, 0xE8))
                    },
                    { ".zip", SignatureValue.Empty
                            .Add(ImmutableArray<byte>.Empty.AddRange(0x50, 0x4B, 0x03, 0x04))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0x50, 0x4B, 0x4C, 0x49, 0x54, 0x45))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0x50, 0x4B, 0x53, 0x70, 0x58))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0x50, 0x4B, 0x05, 0x06))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0x50, 0x4B, 0x07, 0x08))
                            .Add(ImmutableArray<byte>.Empty.AddRange(0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70))
                    },
                }.ToImmutableDictionary();

        public static async Task<Stream> ProcessStreamedFile(
            ILogger logger,
            MultipartSection section, ContentDispositionHeaderValue contentDisposition,
            ModelStateDictionary modelState, string[] permittedExtensions, long sizeLimit)
        {
            var memoryStream = new MemoryStream();
            try
            {
                await section.Body.CopyToAsync(memoryStream);

                // Check if the file is empty or exceeds the size limit.
                if (memoryStream.Length == 0)
                {
                    modelState.AddModelError("File", "The file is empty.");
                }
                else if (memoryStream.Length > sizeLimit)
                {
                    var megabyteSizeLimit = sizeLimit / 1048576;
                    modelState.AddModelError("File", $"The file exceeds {megabyteSizeLimit:N1} MB.");
                }
                else if (!IsValidFileExtensionAndSignature(contentDisposition.FileName.Value, memoryStream, permittedExtensions))
                {
                    modelState.AddModelError("File", "The file type isn't permitted or the file's signature doesn't match the file's extension.");
                }
                else
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    return memoryStream;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Upload failed");
                modelState.AddModelError("File", $"The upload failed. Please contact the Help Desk for support. Error: {ex.HResult}");
            }

            return memoryStream;
        }

        static bool IsValidFileExtensionAndSignature(string fileName, Stream stream, string[] permittedExtensions)
        {
            if (string.IsNullOrEmpty(fileName) || stream == null || stream.Length == 0)
            {
                return false;
            }

            var ext = Path.GetExtension(fileName).ToLowerInvariant();

            if (string.IsNullOrEmpty(ext) || !permittedExtensions.Contains(ext))
            {
                return false;
            }

            stream.Position = 0;

            using (var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
            {
                if (ext.Equals(".txt") || ext.Equals(".csv") || ext.Equals(".prn"))
                {
                    if (allowedChars.Length == 0)
                    {
                        // Limits characters to ASCII encoding.
                        for (var i = 0; i < stream.Length; i++)
                        {
                            if (reader.ReadByte() > sbyte.MaxValue)
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        // Limits characters to ASCII encoding and
                        // values of the _allowedChars array.
                        for (var i = 0; i < stream.Length; i++)
                        {
                            var b = reader.ReadByte();
                            if (b > sbyte.MaxValue ||
                                !allowedChars.Contains(b))
                            {
                                return false;
                            }
                        }
                    }

                    return true;
                }

                // Uncomment the following code block if you must permit
                // files whose signature isn't provided in the _fileSignature
                // dictionary. We recommend that you add file signatures
                // for files (when possible) for all file types you intend
                // to allow on the system and perform the file signature
                // check.
                /*
                if (!_fileSignature.ContainsKey(ext))
                {
                    return true;
                }
                */

                // File signature check
                // --------------------
                // With the file signatures provided in the _fileSignature
                // dictionary, the following code tests the input content's
                // file signature.
                var signatures = fileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));

                return signatures.Any(signature =>
                    headerBytes.Take(signature.Length).SequenceEqual(signature));
            }
        }
    }
}
