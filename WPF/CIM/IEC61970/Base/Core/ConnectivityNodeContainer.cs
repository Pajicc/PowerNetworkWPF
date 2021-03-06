///////////////////////////////////////////////////////////
//  ConnectivityNodeContainer.cs
//  Implementation of the Class ConnectivityNodeContainer
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
    /// A base class for all objects that may contain connectivity nodes or topological
    /// nodes.
    /// </summary>
    [Serializable()]
    public class ConnectivityNodeContainer : PowerSystemResource {

        /// <summary>
        /// Connectivity nodes which belong to this connectivity node container.
        /// </summary>
        public List<ConnectivityNode> ConnectivityNodes = new List<ConnectivityNode>();

        public ConnectivityNodeContainer(){

		}

		~ConnectivityNodeContainer(){

		}

	}//end ConnectivityNodeContainer

}//end namespace Core