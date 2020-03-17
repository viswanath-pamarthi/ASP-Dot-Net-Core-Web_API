﻿using RestApi.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApi.Models
{
    public class Room:Resource
    {
        [Sortable]//crate nw attribute sortable
        public string Name { get; set; }
        [Sortable(Default = true)]
        public Decimal Rate { get; set; }

    }
}
