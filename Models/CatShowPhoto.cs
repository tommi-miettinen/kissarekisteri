﻿using System.Text.Json.Serialization;

namespace Kissarekisteri.Models
{
    public class CatShowPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public int CatShowId { get; set; }

        [JsonIgnore]
        public CatShow CatShow { get; set; }
    }
}
