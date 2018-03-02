///////////////////////////////////////////////////////////
//  BaseVoltage.cs
//  Implementation of the Class BaseVoltage
//  Generated by Enterprise Architect
//  Created on:      24-Aug-2016 11:29:39 PM
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;



using CIM.IEC61970.Base.Core;
namespace CIM.IEC61970.Base.Core {
    /// <summary>
    /// Defines a system base voltage which is referenced.
    /// </summary>
    [Serializable()]
    public class BaseVoltage : IdentifiedObject {

		/// <summary>
		/// The power system resource's base voltage.
		/// </summary>
		public float nominalVoltage;
		/// <summary>
		/// All conducting equipment with this base voltage.  Use only when there is no
		/// voltage level container used and only one base voltage applies.  For example,
		/// not used for transformers.
		/// </summary>
		public List<ConductingEquipment> ConductingEquipment;

		public BaseVoltage(){

		}

		~BaseVoltage(){

		}
        public override string ToString()
        {
            return nominalVoltage.ToString();
        }

    }//end BaseVoltage

}//end namespace Core