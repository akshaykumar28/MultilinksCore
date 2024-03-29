﻿using Newtonsoft.Json;

namespace Multilinks.Core.Models
{
   public class FormFieldOption
   {
      [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
      public string Label { get; set; }

      public object Value { get; set; }
   }
}
