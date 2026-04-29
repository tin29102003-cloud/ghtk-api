using Ghtk.Api.Models;
using Ghtk.Repository.Abstractions;
using Ghtk.Repository.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Ghtk.Api.Controllers
{
    [Route("/services/shipment")]
    [Authorize]
    public class ShipmentServiceController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ShipmentServiceController> logger;
        public ShipmentServiceController(IOrderRepository orderRepository,ILogger<ShipmentServiceController> logger)
        {
            _orderRepository = orderRepository; 
            this.logger = logger;
        }
        [HttpPost]
        [Route("order")]
        
        public async Task<IActionResult> SubmitOrder([FromBody] SubmitOrderRequest order)
        {
            logger.LogInformation("Received order submission: {@Order}", order);
            var partnerId = User.FindFirst("PartnerId")?.Value;
            if(string.IsNullOrEmpty(partnerId))
            {
                logger.LogWarning("PartnerId claim is missing in the token.");
                return Unauthorized();
            }
            var orderEntity = new OrderEntity
            {
               Status = 1, // Trạng thái mới tạo
                PartnerId = partnerId,
                PickName = order.Order.PickName,
                PickAddress = order.Order.PickAddress,
                PickProvince = order.Order.PickProvince,
                PickDistrict = order.Order.PickDistrict,
                PickWard = order.Order.PickWard,
                PickTel = order.Order.PickTel,
                Tel = order.Order.Tel,
                Name = order.Order.Name,
                Address = order.Order.Address,
                IsFreeship = order.Order.IsFreeship,
                PickDate = order.Order.PickDate,
                PickMoney = order.Order.PickMoney,
                Note = order.Order.Note,
                Value = order.Order.Value,
                Id= Guid.NewGuid().ToString(),
                Transport = order.Order.Transport,
                PickOption = order.Order.PickOption,
                District = order.Order.District,
                Hamlet = order.Order.Hamlet,
                Province = order.Order.Province,
                Ward = order.Order.Ward,
                TrackingId = Guid.NewGuid().ToString(),
                 Products = order.Products.Select(p => new ProductEntity
                {
                    Name = p.Name ?? throw new Exception(),
                    Weight = p.Weight,
                    Quantity = p.Quantity,
                    ProductCode = p.ProductCode
                }).ToList(),
                GamSolutions = order.Order.GamSolutions.Select(g => new GamSolutionEntity
                {
                    SolutionId = g.SolutionId
                }).ToArray()
                

            };
            await _orderRepository.CreateOrderAsync(orderEntity);//hàm created này đang trả  void nên ko trả về biến dc
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
                    //cách xử lý đơn gian viết như vậy
                    //PartnerId = Request.Headers["X-Client-Source"].First() ?? throw new Exception(),
                    PartnerId = partnerId,
                    Label = "Label for order " + order.Order.Id,
                    Area = 1,
                    Fee = fee,
                    Products = order.Products.Select(p => new ProductOrder()
                    {
                      Name = p.Name,
                        Weight = p.Weight,
                        Quantity = p.Quantity,
                        ProductCode = p.ProductCode
                    }).ToArray(),
                    StatusId = orderEntity.Status,
                    TrackingId = orderEntity.TrackingId,//cái này nên để về guide vì thằng mongo ko có cơ chế auto increment nên mình phải tự sinh ra 1 cái tracking id nào đó, ở đây mình sinh ra 1 guid ngẫu nhiên
                    EstimatedPickTime = DateTimeOffset.UtcNow.AddHours(1).ToString("o"),
                    EstimatedDeliverTime = DateTimeOffset.UtcNow.AddHours(24).ToString("o"),//dòng này là lấy ngày + 24h trả vè to string theo định dạng ISO 8601 (o)  2026-04-28T08:00:00.0000000+00:00,
                    InsuranceFee = insuranceFee,
                    

                }
            };
            return Ok(response);
        }
        [HttpGet]
        [Route("v2/{id}")]
        public async Task<IActionResult> GetOrderStatus(string id) 
        {
            logger.LogInformation("Received request for order status with id: {Id}", id);
            var partnerId = User.FindFirst("PartnerId")?.Value;
            if (string.IsNullOrEmpty(partnerId))
            {
                logger.LogWarning("PartnerId claim is missing in the token.");
                return Unauthorized();
            }
            var order = await _orderRepository.GetOrderByIdAsync(id, partnerId);
            if(order == null)
            {
                logger.LogWarning("Order with id {Id} not found for partner {PartnerId}.", id, partnerId);
                return NotFound(new ApiResult
                {
                    Message = $"Order with trackingid {id} not found",
                    Success = false
                });

            }
            var totalWeight = order.Products.Sum(p => p.Weight * p.Quantity);
            var result  = new GetOrderStatusResponse
            {
                Message = "Order status retrieved successfully",
                Success = true,
                Order = new Order
                {
                    
                    LabelId = "Label for order " + order.Id,
                    PartnerId = partnerId,
                    Status = order.Status,
                    StatusText = "In Transit",
                    Created = DateTimeOffset.UtcNow.AddDays(-1),
                    Modified = DateTimeOffset.UtcNow,
                    Message = "Your order is on the way",
                    PickDate = DateTimeOffset.UtcNow.AddHours(-12),
                    DeliverDate = DateTimeOffset.UtcNow.AddHours(12),
                    CustomerFullname = "John Doe",
                    CustomerTel = "0123456789",
                    Address = order.Address ?? "123 Main St, City, Country",
                    StorageDay = 0,
                    ShipMoney = 150000,
                    Value = order.Value,
                    Insurance = (int)(order.Value * 0.01),
                    IsFreeship = order.IsFreeship,
                    PickMoney = order.PickMoney,
                    Weight = (int)totalWeight
                    
                }
            };
            return Ok(result);
        }
        [HttpPost]
        [Route("cancel/{id}")]
        public async Task<IActionResult> CancelOrder(string id, [FromQuery] int status)
        {
            logger.LogInformation("Received request to cancel order with id: {Id}", id);
            var partnerId = User.FindFirst("PartnerId")?.Value;
            if (string.IsNullOrEmpty(partnerId))
            {
                logger.LogWarning("PartnerId claim is missing in the token.");
                return Unauthorized();
            }
            var order = _orderRepository.GetOrderByIdAsync(id, partnerId);
            if (order == null) { 
                return NotFound(new ApiResult
                {
                    Message = $"Order with trackingid {id} not found",
                    Success = false
                });
            }
            var b = await _orderRepository.CancelOrderAsync(id, partnerId, status);
            if (b!)
            {
                return BadRequest(new ApiResult
                {
                    Message = $"Failed to cancel order {id}",
                    Success = false
                });
            }
            var result = new ApiResult
            {
                Message = $"Order {id} cancelled successfully",
                Success = true
            };
            return Ok(result);
        }


    }
}
