﻿using Board.BusinessLogic.Types.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Board.BusinessLogic.Types.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class SyncWithModelAttribute : Attribute
    {
        public SyncWithModelAttribute(string modelProperty, ModelSyncDirection direction = ModelSyncDirection.BothWays)
        {
            ModelProperty = modelProperty;
            Direction = direction;
        }

        public string ModelProperty { get; }
        public ModelSyncDirection Direction { get; }
    }
}
