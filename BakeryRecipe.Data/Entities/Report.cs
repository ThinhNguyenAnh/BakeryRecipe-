﻿using BakeryRecipe.Data.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BakeryRecipe.Data.Entities
{
    public class Report
    {
        public Guid UserId { get; set; }
        public int PostId { get; set; }
        public User User { get; set; }
        public Post Post { get; set; }

        public DateTime Date { get; set; }

        public ReportProblem ReportProblem { get; set; }
    }
}
