using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SendEmailMassTransit.Common.Dto;
using SendEmailMassTransit.Common.Services;

namespace SendEmailMassTransit.API.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class MailController:ControllerBase
    {
        private readonly IMailServices _mailServices;

        public MailController(IMailServices mailServices)
        {
            _mailServices = mailServices;
        }
        
        [HttpPut]
        public async Task<IActionResult> Put(EmailDto product, CancellationToken cancellationToken)
        {
            await _mailServices.Put(product, cancellationToken);
            return Ok();
        }
    }
}