using Cyanometer.AirQuality.Services.Implementation.Specific;
using Cyanometer.Core;
using Cyanometer.Web.Models;
using Cyanometer.Web.Services.Abstract;
using Cyanometer.Web.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Cyanometer.Web.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController : CyanometerController<ImagesController>
    {
        readonly IImagesManager imagesManager;
        static readonly FormOptions defaultFormOptions = new FormOptions();
        public ImagesController(ILogger<ImagesController> logger,
            IImagesManager imagesManager) : base(logger)
        {
            this.imagesManager = imagesManager;
        }

        [HttpGet]
        public ActionResult Test()
        {
            return Ok();
        }

        [HttpGet]
        [Route("{country}/{city}")]
        public ActionResult<DisplayData> Get(string country, string city)
        {
            var dataSource = CyanometerDataSources.Default.GetData(city, country);
            if (dataSource != null)
            {
                DateTimeOffset now;
#if DEBUG
                now = new DateTimeOffset(2017, 08, 03, 12, 12, 12, TimeSpan.Zero);                
#else
                now = DateTimeOffset.Now;
#endif
                var lastImages = imagesManager.GetOlderImagesThan(dataSource, now).Take(12);
                var query = from m in lastImages
                            select new ImageItem(
                                     takenAt: m.Date,
                                     url: m.ImageUriPath,
                                     thumbnailUrl: m.ThumbnailUriPath,
                                     city: city,
                                     country: country,
                                     id: 146649,
                                     bluenessIndex: m.BluenessIndex
                                 );
                var imageItems = query.ToImmutableArray();
                var averageBlueness = Convert.ToInt32(imageItems.Average(i => i.BluenessIndex));
                return new DisplayData(
                    averageBlueness: averageBlueness,
                    images: imageItems
                );
            }
            return NotFound();
        }

        [HttpPost]
        [Route("{country}/{city}")]
        //[DisableFormValueModelBinding]
        public async Task<IActionResult> PostFiles()
        {
            try
            {
                string country = (string)Request.RouteValues["country"];
                string city = (string)Request.RouteValues["city"];
                logger.LogDebug($"Receiving photo for {country}/{city}");
                var dataSource = CyanometerDataSources.Default.GetData(city, country);
                if (dataSource != null)
                {
                    string token = Request.Headers["CyanometerToken"];
                    if (!string.Equals(dataSource.Id.ToString(), token, StringComparison.OrdinalIgnoreCase))
                    {
                        logger.LogWarning("Invalid token");
                        return StatusCode(StatusCodes.Status401Unauthorized);
                    }
                    if (!MultipartRequestHelper.IsMultipartContentType(Request.ContentType))
                    {
                        ModelState.AddModelError("File", "The request couldn't be processed (Error 1).");
                        logger.LogWarning("The request couldn't be processed (Error 1).");
                        return BadRequest(ModelState);
                    }
                    var boundary = MultipartRequestHelper.GetBoundary(
                        MediaTypeHeaderValue.Parse(Request.ContentType),
                        defaultFormOptions.MultipartBoundaryLengthLimit);
                    var reader = new MultipartReader(boundary, HttpContext.Request.Body);
                    var section = await reader.ReadNextSectionAsync();

                    while (section != null)
                    {
                        logger.LogDebug("Reading section");
                        var hasContentDispositionHeader = ContentDispositionHeaderValue.TryParse(section.ContentDisposition, out var contentDisposition);

                        if (hasContentDispositionHeader)
                        {
                            // This check assumes that there's a file
                            // present without form data. If form data
                            // is present, this method immediately fails
                            // and returns the model error.
                            if (!MultipartRequestHelper.HasFileContentDisposition(contentDisposition))
                            {
                                ModelState.AddModelError("File", "The request couldn't be processed (Error 2)");
                                logger.LogWarning("The request couldn't be processed (Error 2)");
                                return BadRequest(ModelState);
                            }
                            else
                            {
                                // Don't trust the file name sent by the client. To display
                                // the file name, HTML-encode the value.
                                using (var streamedFileContent = await FileHelpers.ProcessStreamedFile(logger,
                                    section, contentDisposition, ModelState,
                                    new[] { ".jpg", ".jpeg" }, 1024 * 1024 * 6)) // max 4MB file
                                {

                                    if (!ModelState.IsValid)
                                    {

                                        logger.LogWarning($"Invalid model state");
                                        foreach (var pair in ModelState)
                                        {
                                            foreach (var error in pair.Value.Errors)
                                            {
                                                logger.LogError($"{pair.Key}: {error.ErrorMessage}");
                                            }
                                        }
                                        return BadRequest(ModelState);
                                    }

                                    try
                                    {
                                        logger.LogDebug($"Saving image {contentDisposition.FileName.Value}");
                                        imagesManager.SaveImage(dataSource, contentDisposition.FileName.Value, streamedFileContent);
                                    }
                                    catch (Exception ex)
                                    {
                                        logger.LogError(ex, "Failed saving image");
                                        return BadRequest("Image procession failed");
                                    }
                                }
                            }
                        }

                        // Drain any remaining section body that hasn't been consumed and
                        // read the headers for the next section.
                        section = await reader.ReadNextSectionAsync();
                    }
                    logger.LogDebug("Done reading sections");

                    return Created(nameof(ImagesController), null);
                }
                else
                {
                    logger.LogWarning($"No data source found for given {country}/{city}");
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed processing file");
                throw;
            }
        }
    }

    public static class MultipartRequestHelper
    {
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec at https://tools.ietf.org/html/rfc2046#section-5.1 states that 70 characters is a reasonable limit.
        public static string GetBoundary(MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary).Value;

            if (string.IsNullOrWhiteSpace(boundary))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }

        public static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool HasFormDataContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && string.IsNullOrEmpty(contentDisposition.FileName.Value)
                && string.IsNullOrEmpty(contentDisposition.FileNameStar.Value);
        }

        public static bool HasFileContentDisposition(ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                && contentDisposition.DispositionType.Equals("form-data")
                && (!string.IsNullOrEmpty(contentDisposition.FileName.Value)
                    || !string.IsNullOrEmpty(contentDisposition.FileNameStar.Value));
        }
    }
}