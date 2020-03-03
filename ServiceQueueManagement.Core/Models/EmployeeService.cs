﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ServiceQueueManagement.Core.Models
{
    public class EmployeeService
    {
        public int FkEmployeeId { get; set; }
        public Employee Employee { get; set; }

        public int FkServiceId { get; set; }

        public Service Service { get; set; }
    }
}
