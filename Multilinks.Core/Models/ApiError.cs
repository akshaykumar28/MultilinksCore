﻿using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Linq;

namespace Multilinks.Core.Models
{
   public class ApiError
   {
      public string Message { get; set; }

      public ApiError()
      {
      }

      public ApiError(string message)
      {
         Message = message;
      }

      public ApiError(ModelStateDictionary modelState)
      {
         Message = "Invalid parameters.";
         Detail = modelState
             .FirstOrDefault(x => x.Value.Errors.Any()).Value.Errors
             .FirstOrDefault().ErrorMessage;
      }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Detail { get; set; }

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
      [DefaultValue("")]
      public string StackTrace { get; set; }
   }
}
