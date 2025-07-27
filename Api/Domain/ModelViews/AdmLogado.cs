using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ModelViews
{
    public class AdmLogado
    {
        public string Email { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Token { get; set; } = default!;

    }
}