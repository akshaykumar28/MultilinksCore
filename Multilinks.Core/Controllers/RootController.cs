﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Multilinks.Core.Infrastructure;
using Multilinks.Core.Models;

namespace Multilinks.Core.Controllers
{
   [Route("/")]
   [ApiVersion("1.0")]
   [Authorize]
   public class RootController : Controller
   {
      [HttpGet(Name = nameof(GetRoot))]
      [ResponseCache(CacheProfileName = "Static")]
      [Etag]
      [AllowAnonymous]
      public IActionResult GetRoot()
      {
         var response = new RootResponse
         {
            Self = Link.To(nameof(GetRoot)),
            Endpoints = Link.To(nameof(EndpointsController.GetEndpointsAsync))
         };

         if(!Request.GetEtagHandler().NoneMatch(response))
         {
            return StatusCode(304, response);
         }

         return Ok(response);
      }
   }
}
