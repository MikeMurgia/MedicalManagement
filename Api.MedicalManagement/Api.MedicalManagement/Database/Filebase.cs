using Library.MedicalManagement.Models;
using Newtonsoft.Json;

namespace Api.MedicalManagement.Database
{
    public class Filebase
    {
        private string _root;
        private string _patRoot;
        private static Filebase _instance;


        public static Filebase Current
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Filebase();
                }

                return _instance;
            }
        }

        private Filebase()
        {
            _root = @"C:\Users\mikem\source\data";
            _patRoot = $"{_root}\\Patients";
        }

        public int LastPatKey
        {
            get
            {
                if (Pats.Any())
                {
                    return Pats.Select(x => x.Id).Max();
                }
                return 0;
            }
        }

        public Patient AddOrUpdate(Patient pat)
        { 
            if (pat.Id <= 0)
            {
                pat.Id = LastPatKey + 1;
            }

            
            string path = $"{_patRoot}\\{pat.Id}.json";


            
            if (File.Exists(path))
            {
                //blow it up
                File.Delete(path);
            }

            
            File.WriteAllText(path, JsonConvert.SerializeObject(pat));

            return pat;
        }

        public List<Patient> Pats
        {
            get
            {
                var root = new DirectoryInfo(_patRoot);
                var _pats = new List<Patient>();
                foreach (var patientFile in root.GetFiles())
                {
                    var patient = JsonConvert
                        .DeserializeObject<Patient>
                        (File.ReadAllText(patientFile.FullName));
                    if (patient != null)
                    {
                        _pats.Add(patient);
                    }

                }
                return _pats;
            }
        }


        public bool Delete(string type, string id)
        {
            //TODO: refer to AddOrUpdate for an idea of how you can implement this.
            return true;
        }
    }



}