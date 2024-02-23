using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSP.Models
{
    public class ResultsModel
    {
        public string Message { get; set; }

        public List<AlgorithmResultModel> AlgorithmResults { get; set; }

        public ResultsModel()
        {
            Message = "";
            AlgorithmResults = new();
        }
        
        public int NumberOfResults => AlgorithmResults.Count;
    }
}
