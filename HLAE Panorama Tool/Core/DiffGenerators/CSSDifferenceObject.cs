using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLAE_Panorama_Tool.Core.DiffGenerators
{
    public class CSSDifferenceObject : DifferenceObject
    {
        #region Methods
        private CSSDifferenceObject() { }

        public CSSDifferenceObject(string ChangedFilename, string StaticFilename)
        {
            Construct(ChangedFilename, StaticFilename);
        }

        public CSSDifferenceObject(Stream ChangedFilestream, Stream StaticFilestream)
        {
            Construct(ChangedFilestream, StaticFilestream);
        }

        protected override void Construct(string ChangedFilename, string StaticFilename)
        {
            this.m_Filename = ChangedFilename;
            this.m_Differences = new List<Difference>();

            StreamReader ChangedStream = new StreamReader(ChangedFilename);
            StreamReader StaticStream = new StreamReader(StaticFilename);

            Dictionary<string, string> CSClasses = CSSFindClasses(ChangedStream);
            Dictionary<string, string> SSClasses = CSSFindClasses(StaticStream);

            ChangedStream.Close();
            StaticStream.Close();

            // Check where differences occur
            foreach (KeyValuePair<string, string> entry in CSClasses)
            {
                // Key exists in both
                if(SSClasses.ContainsKey(entry.Key))
                {
                    // differences exist
                    if(SSClasses[entry.Key] != entry.Value)
                    {
                        Difference nd = new Difference(entry.Key, entry.Value, SSClasses[entry.Key]);
                        this.m_Differences.Add(nd);
                    }
                    // remove from snapshots classes
                    SSClasses.Remove(entry.Key);
                }
                else
                {
                    // This is a new class
                    Difference nc = new Difference(entry.Key, entry.Value, null);
                    this.m_Differences.Add(nc);
                }
            }

            // Remaining snapshot classes must be deletions
            foreach (KeyValuePair<string, string> entry in SSClasses)
                this.m_Differences.Add(new Difference(entry.Key, null, entry.Value));
        }

        protected override void Construct(Stream ChangedFilestream, Stream StaticFilestream)
        {
            // This function does not follow DRY, will reimplement later :-b
            this.m_Filename = ((FileStream)ChangedFilestream).Name;
            this.m_Differences = new List<Difference>();

            StreamReader ChangedStream = new StreamReader(ChangedFilestream);
            StreamReader StaticStream = new StreamReader(StaticFilestream);

            Dictionary<string, string> CSClasses = CSSFindClasses(ChangedStream);
            Dictionary<string, string> SSClasses = CSSFindClasses(StaticStream);

            ChangedStream.Close();
            StaticStream.Close();

            // Check where differences occur
            foreach (KeyValuePair<string, string> entry in CSClasses)
            {
                // Key exists in both
                if (SSClasses.ContainsKey(entry.Key))
                {
                    // differences exist
                    if (SSClasses[entry.Key] != entry.Value)
                    {
                        Difference nd = new Difference(entry.Key, entry.Value, SSClasses[entry.Key]);
                        this.m_Differences.Add(nd);
                    }
                    // remove from snapshots classes
                    SSClasses.Remove(entry.Key);
                }
                else
                {
                    // This is a new class
                    Difference nc = new Difference(entry.Key, entry.Value, null);
                    this.m_Differences.Add(nc);
                }
            }

            // Remaining snapshot classes must be deletions
            foreach (KeyValuePair<string, string> entry in SSClasses)
                this.m_Differences.Add(new Difference(entry.Key, null, entry.Value));
        }

        private Dictionary<string, string> CSSFindClasses(StreamReader stream)
        {
            Dictionary<string, string> rval = new Dictionary<string, string>();

            // Search until we hit a valid piece of data
            for (char c = (char)stream.Read(); !stream.EndOfStream; c = (char)stream.Read())
            {
                // hit data
                if (!IgnorableCharacters.Contains(c))
                {
                    // macro/keyframes
                    if (c == '@')
                    {
                        // read until beginning of macro/keyframes identifier
                        while (c != ' ')
                            c = (char)stream.Read();

                        string id = "" + (char)stream.Read();
                        c = (char)stream.Read();

                        // fill identifier
                        while(c != ' ' && c != ':' && c != '{')
                        { id += c; c = (char)stream.Read(); }

                        string value = "";

                        // Keyframes
                        if (c == '{')
                        {
                            int ClassDepth = 1;

                            // fill data
                            while (ClassDepth > 0)
                            {
                                c = (char)stream.Read();
                                if (c == '}')
                                { ClassDepth--; c = (char)stream.Read(); continue; }
                                if (c == '{')
                                { ClassDepth++; c = (char)stream.Read(); continue; }

                                value += c;
                            }
                        }
                        else // Macro
                        {
                            c = (char)stream.Read();
                        
                            // read until next valid char
                            while (c == ' ' || c == ':')
                            c = (char)stream.Read();

                            value += c;
                            c = (char)stream.Read();

                            // fill value
                            while (c != ';')
                            { value += c; c = (char)stream.Read(); }
                        }

                        // add the value
                        rval.Add(id, value);
                    }
                    else // class or s.th. else
                    {
                        string cid = "" + c;
                        c = (char)stream.Read();

                        // fill class(es)
                        while(c != '{')
                        { cid += c; c = (char)stream.Read(); }

                        cid.TrimEnd(IgnorableCharacters.ToCharArray()); // remove trailing whitespace
                        int ClassDepth = 1;
                        string Data = "";

                        // fill data
                        while(ClassDepth > 0)
                        {
                            c = (char)stream.Read();
                            if (c == '}')
                            { ClassDepth--; c = (char)stream.Read(); continue; }
                            if(c == '{')
                            { ClassDepth++; c = (char)stream.Read(); continue; }

                            Data += c;
                        }

                        rval.Add(cid, Data);
                    }
                }
            }

            return rval;
        }
        #endregion

        #region Properties
        internal static string IgnorableCharacters = "\x9\xA\xB\xC\xD\x20";
        #endregion
    }
}
