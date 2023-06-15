using System;

namespace Byndyusoft.Example.Domain.Dtos
{
    public class ExampleMessageDto
    {
        public string Text { get; set; } = default!;
        
        public Guid Guid { get; set;}
    }
}