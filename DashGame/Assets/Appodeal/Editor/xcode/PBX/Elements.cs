<<<<<<< HEAD
using System;
using System.Collections;
using System.Collections.Generic;

namespace Unity.Appodeal.Xcode.PBX {

    class PBXElement {
        protected PBXElement () { }

        // convenience methods
        public string AsString () { return ((PBXElementString) this).value; }
        public PBXElementArray AsArray () { return (PBXElementArray) this; }
        public PBXElementDict AsDict () { return (PBXElementDict) this; }

        public PBXElement this [string key] {
            get { return AsDict () [key]; }
            set { AsDict () [key] = value; }
        }
    }

    class PBXElementString : PBXElement {
        public PBXElementString (string v) { value = v; }

        public string value;
    }

    class PBXElementDict : PBXElement {
        public PBXElementDict () : base () { }

        private Dictionary<string, PBXElement> m_PrivateValue = new Dictionary<string, PBXElement> ();
        public IDictionary<string, PBXElement> values { get { return m_PrivateValue; } }

        new public PBXElement this [string key] {
            get {
                if (values.ContainsKey (key))
=======
using System.Collections.Generic;
using System.Collections;
using System;

namespace Unity.Appodeal.Xcode.PBX
{
    
    class PBXElement
    {
        protected PBXElement() {}
        
        // convenience methods
        public string AsString() { return ((PBXElementString)this).value; }
        public PBXElementArray AsArray() { return (PBXElementArray)this; }
        public PBXElementDict AsDict()   { return (PBXElementDict)this; }
        
        public PBXElement this[string key]
        {
            get { return AsDict()[key]; }
            set { AsDict()[key] = value; }
        }
    }
    
    class PBXElementString : PBXElement
    {
        public PBXElementString(string v) { value = v; }
        
        public string value;
    }

    class PBXElementDict : PBXElement
    {
        public PBXElementDict() : base() {}
        
        private Dictionary<string, PBXElement> m_PrivateValue = new Dictionary<string, PBXElement>();
        public IDictionary<string, PBXElement> values { get { return m_PrivateValue; }}
        
        new public PBXElement this[string key]
        {
            get {
                if (values.ContainsKey(key))
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
                    return values[key];
                return null;
            }
            set { this.values[key] = value; }
        }
<<<<<<< HEAD

        public bool Contains (string key) {
            return values.ContainsKey (key);
        }

        public void Remove (string key) {
            values.Remove (key);
        }

        public void SetString (string key, string val) {
            values[key] = new PBXElementString (val);
        }

        public PBXElementArray CreateArray (string key) {
            var v = new PBXElementArray ();
            values[key] = v;
            return v;
        }

        public PBXElementDict CreateDict (string key) {
            var v = new PBXElementDict ();
=======
        
        public bool Contains(string key)
        {
            return values.ContainsKey(key);
        }
        
        public void Remove(string key)
        {
            values.Remove(key);
        }

        public void SetString(string key, string val)
        {
            values[key] = new PBXElementString(val);
        }
        
        public PBXElementArray CreateArray(string key)
        {
            var v = new PBXElementArray();
            values[key] = v;
            return v;
        }
        
        public PBXElementDict CreateDict(string key)
        {
            var v = new PBXElementDict();
>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
            values[key] = v;
            return v;
        }
    }
<<<<<<< HEAD

    class PBXElementArray : PBXElement {
        public PBXElementArray () : base () { }
        public List<PBXElement> values = new List<PBXElement> ();

        // convenience methods
        public void AddString (string val) {
            values.Add (new PBXElementString (val));
        }

        public PBXElementArray AddArray () {
            var v = new PBXElementArray ();
            values.Add (v);
            return v;
        }

        public PBXElementDict AddDict () {
            var v = new PBXElementDict ();
            values.Add (v);
            return v;
        }
    }
}
=======
    
    class PBXElementArray : PBXElement
    {
        public PBXElementArray() : base() {}
        public List<PBXElement> values = new List<PBXElement>();
        
        // convenience methods
        public void AddString(string val)
        {
            values.Add(new PBXElementString(val));
        }
        
        public PBXElementArray AddArray()
        {
            var v = new PBXElementArray();
            values.Add(v);
            return v;
        }
        
        public PBXElementDict AddDict()
        {
            var v = new PBXElementDict();
            values.Add(v);
            return v;
        }
    }

} // namespace UnityEditor.iOS.Xcode

>>>>>>> 1aec2fb31523c49eca080618f52a5c2e6c3139fa
