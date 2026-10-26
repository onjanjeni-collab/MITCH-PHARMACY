using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Revmax_Interface_Promun.Services;

namespace Revmax_Interface_Promun.Processing
{
    public static class ProductMatcher
    {
        private static readonly char[] Separators = { ' ', ',', '.', '-', '_', '/', '\\', '(', ')', '[', ']' };

        private static string NormalizeText(string text)
        {
            return string.Join(" ", Tokenize(text ?? string.Empty));
        }

        private static List<string> Tokenize(string text)
        {
            return text
                .ToLowerInvariant()
                .Split(Separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(NormalizeToken)
                .Where(token => !string.IsNullOrWhiteSpace(token))
                .ToList();
        }

        private static string NormalizeToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return string.Empty;

            token = token.Trim();

            if (token.Length > 4 && token.EndsWith("ies", StringComparison.Ordinal))
                return token.Substring(0, token.Length - 3) + "y";

            if (token.Length > 3 && token.EndsWith("es", StringComparison.Ordinal) &&
                (token.EndsWith("ses", StringComparison.Ordinal) || token.EndsWith("xes", StringComparison.Ordinal) ||
                 token.EndsWith("zes", StringComparison.Ordinal) || token.EndsWith("ches", StringComparison.Ordinal) ||
                 token.EndsWith("shes", StringComparison.Ordinal)))
                return token.Substring(0, token.Length - 2);

            if (token.Length > 3 && token.EndsWith("s", StringComparison.Ordinal) && !token.EndsWith("ss", StringComparison.Ordinal))
                return token.Substring(0, token.Length - 1);

            return token;
        }

        public static double CosineSimilarity(string text1, string text2)
        {
            if (string.IsNullOrWhiteSpace(text1) || string.IsNullOrWhiteSpace(text2))
                return 0.0;

            var words1 = Tokenize(text1);
            var words2 = Tokenize(text2);

            var vocab = new HashSet<string>(words1.Concat(words2));

            double dot = 0, mag1 = 0, mag2 = 0;
            foreach (var word in vocab)
            {
                int c1 = words1.Count(w => w == word);
                int c2 = words2.Count(w => w == word);
                dot += c1 * c2;
                mag1 += c1 * c1;
                mag2 += c2 * c2;
            }

            if (mag1 == 0 || mag2 == 0)
                return 0.0;

            return dot / (Math.Sqrt(mag1) * Math.Sqrt(mag2));
        }

        private static double TokenOverlapSimilarity(string text1, string text2)
        {
            var words1 = new HashSet<string>(Tokenize(text1));
            var words2 = new HashSet<string>(Tokenize(text2));

            if (words1.Count == 0 || words2.Count == 0)
                return 0.0;

            int intersection = words1.Intersect(words2).Count();
            int maxCount = Math.Max(words1.Count, words2.Count);
            return maxCount == 0 ? 0.0 : (double)intersection / maxCount;
        }

        private static double CombinedSimilarity(string text1, string text2)
        {
            string normalized1 = NormalizeText(text1);
            string normalized2 = NormalizeText(text2);

            if (string.IsNullOrWhiteSpace(normalized1) || string.IsNullOrWhiteSpace(normalized2))
                return 0.0;

            if (string.Equals(normalized1, normalized2, StringComparison.Ordinal))
                return 1.0;

            double cosine = CosineSimilarity(normalized1, normalized2);
            double overlap = TokenOverlapSimilarity(normalized1, normalized2);

            return Math.Max(cosine, overlap);
        }

        public static TerminalSiteProduct FindBestMatchingProduct(
            string productDescription,
            List<TerminalSiteProduct> products,
            double threshold = -1)
        {
            if (products == null || products.Count == 0 || string.IsNullOrWhiteSpace(productDescription))
                return null;

            if (threshold < 0)
            {
                string cfgThreshold = ConfigurationManager.AppSettings.Get("ProductMatchThreshold");
                if (!double.TryParse(cfgThreshold, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out threshold))
                    threshold = 0.90;
            }

            double bestScore = 0;
            TerminalSiteProduct bestMatch = null;

            foreach (var product in products)
            {
                string candidate = product.Description ?? string.Empty;
                double score = CombinedSimilarity(productDescription, candidate);
                if (score > bestScore)
                {
                    bestScore = score;
                    bestMatch = product;
                }
            }

            if (bestScore >= threshold && bestMatch != null)
                return bestMatch;

            return null;
        }
    }
}
