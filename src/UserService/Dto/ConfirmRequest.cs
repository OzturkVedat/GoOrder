using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Dto
{
    public class ConfirmRequest
    {
        public string Email { get; set; }
        public string ConfirmationCode { get; set; }
    }

}
