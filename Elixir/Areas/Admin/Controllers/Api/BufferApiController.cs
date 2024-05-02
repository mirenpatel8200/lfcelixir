using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Elixir.Attributes;
using Elixir.Contracts.Interfaces.Core;
using Elixir.Controllers.Base;
using Elixir.Models.Core.BufferApi;
using Elixir.Models.Enums;
using Elixir.Models.Json;
using Elixir.Models.Utils;

namespace Elixir.Areas.Admin.Controllers.Api
{
    [CustomAuthenticationFilter]
    [CustomAuthorize(Roles.Administrator, Roles.Editor, Roles.Writer)]
    public class BufferApiController : JsonController
    {
        private readonly IBufferClient _bufferClient;

        public BufferApiController(IBufferClient bufferClient)
        {
            _bufferClient = bufferClient;
        }

        [HttpGet]
        public async Task<ActionResult> PendingPosts()
        {
            try
            {
                var ids = await _bufferClient.WithCredentials(Request.Url.Authority).GetProfiles();

                var tasks = ids.Select(x => _bufferClient.WithCredentials(Request.Url.Authority).GetPendingUpdates(x.Key));
                var rJson = await Task.WhenAll(tasks);
                var mJson = rJson.Select((x, i) => $"Profile id '{ids.ElementAt(i).Key}': {x}");
                var allJson = string.Join("\n", mJson);

                return Json(new JsonActionResult<string>(true, allJson), JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new JsonActionResult(false, e.Message), JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// Creates same posts for all available profiles.
        /// </summary>
        /// <param name="postText"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreatePost(string postText)
        {
            if (Request.UrlReferrer.Authority.Equals("liveforever.club") == false)
            {
                return Json(new JsonActionResult(true, AppConstants.Messages.SocialPostsOnDev, true));
            }
            try
            {
                var idsMap = await _bufferClient.WithCredentials(Request.Url.Authority).GetProfiles();
                var ids = idsMap.Keys.ToArray();
                var r = await _bufferClient.WithCredentials(Request.Url.Authority).CreatePost(postText, ids);

                return Json(new JsonActionResult<CreatePostsResult>(true, r));
            }
            catch (Exception e)
            {
                return Json(new JsonActionResult(false, e.Message));
            }        
        }

        /// <summary>
        /// Creates different posts for profiles based on service names.
        /// </summary>
        /// <param name="servicesPosts">Mapping of services to posts that will be created.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreatePosts(Dictionary<string, string> servicesPosts)
        {
            if (Request.UrlReferrer.Authority.Equals("liveforever.club") == false)
            {
                return Json(new JsonActionResult(true, AppConstants.Messages.SocialPostsOnDev, true));
            }
            try
            {
                var idsServices = await _bufferClient.WithCredentials(Request.Url.Authority).GetProfiles();

                Dictionary<string, string> servicesIds;
                try
                {
                    servicesIds = idsServices.ToDictionary(x => x.Value, x => x.Key);
                }
                catch (ArgumentException e)
                {
                    throw new Exception("Unable to map service names to ids because there are more than 1 id per service. Please contact developers to fix this issue.", e);
                }

                var notSupportedServices = new List<string>();
                var idsPosts = new Dictionary<string, string>();
                foreach (var servicePost in servicesPosts)
                {
                    if (servicesIds.ContainsKey(servicePost.Key))
                        idsPosts.Add(servicesIds[servicePost.Key], servicePost.Value);
                    else
                        notSupportedServices.Add(servicePost.Key);
                }

                var r = await _bufferClient.WithCredentials(Request.Url.Authority).CreatePosts(idsPosts);
                r.AddRange(notSupportedServices.Select(s => new CreatePostResult() { IsSuccess = false, ServiceName = s }));

                return Json(new JsonActionResult<CreatePostsResult>(true, r));
            }
            catch (Exception e)
            {
                return Json(new JsonActionResult(false, e.Message));
            }
        }

        public ActionResult Auth()
        {
            //var r = _bufferClient.GetProfiles();
            return Content("Authed");
        }
    }
}