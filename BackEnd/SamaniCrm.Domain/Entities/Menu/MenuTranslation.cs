using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamaniCrm.Domain.Entities;

public class MenuTranslation : BaseTranslation
{
    public Guid MenuId { get; set; }


    public string Title { get; set; } = string.Empty!;

    public virtual Menu Menu { get; set; } = default!;



}
