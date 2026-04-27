namespace Ghtk.Api.Models
{
    using System;
    using System.Text.Json.Serialization;

    public partial class GetOrderStatusResponse: ApiResult
    {
  
        [JsonPropertyName("order")]
        public Order? Order { get; set; } 
    }

    public partial class Order
    {
        [JsonPropertyName("label_id")]
        public required string LabelId { get; set; }  

        [JsonPropertyName("partner_id")]

        public int PartnerId { get; set; }

        [JsonPropertyName("status")]
       
        public int Status { get; set; }

        [JsonPropertyName("status_text")]
        public string StatusText { get; set; } = default!;

        [JsonPropertyName("created")]
        public DateTimeOffset Created { get; set; }

        [JsonPropertyName("modified")]
        public DateTimeOffset Modified { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("pick_date")]
        public DateTimeOffset PickDate { get; set; }

        [JsonPropertyName("deliver_date")]
        public DateTimeOffset DeliverDate { get; set; }

        [JsonPropertyName("customer_fullname")]
        public required string CustomerFullname { get; set; } 

        [JsonPropertyName("customer_tel")]
        public string CustomerTel { get; set; } = default!;

        [JsonPropertyName("address")]
        public string Address { get; set; } = default!;

        [JsonPropertyName("storage_day")]

        public int StorageDay { get; set; }

        [JsonPropertyName("ship_money")]
       
        public int ShipMoney { get; set; }

        [JsonPropertyName("insurance")]

        public int Insurance { get; set; }

        [JsonPropertyName("value")]

        public int Value { get; set; }

        [JsonPropertyName("weight")]
        
        public int Weight { get; set; }

        [JsonPropertyName("pick_money")]
        public int PickMoney { get; set; }

        [JsonPropertyName("is_freeship")]
   
        public int IsFreeship { get; set; }
    }
}
