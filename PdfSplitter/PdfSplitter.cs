using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Path = System.IO.Path;

namespace PdfSplitter
{
    public class PdfSplitter
    {
        public readonly string InputFilePath;
        public readonly string OutputDirectory;

        public PdfSplitter(string inputFilePath, string outputPath)
        {
            InputFilePath = inputFilePath;
            OutputDirectory = outputPath;
        }

        public List<string> SplitOnKeyword(string keyword)
        {
            List<int> pagesWithKeyWord = GetPagesWithKeyword(keyword);
            return SplitOnPages(pagesWithKeyWord.ToArray());
        }

        public List<string> SplitOnPages(params int[] pageNumbers)
        {
            if (pageNumbers[0] == 1)
            {
                pageNumbers = pageNumbers.Skip(1).ToArray();
            }

            List<string> pathsToCreatedFiles = new List<string>();

            for (int i = 0; i < pageNumbers.Length + 1; i++)
            {
                int startPage = (i == 0) ? 1 : pageNumbers[i - 1];
                int? endPage = (i < pageNumbers.Length) ? (int?)pageNumbers[i] - 1 : null;
                pathsToCreatedFiles.Add(CreateCopyForRange(startPage, endPage));
            }

            return pathsToCreatedFiles;
        }

        public List<int> GetPagesWithKeyword(string keyword)
        {
            List<int> pagesWithKeyword = new List<int>();

            using (PdfReader pdfReader = new PdfReader(InputFilePath))
            {
                for (int page = 1; page <= pdfReader.NumberOfPages; page++)
                {
                    string pageText = PdfTextExtractor.GetTextFromPage(pdfReader, page);
                    if (pageText.Contains(keyword))
                    {
                        pagesWithKeyword.Add(page);
                    }
                }
            }

            return pagesWithKeyword;
        }

        public string CreateCopyForRange(int startPage, int? endPage)
        {
            if (endPage != null && startPage > endPage)
            {
                throw new ArgumentException("Start page cannot be greater than the end page.");
            }

            Directory.CreateDirectory(OutputDirectory);

            using (PdfReader pdfReader = new PdfReader(InputFilePath))
            {
                if (endPage == null)
                {
                    endPage = pdfReader.NumberOfPages;
                }

                string inputFileName = Path.GetFileNameWithoutExtension(InputFilePath);
                string outputFileName = $"{inputFileName}_pg_{startPage}_thru_{endPage}.pdf";
                string outputFilePath = Path.Combine(OutputDirectory, outputFileName);

                using (Document sourceDocument = new Document(pdfReader.GetPageSizeWithRotation(startPage)))
                using (FileStream fileStream = new FileStream(outputFilePath, FileMode.Create))
                using (PdfCopy pdfCopy = new PdfCopy(sourceDocument, fileStream))
                {
                    sourceDocument.Open();
                    for (int page = startPage; page <= endPage; page++)
                    {
                        PdfImportedPage pdfImportedPage = pdfCopy.GetImportedPage(pdfReader, page);
                        pdfCopy.AddPage(pdfImportedPage);
                    }
                    sourceDocument.Close();
                }

                return outputFilePath;
            }
        }
    }
}