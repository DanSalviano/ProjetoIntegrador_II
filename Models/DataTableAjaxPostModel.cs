using Newtonsoft.Json;
using System.Collections.Generic;

namespace PizzaDelivery.Models
{
    public class DataTableAjaxPostModel
    {
        [JsonProperty("draw")]
        public int Draw { get; set; }

        [JsonProperty("start")]
        public int Start { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("columns")]
        public List<Column> Columns { get; set; }

        [JsonProperty("search")]
        public Search Search { get; set; }

        [JsonProperty("order")]
        public List<Order> Order { get; set; }
    }

    public class Column
    {
        [JsonProperty("data")]
        public string Data { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("searchable")]
        public bool Searchable { get; set; }

        [JsonProperty("orderable")]
        public bool Orderable { get; set; }

        [JsonProperty("search")]
        public Search Search { get; set; }
    }

    public class Search
    {
        [JsonProperty("value")]
        public string Value { get; set; }

        [JsonProperty("regex")]
        public string Regex { get; set; }
    }

    public class Order
    {
        [JsonProperty("column")]
        public int Column { get; set; }

        [JsonProperty("dir")]
        public string Dir { get; set; }
    }
}
