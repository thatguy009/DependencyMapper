using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DependencyMapper.Models
{
    [Table("Systems")]
    public class System
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string Tags { get; set; }

        public List<System> Dependants { get; set; }
        public List<System> Dependancies { get; set; }

    }

    [NotMapped]
    public class Link
    {
        [JsonProperty("source")]
        public string Source { get; set; }
        [JsonProperty("target")]
        public string Target { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
    }

}