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
    
            return similarity;
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
        /*static List<string> FindMatchingPhrases(string text1, string text2)
        {
            // Split the text into sentences
            string[] sentences1 = text1.Split('.');
            string[] sentences2 = text2.Split('.');

            // Find the matching phrases between the two texts
            List<string> matches = new List<string>();
            foreach (string sentence1 in sentences1)
            {
                foreach (string sentence2 in sentences2)
                {
                    double similarity = new Cosine().Distance(ConvertToFeatureVector(sentence1), ConvertToFeatureVector(sentence2));
                    if (similarity > 0.9) // Threshold for similarity
                    {
                        matches.Add(sentence1.Trim() + " | " + sentence2.Trim());
                    }
                }
            }

            return matches;
        }*/
    }

