using System;
using System.Collections.Generic;
using System.Text;

namespace SampleGame
{
    public class Output
    {
        //we might enum a lot of things, but don't enum Category, it will be read by client apps.
        public string Category { get; set; }
        public string Data { get; set; }
    }

    public class OutputCategory
    {
        public const string Text = nameof(Text);
    }

}
