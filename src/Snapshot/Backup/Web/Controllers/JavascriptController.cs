using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using Core.Services;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using Web.Bootstrap;
using Web.Services;

namespace Web.Controllers
{
    /// <summary>
    /// This controller is responsible for compacting scripts and versioning
    /// </summary>
    public class JavascriptController : Controller
    {
        #region Constants
        private const string IF_NONE_MATCH_HEADER = "If-None-Match";
        private const string LAST_MODIFIED_SINCE_HEADER = "If-Modified-Since";
        private const string JS_FOLDER = "~/Assets/js/";

        #endregion

        #region Fields

        private readonly IJavaScriptProviderService scriptProviderService;
        public IPathService PathService {get; set;}
        public IETagService ETagService {get; set;}

        #endregion

        #region Ctors


        public JavascriptController(IJavaScriptProviderService scriptProviderService)
        {
            this.scriptProviderService = scriptProviderService;
        }

        #endregion

        #region Actions
        /// <summary>
        /// Returns the script
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public ActionResult Index(string group)
        {
            string absolutePath = Server.MapPath(JS_FOLDER);

            if (!PathService.Exists(absolutePath))
            {
                Response.StatusCode = (int)HttpStatusCode.NotFound;
                Response.SuppressContent = true;
                return new EmptyResult();
            }
                       
            // 304 (If-Last-Modified)
            DateTime lLastModified = PathService.GetLastWriteTime(absolutePath);
            if (BrowserIsRequestingFileUnmodifiedSince(lLastModified))
            {
                Response.StatusCode = (int)HttpStatusCode.NotModified;
                Response.SuppressContent = true;
                return new EmptyResult();
            }

            // 304 (If-None-Match), a better test of uniqueness than modified date
            string lEtag = ETagService.Generate(absolutePath);
            if (BrowserIsRequestingFileIdentifiedBy(lEtag))
            {
                Response.StatusCode = (int)HttpStatusCode.NotModified;
                Response.SuppressContent = true;
                return new EmptyResult();
            }
            // 200 - OK
            AddCachingHeaders(lEtag, lLastModified, AppSettings.StaticFileHttpMaxAge);

            string content = string.Empty;
            try
            {
                content = scriptProviderService.GetScript(group);
            }
            catch (Exception ex)
            {
                content = ex.ToString() + ex.StackTrace + ex.Source;
            }


            return new ContentResult
            {
                Content = content,
                ContentEncoding = Encoding.UTF8,
                ContentType = "text/javascript"
            };
        }

        #endregion Actions

        #region Methods

        private bool BrowserIsRequestingFileIdentifiedBy(string etag)
        {
            if (Request.Headers[IF_NONE_MATCH_HEADER] == null)
            {
                return false;
            }

            string lIfNoneMatch = Request.Headers[IF_NONE_MATCH_HEADER];

            return lIfNoneMatch.Equals(etag, StringComparison.InvariantCultureIgnoreCase);
        }

        private bool BrowserIsRequestingFileUnmodifiedSince(DateTime lastModified)
        {
            if (Request.Headers[LAST_MODIFIED_SINCE_HEADER] == null)
            {
                return false;
            }

            // Header values may have additional attributes separated by semi-colons
            string ifModifiedSince = Request.Headers[LAST_MODIFIED_SINCE_HEADER];
            if (ifModifiedSince.IndexOf(";") > -1)
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
