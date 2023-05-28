using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Kernels;
using ExcelDataReader;

/* Bu algoritmayı kullanabilmek için, Accord.NET - Accord.Math - Accord.Statistics.Tools -
 * Accord.Statistics.Kernels - ExcelDataReader - System.Data kütüphanelerini projenize eklemelisiniz.
 * Üst kısımdan "Proje" sekmesi içerisinde "Nuget Paketlerini Yönet..." kısmından bu kütüphaneleri ekleyebilirsiniz.    
*/

namespace OPTICS_Algorithm
{
    class Program
    {
        static void Main(string[] args)
        {
            // Excel dosyasını okuma
            DataSet result;
            using (var stream = File.Open("Mall_Customers.xlsx", FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    result = reader.AsDataSet();
                }
            }

            // Verileri alma
            var table = result.Tables[0];
            var data = new List<double[]>();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                var row = table.Rows[i];
                var values = new double[3]
                {
                    Convert.ToDouble(row["Age"]),
                    Convert.ToDouble(row["Annual Income"]),
                    Convert.ToDouble(row["Spending Score (1-100)"])
                };
                data.Add(values);
            }

            // Verileri ölçeklendirme
            double[][] scaledData = Accord.Statistics.Tools.ZScores(data.ToArray());

            // OPTICS modelini oluşturma
            var optics = new Optics<double>(minPts: 5, xi: 0.05);

            // Verilere uygulama
            var clusterIDs = optics.Compute(scaledData);

            // Sonuçları yazdırma
            for (int i = 0; i < clusterIDs.Length; i++)
            {
                Console.WriteLine($"Veri noktası {i + 1}: Küme ID: {clusterIDs[i]}");
            }

            Console.ReadLine();
        }
    }
}
