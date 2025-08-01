﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.DTO.CompanieDto
{
    public class CompanieResponseDto
    {
        public Guid Id { get; set; }
        public string CompanyName { get; set; }
        public string EmailCompany { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public Guid ActivityTypeId { get; set; }
        public string ActivityTypeName { get; set; }
    }
}
