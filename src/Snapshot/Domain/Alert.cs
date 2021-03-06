﻿using Core.Domain;
using Domain.Enums;
using System;

namespace Domain
{
    public class Alert : DomainEntity
    {
        public virtual Guid OutpostId { get; set; }
        public virtual String OutpostName { get; set; }
        public virtual String Contact { get; set; }
        public virtual Guid ProductGroupId { get; set; }
        public virtual String ProductGroupName { get; set; }
        public virtual String LowLevelStock { get; set; }
        public virtual DateTime? LastUpdate { get; set; }
        public virtual Guid OutpostStockLevelId { get; set; }
        public virtual Client Client { get; set; }
        public virtual AlertType AlertType { get; set; }
    }
}
