using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLAE_Panorama_Tool.Core.DiffGenerators
{
    public enum DifferenceMode
    {
        Modification,
        Deletion,
        Insertion
    }

    public class Difference
    {
        #region Methods
        public Difference(string Class, string NewData, string OldData)
        {
            this.m_Class = Class;
            this.m_NewData = NewData;
            this.m_OldData = OldData;
        }

        public string Class { get { return m_Class; } }
        public string OldData { get { return m_OldData; } }
        public string NewData { get { return m_NewData; } }

        /// <summary>
        /// Returns mode of the difference.
        /// </summary>
        /// <returns>The mode.</returns>
        /// <throws>NullReferenceException when modification cannot be determined.</throws>
        public DifferenceMode Mode
        { get
            {
                if (m_OldData == null && m_NewData == null)
                    throw new NullReferenceException(String.Format("Could not determine modification in the class {0}", m_Class));
                if (m_OldData == null) // insertion
                    return DifferenceMode.Insertion;
                if (m_NewData == null) // deletion
                    return DifferenceMode.Deletion;
                if (m_OldData != m_NewData) // modification
                    return DifferenceMode.Modification;

                throw new NullReferenceException(String.Format("Could not determine modification in the class {0}", m_Class));
            }
        }
        #endregion

        #region Properties
        private string m_Class;
        private string m_NewData;
        private string m_OldData;
        #endregion
    }
}
