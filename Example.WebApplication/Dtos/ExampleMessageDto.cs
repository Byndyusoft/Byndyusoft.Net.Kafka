using System;

namespace Byndyusoft.Example.WebApplication.Dtos;

public class ExampleMessageDto
{
    public string Text { get; set; } = default!;
        
    public Guid Guid { get; set;}
}