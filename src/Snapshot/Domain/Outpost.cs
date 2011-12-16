﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Domain;

namespace Domain
{
    public class Outpost : DomainEntity
    {
        public virtual string Name { get; set; }
        public virtual string OutpostType { get; set; }
        public virtual string Email { get; set; }
        public virtual string MainMobileNumber { get; set; }
        public virtual string Longitude { get; set; }
        public virtual string Latitude { get; set; }
        public virtual Domain.Country Country { get; set; }
        public virtual Domain.Region Region { get; set; }
        public virtual Domain.District District { get; set; }
        public virtual Domain.Client Client { get; set; }
        public virtual IList<Domain.MobilePhone> MobilePhones { get; set; }


        public Outpost()
        {
            //MobilePhones = new List<MobilePhone>();
        }

        public virtual void AddMobilePhone(MobilePhone mobilePhone)
        {
            //MobilePhones.Add(mobilePhone);
        }
    }
}
