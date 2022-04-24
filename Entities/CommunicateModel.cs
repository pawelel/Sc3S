﻿namespace Sc3S.Entities;
public class CommunicateModel : BaseEntity
{
    public int ModelId { get; set; }
    public virtual Model Model { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
