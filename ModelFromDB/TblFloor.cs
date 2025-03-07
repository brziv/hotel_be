using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace hotel_be.ModelFromDB;

[Table("tbl_Floors")]
public partial class TblFloor
{
    [Key]
    [Column("f_FloorID")]
    public Guid FFloorId { get; set; }

    [Column("f_Floor")]
    [StringLength(10)]
    public string FFloor { get; set; } = null!;

    [InverseProperty("RFloor")]
    public virtual ICollection<TblRoom> TblRooms { get; set; } = new List<TblRoom>();
}
