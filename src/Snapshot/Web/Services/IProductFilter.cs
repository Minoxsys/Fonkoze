﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain;

namespace Web.Services
{
    public interface IProductFilter
    {
        bool CheckProduct(Product product);
    }
}