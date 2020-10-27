using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace InLooxApiExamples.Examples
{
    class Example
    {
        public string Description { get; set; }
        public Func<Default.Container, Task> Action { get; set; }
    }
}
