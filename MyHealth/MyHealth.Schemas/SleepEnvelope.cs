using System;
using System.Collections.Generic;
using System.Text;

namespace MyHealth.Schemas
{
    public class SleepEnvelope
    {
        public string SleepId { get; set; }
        public Sleep Sleep { get; set; }
        public string DocumentType { get; set; }
        public string FileName { get; set; }
    }
}
