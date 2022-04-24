﻿namespace Sc3S.Entities;
public class ModelParameter : BaseEntity
{
    public int ModelId { get; set; }
    public virtual Model Model { get; set; } = new();
    public virtual Parameter Parameter { get; set; } = new();
    public int ParameterId { get; set; }
    public string Value { get; set; } = string.Empty;
}
