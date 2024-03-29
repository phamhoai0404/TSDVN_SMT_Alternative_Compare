﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMT_Picklist_Compare.Model
{
   public class Picklist
    {
        public string plItem { get; set; }
        public string plComment { get; set; }

        public Picklist()
        {

        }
        public Picklist(string item, string comment)
        {
            this.plItem = item;
            this.plComment = comment;
        }
        public Picklist (Picklist s)
        {
            this.plItem = s.plItem;
            this.plComment = s.plComment;
        }
    }

    /// <summary>
    /// Dung de kiem tra 1 hay 2
    /// </summary>
    public class PicklistType : Picklist
    {
        public bool isFirst { get; set; }
        public PicklistType(Picklist s, bool isFirst)
        {
            this.plComment = s.plComment;
            this.plItem = s.plItem;
            this.isFirst = isFirst;
        }
    }
}
