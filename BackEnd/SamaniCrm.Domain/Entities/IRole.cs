using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;
public interface IRole
{
    public ICollection<RolePermission> RolePermissions { get; set; }

}

