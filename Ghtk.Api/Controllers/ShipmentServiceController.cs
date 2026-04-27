using Ghtk.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ghtk.Api.Controllers
{
    [Route("/services/shipment")]
    [Authorize]
    public class ShipmentServiceController : ControllerBase
    {
        private readonly ILogger<ShipmentServiceController> logger;
        public ShipmentServiceController(ILogger<ShipmentServiceController> logger)
        {
            this.logger = logger;
        }
        [HttpPost]
        [Route("order")]
        
        public IActionResult SubmitOrder([FromBody] SubmitOrderRequest order)
        {
            logger.LogInformation("Received order submission: {@Order}", order);
            var  totalWeight = order.Products.Sum(p => p.Weight * p.Quantity);
            var  fee = 100000 + (int)(totalWeight * 10000); // Tính phí dựa trên trọng lượng tổng cộng của sản phẩm
            var  insuranceFee = (int)(order.Order.Value * 0.01); // Tính phí bảo hiểm là 1% của phí vận chuyển
            if(insuranceFee < 5000) insuranceFee = 5000; // Phí bảo hiểm tối thiểu là 5000
            var response = new SubmitOrderReponse
            {
               Message = "Order submitted successfully",
                Success = true,
                Order = new SubmitOrderResponseOrder
                {
                    PartnerId = order.Order.Id,
                    Label = "Label for order " + order.Order.Id,
                    Area = 1,
                    Fee = fee,
                    Products = order.Products,
                    StatusId = 1,
                    TrackingId = new Random().NextInt64(1000000000, 9999999999),
                    EstimatedPickTime = DateTimeOffset.UtcNow.AddHours(1).ToString("o"),
                    EstimatedDeliverTime = DateTimeOffset.UtcNow.AddHours(24).ToString("o"),//dòng này là lấy ngày + 24h trả vè to string theo định dạng ISO 8601 (o)  2026-04-28T08:00:00.0000000+00:00,
                    InsuranceFee = insuranceFee
                
                }
           };  
            return Ok(response);
        }
        [HttpGet]
        [Route("v2/{id}")]
        public IActionResult GetOrderStatus(string id) 
        {
            logger.LogInformation("Received request for order status with id: {Id}", id);
            var result  = new GetOrderStatusResponse
            {
                Message = "Order status retrieved successfully",
                Success = true,
                Order = new Order
                {
                    LabelId = id,
                    PartnerId = 123456,
                    Status = 2,
                    StatusText = "In Transit",
                    Created = DateTimeOffset.UtcNow.AddDays(-1),
                    Modified = DateTimeOffset.UtcNow,
                    Message = "Your order is on the way",
                    PickDate = DateTimeOffset.UtcNow.AddHours(-12),
                    DeliverDate = DateTimeOffset.UtcNow.AddHours(12),
                    CustomerFullname = "John Doe",
                    CustomerTel = "0123456789",
                    Address = "123 Main St, City, Country",
                    StorageDay = 0,
                    ShipMoney = 150000
                }
            };
            return Ok(result);
        }
        [HttpPost]
        [Route("cancel/{id}")]
        public IActionResult CancelOrder(string id)
        {
            logger.LogInformation("Received request to cancel order with id: {Id}", id);
            var result = new ApiResult
            {
                Message = $"Order {id} cancelled successfully",
                Success = true
            };
            return Ok(result);
        }


    }
}
