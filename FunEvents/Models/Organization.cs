﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents.Models
{
    public class Organization
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganizationNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public bool IsVerified { get; set; }

        [InverseProperty("ManagerInOrganizations")]
        public ICollection<AppUser> OrganizationManagers { get; set; }

        [InverseProperty("AssistantInOrganizations")]
        public ICollection<AppUser> OrganizationAssistants { get; set; }
    }
}
