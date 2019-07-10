﻿using System;
using System.Collections.Generic;
using System.Text;

namespace DependencyInjection.Autoregister.Abstraction.Models
{
    public class ServiceRegistration
    {
        public Type ServiceType { get; set; }
        public Type ImplementationType { get; set; }
        public ServiceRegistrationType Type { get; set; }
    }
}
