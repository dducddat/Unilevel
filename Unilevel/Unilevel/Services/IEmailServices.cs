using Unilevel.Models;

namespace Unilevel.Services
{
    public interface IEmailServices
    {
        public void SendEmail(EmailDTO email);
    }
}
