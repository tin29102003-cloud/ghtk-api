using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Ghtk.Repository.Abstractions.Entities
{
    public class OrderEntity
    {
        public required string Id { get; set; }

        public required string TrackingId { get; set; } 
        public string PartnerId { get; set; } = default!;
        public int Status { get; set; } 

        public required string PickName { get; set; }

        public required string PickAddress { get; set; }

        public required string PickProvince { get; set; }

        public required string PickDistrict { get; set; }

        public required string PickWard { get; set; }

        public required string PickTel { get; set; }

        public required string Tel { get; set; }

        public string? Name { get; set; }

        public string? Address { get; set; }

        public string? Province { get; set; }

        public string? District { get; set; }

        public string? Ward { get; set; }

        public string? Hamlet { get; set; }

        public int IsFreeship { get; set; }

        public DateTimeOffset PickDate { get; set; }

        public int PickMoney { get; set; }

        public string? Note { get; set; }

        public int Value { get; set; }

        public string? Transport { get; set; }

        public string? PickOption { get; set; }
 

        public GamSolutionEntity[] GamSolutions { get; set; } = default!;
        public List<ProductEntity> Products { get; set; } = [];
    }
}
