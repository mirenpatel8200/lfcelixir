using PostmarkDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elixir.Utils
{
    public static class EmailService
    {
        public static async Task<bool> SendEmail(TemplatedPostmarkMessage message)
        {
            try
            {
                var client = new PostmarkClient("3f3f06fd-299e-48bd-92f2-04e66f812fb5");
                var sendResult = await client.SendMessageAsync(message);

                if (sendResult.Status == PostmarkStatus.Success)
                {
                    /* Handle success */
                    return true;
                }
                else
                {
                    /* Resolve issue.*/
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Manage scenario if postmark throws exception
                return false;
            }
        }
    }
}
