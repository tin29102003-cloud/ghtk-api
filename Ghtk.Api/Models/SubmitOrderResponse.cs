namespace Ghtk.Api.Models
{
    using System;
    using System.Collections.Generic;

    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Globalization;

    public  class SubmitOrderReponse: ApiResult
    {
       

        [JsonPropertyName("order")]
        public SubmitOrderResponseOrder? Order { get; set; } 
    }

    public  class SubmitOrderResponseOrder
    {
        [JsonPropertyName("partner_id")]
        public required string PartnerId { get; set; }

        [JsonPropertyName("label")]
        public string? Label { get; set; }

        [JsonPropertyName("area")]
       
        public int Area { get; set; }

        [JsonPropertyName("fee")]
       
        public double Fee { get; set; }

        [JsonPropertyName("insurance_fee")]
        
        public double InsuranceFee { get; set; }

        [JsonPropertyName("tracking_id")]
        public  required string TrackingId { get; set; }

        [JsonPropertyName("estimated_pick_time")]
        public string EstimatedPickTime { get; set; } = default!;

        [JsonPropertyName("estimated_deliver_time")]
        public string EstimatedDeliverTime { get; set; } = default!;

        [JsonPropertyName("products")]
        public ProductOrder[] Products { get; set; } = [];

        [JsonPropertyName("status_id")]
        public long StatusId { get; set; }
    }
}
