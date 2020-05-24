using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Checkout.PaymentGateway.AcquiringBankSimulator
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PaymentResponse))]
        public async Task<IActionResult> Post([FromBody, Required] PaymentRequest request)
        {
            return request.Amount switch
            {
                999 => Ok(new PaymentResponse(Status.Declined)),
                _ => Ok(new PaymentResponse(Status.Authorized))
            };
        }
    }
}