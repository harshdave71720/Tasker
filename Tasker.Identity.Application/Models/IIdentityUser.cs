﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Tasker.Identity.Application.Models
{
    public interface IIdentityUser
    {
        public string Email { get; }
    }
}
