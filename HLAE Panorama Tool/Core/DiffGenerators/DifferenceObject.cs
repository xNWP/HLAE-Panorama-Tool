using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLAE_Panorama_Tool.Core.DiffGenerators
{
    public abstract class DifferenceObject
    {
        #region Methods
        protected abstract void Construct(string ChangedFilename, string StaticFilename);

        public List<Difference> Differences { get { return this.m_Differences; } }
        public string Filename{ get { return this.m_Filename; } }
        #endregion

        #region Properties
        protected List<Difference> m_Differences;
        protected string m_Filename;

        #endregion
    }
}
