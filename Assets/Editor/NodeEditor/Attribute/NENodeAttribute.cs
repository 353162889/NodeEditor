﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NENodeAttribute : Attribute
{
    public string name { get; private set; }
    public NENodeAttribute(string name = null)
    {
        this.name = name;
    }
}
