///////////////////////////////////////////////////////////
//  IdentifiedObject.cs
//  Implementation of the Class IdentifiedObject
//  Generated by Enterprise Architect
//  Created on:      24-Aug-2016 11:29:39 PM
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CIM.IEC61970.Base.Core {
    /// <summary>
    /// This is a root class to provide common identification for all classes needing
    /// identification and naming attributes.
    /// </summary>

    [Serializable()]
    public class IdentifiedObject : ICloneable {

		/// <summary>
		/// The aliasName is free text human readable name of the object alternative to
		/// IdentifiedObject.name. It may be non unique and may not correlate to a naming
		/// hierarchy.
		/// The attribute aliasName is retained because of backwards compatibility between
		/// CIM relases. It is however recommended to replace aliasName with the Name class
		/// as aliasName is planned for retirement at a future time.
		/// </summary>
		public String aliasName;
		/// <summary>
		/// The description is a free human readable text describing or naming the object.
		/// It may be non unique and may not correlate to a naming hierarchy.
		/// </summary>
		public String description;
		/// <summary>
		/// Master resource identifier issued by a model authority. The mRID is globally
		/// unique within an exchange context. Global uniqueness is easily achieved by
		/// using a UUID,  as specified in RFC 4122, for the mRID.  The use of UUID is
		/// strongly recommended.
		/// For CIMXML data files in RDF syntax conforming to IEC 61970-552 Edition 1, the
		/// mRID is mapped to rdf:ID or rdf:about attributes that identify CIM object
		/// elements.
		/// </summary>
		public String mRID { get; set; }
        /// <summary>
        /// The name is any free human readable and possibly non unique text naming the
        /// object.
        /// </summary>
        public String name { get; set; }

        public IdentifiedObject(){

		}

		~IdentifiedObject(){

		}

        public virtual object Clone()
        {
            return null;
        }

    }//end IdentifiedObject

}//end namespace Core