﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class ScheduleFrequency : DomainEntity
    {
        public virtual string FrequencyType { get; set; }
        public virtual int FrequencyValue { get; set; }
        public virtual int StartOn { get; set; }
    }
}