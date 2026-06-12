
using ApartmentAgencyApp.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;

namespace ApartmentAgencyApp.Test
{
    public class PictParser
    {
        private static readonly string PictResultPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "PictResults.txt");

        public static IEnumerable<TestCaseData> GetTestCases()
        {
            string[] lines = File.ReadAllLines(PictResultPath);

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split('\t');

                if (parts.Length < 5)
                {
                    continue;
                }

                double distanceFromBeach = double.Parse(parts[0].Trim());
                int percentOfPositiveReviews = int.Parse(parts[1].Trim());

                ApartmentType apartmentType = (ApartmentType)Enum.Parse(
                    typeof(ApartmentType),
                    parts[2].Trim());

                bool renovatedInTheLastYear = bool.Parse(parts[3].Trim());

                ApartmentRank expectedRank = (ApartmentRank)Enum.Parse(
                    typeof(ApartmentRank),
                    parts[4].Trim());

                yield return new TestCaseData(
                    distanceFromBeach,
                    percentOfPositiveReviews,
                    apartmentType,
                    renovatedInTheLastYear,
                    expectedRank)
                    .SetName(
                        $"CalculateApartmentRank_Distance={distanceFromBeach}_" +
                        $"Reviews={percentOfPositiveReviews}_" +
                        $"Type={apartmentType}_" +
                        $"Renovated={renovatedInTheLastYear}_" +
                        $"Expected={expectedRank}");
            }
        }
    }
}