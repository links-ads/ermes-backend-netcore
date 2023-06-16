using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Ermes.MapRequests
{
    public class BoundaryCondition
    {
        /// <summary>
        /// Time for the boundary conditions, that must be in minutes
        /// </summary>
        public int Time { get; set; }
        /// <summary>
        /// Wind direction in degrees (0 is North, clockwise)
        /// </summary>
        public int WindDirection { get; set; }
        /// <summary>
        /// Wind speed in Km/
        /// </summary>
        public int WindSpeed { get; set; }
        /// <summary>
        /// Fuel moisture content, from 0 to 100
        /// </summary>
        [Range(0, 100)]
        public int Moisture { get; set; }

        /// <summary>
        /// Key is one of the value contained in FireBreakType enum
        /// Value is a LINESTRING
        /// </summary>
        public Dictionary<string,string> FireBreak { get; set; }

    }
}
