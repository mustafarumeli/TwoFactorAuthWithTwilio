using System;
using System.Collections.Generic;

namespace UserLoginWithTwiloo.Models
{
    public partial class Verification
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public DateTime ValidUntil { get; set; } = DateTime.Now.AddMinutes(1);

    }
}
