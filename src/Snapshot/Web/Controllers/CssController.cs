using Core.Services;
using System;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web.Bootstrap;
using Web.Services;

namespace Web.Controllers
{
    /// <summary>
    /// This controller is responsible for compacting scripts and versioning
    /// </summary>
    public class CssController : Controller
    {
        #region Constants

        private const string IfNoneMatchHeader = "If-None-Match";
        private const string LastModifiedSinceHeader = "If-Modified-Since";
        private const string CssFolder = "~/Assets/css/";


        #endregion

        #region Fields

        private readonly ICssProviderService _cssProviderService;
        public IETagService ETagService { get; set; }
        public IPathService PathService { get; set; }


        #endregion

        #region Ctors


        public CssController(ICssProviderService cssProviderServicer)
        {
            _cssProviderService = cssProviderServicer;
        }

        #endregion

        #region Actions

        /// <summary>
        /// Returns the stylesheet
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(string group)
        {
            string absolutePath = Server.MapPath(CssFolder);

            if (!PathService.Exists(absolutePath))
            {
                Response.StatusCode = (int) HttpStatusCode.NotFound;
                Response.SuppressContent = true;
                return new EmptyResult();
            }


            // 304 (If-Last-Modified)
            DateTime lLastModified = PathService.GetLastWriteTime(absolutePath);
            if (BrowserIsRequestingFileUnmodifiedSince(lLastModified))
            {
                Response.StatusCode = (int) HttpStatusCode.NotModified;
                Response.SuppressContent = true;
                return new EmptyResult();
            }

            // 304 (If-None-Match), a better test of uniqueness than modified date
            string lEtag = ETagService.Generate(absolutePath);
            if (BrowserIsRequestingFileIdentifiedBy(lEtag))
            {
                Response.StatusCode = (int) HttpStatusCode.NotModified;
                Response.SuppressContent = true;
                return new EmptyResult();
            }
            // 200 - OK
            AddCachingHeaders(lEtag, lLastModified, AppSettings.StaticFileHttpMaxAge);
            // and reading everything that follows the version
            string content;
            try
            {
                content = _cssProviderService.GetCss(group);
            }
            catch (Exception ex)
            {
                content = ex + ex.StackTrace + ex.Source;
            }

            return new ContentResult
                {
                    Content = content,
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "text/css"
                };
        }

        #endregion Actions

        #region Methods

        private bool BrowserIsRequestingFileIdentifiedBy(string etag)
        {
            if (Request.Headers[IfNoneMatchHeader] == null)
            {
                return false;
            }

            string lIfNoneMatch = Request.Headers[IfNoneMatchHeader];

            return lIfNoneMatch.Equals(etag, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool BrowserIsRequestingFileUnmodifiedSince(DateTime lastModified)
        {
            if (Request.Headers[LastModifiedSinceHeader] == null)
            {
                return false;
            }

            // Header values may have additional attributes separated by semi-colons
            string ifModifiedSince = Request.Headers[LastModifiedSinceHeader];
            if (ifModifiedSince.IndexOf(";", StringComparison.Ordinal) > -1)
            {
                ifModifiedSince = ifModifiedSince.Split(';').First();
            }

            // Get the dates for comparison; truncate milliseconds in date if needed
            DateTime sinceDate = Convert.ToDateTime(ifModifiedSince).ToUniversalTime();
            DateTime fileDate = lastModified.ToUniversalTime();
            if (sinceDate.Millisecond.Equals(0))
            {
                fileDate = new DateTime(fileDate.Year,
                                        fileDate.Month,
                                        fileDate.Day,
                                        fileDate.Hour,
                                        fileDate.Minute,
                                        fileDate.Second,
                                        0);
            }

            return fileDate.CompareTo(sinceDate) <= 0;
        }

        private void AddCachingHeaders(string etag, DateTime lastModified, TimeSpan maxAge)
        {
            // Cacheability must be set to public for SetETag to work; you could also
            // add the ETag header yourself with AppendHeader or AddHeader methods
            Response.Cache.SetCacheability(HttpCacheability.Public);
            Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
            Response.Cache.SetExpires(DateTime.UtcNow.Add(maxAge));
            Response.Cache.SetMaxAge(maxAge);
            Response.Cache.SetLastModified(lastModified);
            Response.Cache.SetETag(etag);
        }


        #endregion Methods
    }
}
