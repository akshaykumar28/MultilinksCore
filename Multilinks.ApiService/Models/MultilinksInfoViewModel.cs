﻿namespace Multilinks.ApiService.Models
{
   public class MultilinksInfoViewModel : Resource
   {
      public string Title { get; set; }
      public string Tagline { get; set; }
      public string Email { get; set; }
      public string Website { get; set; }
      public Address Location { get; set; }
   }

   public class Address
   {
      public string City { get; set; }
      public string Country { get; set; }
   }
}