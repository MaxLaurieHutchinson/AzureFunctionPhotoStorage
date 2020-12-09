using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoragePhoto.Models
{
    public class PhotoUploadModel
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("tags")]
        public string[] Tags { get; set; }

        [JsonProperty("photo")]
        public string Photo { get; set; }
    }
}
