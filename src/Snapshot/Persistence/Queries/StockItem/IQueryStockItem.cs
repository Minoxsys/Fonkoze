﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Domain;

namespace Persistence.Queries.StockItems
{
    public interface IQueryStockItem
    {
        IQueryable<StockItem> GetAll();
    }
}
