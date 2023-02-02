using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Global
{
    public class Settings
    {
        Dictionary<string, string> dictionary { get; set; }

        public Settings()
        {
            dictionary = new Dictionary<string, string>();
        }
        public void AddSetting(string key, string value)
        {
            dictionary.Add(key, value);
        }

        public void ChangeSetting(string key, string value)
        {
            string read = dictionary[key];
            if (read == null)
                AddSetting(key, value);
            dictionary[key] = value;
        }

        public void RemoveSetting(string key)
        {
            dictionary.Remove(key);
        }
    }
}
