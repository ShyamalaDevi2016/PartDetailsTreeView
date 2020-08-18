using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartDetails.Models
{
    /// <summary>
    /// Class to represent the parts hierarchy
    /// </summary>
    
    public class Parts
    {

            public Parts()
            {
               SubParts = new ObservableCollection<Parts>();
            }

            public int id { get; set; }
            public int parent_id { get; set; }
      
            public string Name { get; set; }
            public string Description { get; set; }
            public string RootDescription { get; set; }
            public double Qty { get; set; }
            public ObservableCollection<Parts> SubParts { get; set; }
        

    }
}
