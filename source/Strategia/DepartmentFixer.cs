﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using KSP;
using Strategies;

namespace Strategia
{
    /// <summary>
    /// Special MonoBehaviour to fix up the departments.
    /// </summary>
    [KSPAddon(KSPAddon.Startup.SpaceCentre, false)]
    public class DepartmentFixer : MonoBehaviour
    {
        public void Awake()
        {
        }

        public void Update()
        {
            if (FixDepartments())
            {
                Destroy(this);
            }
        }

        public bool FixDepartments()
        {
            // Wait for the strategy system to get loaded
            if (StrategySystem.Instance == null)
            {
                return false;
            }

            // Go and find the config class
            FieldInfo configField = typeof(StrategySystem).GetFields(BindingFlags.Instance | BindingFlags.NonPublic).
                Where(fi => fi.FieldType == typeof(StrategySystemConfig)).First();
            StrategySystemConfig config = (StrategySystemConfig)configField.GetValue(StrategySystem.Instance);

            DepartmentConfig gene = null;
            DepartmentConfig wernher = null;

            // Re-order stuff
            if (wernher != null)
            {
                config.Departments.Remove(wernher);
                config.Departments.Insert(0, wernher);
            }
            if (gene != null)
            {
                config.Departments.Remove(gene);
                config.Departments.Insert(0, gene);
            }

            return true;
        }
    }
}
