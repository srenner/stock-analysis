using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StockLibrary.Models
{
    public class Fund
    {
        [Key]
        public string Symbol { get; set; }

        public string Name { get; set; }
        public string Exchange { get; set; }
        public bool IsActive { get; set; }
    }
}
