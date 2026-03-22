namespace Account.Domain.Validations
{
    public static class DocumentValidator
    {
        public static bool IsValid(string document)
        {
            if (string.IsNullOrEmpty(document)) { return false; }

            // Remove non-numeric characters.
            document = new string(document.Where(char.IsDigit).ToArray());

            if (document.Length != 11) { return false; }

            // Remove documents with all numbers the same.
            if (new string(document[0], 11) == document) { return false; }

            int[] multiplier1 = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            string tempDoc = document.Substring(0, 9);
            int sumValue = 0;

            for (int i = 0; i < 9; i++)
            {
                sumValue += int.Parse(tempDoc[i].ToString()) * multiplier1[i];
            }

            int rest = sumValue % 11;
            rest = rest < 2 ? 0 : 11 - rest;

            string typeChecker = rest.ToString();
            tempDoc = tempDoc + typeChecker;

            
            int[] multiplier2 = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            sumValue = 0;

            for (int i = 0; i < 10; i++)
            {
                sumValue += int.Parse(tempDoc[i].ToString()) * multiplier2[i];
            }

            rest = sumValue % 11;
            rest = rest < 2 ? 0 : 11 - rest;

            typeChecker = typeChecker + rest.ToString();

            return document.EndsWith(typeChecker);
        }
    }
}
