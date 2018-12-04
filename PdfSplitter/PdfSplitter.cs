using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections.Generic;
using System.IO;
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

        public List<string> SplitByKeyword(string keyword)
        {
            throw new NotImplementedException();
            //List<int> pagesWithKeyWord = GetPagesWithKeyword(keyword);
            //SplitPdfsByPages(pagesWithKeyWord);
        }

        public List<string> SplitByPages(params int[] lastPageNumbers)
        {
            return null;
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

        public string CreateCopyForRange(int startPage, int endPage)
        {
            string inputFileName = Path.GetFileNameWithoutExtension(InputFilePath);
            string outputFileName = $"{inputFileName}_pg_{startPage}_thru_{endPage}.pdf";
            string outputFilePath = Path.Combine(OutputDirectory, outputFileName);
            Directory.CreateDirectory(OutputDirectory);

            using (PdfReader pdfReader = new PdfReader(InputFilePath))
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