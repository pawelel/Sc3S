﻿namespace Sc3S.Entities;
public class CommunicateSpace : BaseEntity
{
    public int SpaceId { get; set; }
    public virtual Space Space { get; set; } = new();
    public int CommunicateId { get; set; }
    public virtual Communicate Communicate { get; set; } = new();
}
