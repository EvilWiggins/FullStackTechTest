using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ImportResult
    {
        public int TotalRecords { get; set; }
        public int Inserted { get; set; }
        public int SkippedExisting { get; set; }
        public List<string> Errors { get; } = new();
        public bool Success => Errors.Count == 0;
    }
}
