using System;
using System.Collections.Generic;

using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Accord.Math.Distances;
using AppAntiPlagiat.Models;


    public class PlagiarismDetection
    {
        private ApplicationDbContext context;
        public PlagiarismDetection(ApplicationDbContext context) 
        {
            this.context = context;
        }
        public double PlagiatDedeux(byte[] pdf1, byte[] pdf2)
        {
            
            string text1 = ExtractTextFromPdf(pdf1);
            string text2 = ExtractTextFromPdf(pdf2);

            // Convert the text to feature vectors
            double[] vector1 = ConvertToFeatureVector(text1);
            double[] vector2 = ConvertToFeatureVector(text2);

        // Calculate the similarity between the feature vectors using Cosine distance
            double similarity;
            
            if (vector1.Length < vector2.Length) {
                similarity = new Cosine().Distance(vector1, vector2);
            }
            else
            {
                similarity = new Cosine().Distance(vector2, vector1);
            }
            if (similarity < 0)
            {
                return 1;
            }
    
            return similarity;
        }
        
        public List<rapportEtPourcentage> rapportEtPourcentages(byte[] pdf) 
        {
            var ListeRapports = context.Rapports.Where(x => x.data != pdf).ToList();
            List<rapportEtPourcentage> liste = new List<rapportEtPourcentage>();
            if(ListeRapports != null)
            {
                foreach(var r in ListeRapports)
                {
                    rapportEtPourcentage model = new rapportEtPourcentage()
                    {
                        IdRapport = r.Id,
                        Pplagiat = PlagiatDedeux(pdf,r.data)
                    };
                    liste.Add(model);
                }

            }
            return liste;
        }
        public double Plagiat(byte[] pdf)
        {
            var ListeRapports = context.Rapports.Where(x => x.data != pdf).ToList();
            if (ListeRapports != null)
            {
                var similarities = new List<double>();

                foreach (Rapport rapport in ListeRapports)
                { 
                    similarities.Add(PlagiatDedeux(pdf, rapport.data));
                }

                double similarity = 0;
                foreach (double s in similarities)
                {
                    similarity += s;
                }

                return (similarity / similarities.Count());
            }
            return 0;
        }

        public string ExtractTextFromPdf(byte[] pdf)
        {
            PdfReader reader = new PdfReader(pdf);
            string text = PdfTextExtractor.GetTextFromPage(reader, 1);
            reader.Close();
            return text;
        }

        public double[] ConvertToFeatureVector(string text)
        {
            // Split the text into individual words and count their frequencies
            string[] words = text.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int[] frequencies = words.GroupBy(x => x).Select(x => x.Count()).ToArray();

            // Normalize the frequencies to obtain a feature vector
            double[] vector = frequencies.Select(x => (double)x / words.Length).ToArray();

            return vector;
        }
        
        public double PlagiatAutoExeption(int rappID, byte[] pdf)
        {
            var ListeRapports = context.Rapports.Where(x => x.data != pdf && x.Id != rappID).ToList();
            if (ListeRapports != null)
            {
                var similarities = new List<double>();

                foreach (Rapport rapport in ListeRapports)
                {
                    similarities.Add(PlagiatDedeux(pdf, rapport.data));
                }

                double similarity = 0;
                foreach (double s in similarities)
                {
                    similarity += s;
                }

                return (similarity / similarities.Count());
            }
            return 0;
    }
        
    }
    public class rapportEtPourcentage
    {
        public int IdRapport { get; set; }
        public double Pplagiat { set; get; }
    }

