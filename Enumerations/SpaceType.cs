﻿
using System.ComponentModel.DataAnnotations;
namespace Sc3S.Enumerations;
public enum SpaceType
{
    [Display(Name = "Nieznana")]
    Unknown,
    [Display(Name = "Linia")]
    Line,
    [Display(Name = "Pomieszczenie")]
    Room,
    [Display(Name = "Magazyn")]
    Storage
}
