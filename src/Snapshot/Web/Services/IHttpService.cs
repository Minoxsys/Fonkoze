﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Web.Services
{
    public interface IHttpService
    {
        String Post(String url, String data);
        string Post(string data);
    }
}
