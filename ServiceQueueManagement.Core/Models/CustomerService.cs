﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ServiceQueueManagement.Core.Models
{

    //this table is used as a primary table to appoinment table(apponment is many to many with customer service and employee)
    //
    public class CustomerService
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int FkCustomerId { get; set; }
        public Customer Customer { get; set; }

        public int FkServiceId { get; set; }
        public Service Service { get; set; }



        //use to check this record has alredy assigned or not
        public bool? IsAssigned { get; set; }

        public virtual ICollection<Appoinment> Appoinment { get; set; }
    }
}
