using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfSplitter;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace PdfSplitterTests
{
    public class PdfSplitterTests
    {
        private readonly PdfSplitter.PdfSplitter _pdfSplitter;

        public PdfSplitterTests()
        {
            string inputFilePath = Resources.PdfTestPath;
            string outputDirectory = Resources.OutputDirectory;
            _pdfSplitter = new PdfSplitter.PdfSplitter(inputFilePath, outputDirectory);
        }

        [Fact]
        public void Get_page_with_keyword()
        {
            string keyword = "Adobe PDF Library customers";

            List<int> pagesWithKeyword = _pdfSplitter.GetPagesWithKeyword(keyword);

            Assert.Contains(2, pagesWithKeyword);
            Assert.Contains(5, pagesWithKeyword);
            Assert.Contains(6, pagesWithKeyword);
            Assert.DoesNotContain(1, pagesWithKeyword);
            Assert.DoesNotContain(3, pagesWithKeyword);
            Assert.DoesNotContain(4, pagesWithKeyword);
        }

        //[Fact]
        //public void Split_by_keyword()
        //{
        //    string keyword = "Adobe PDF Library customers";

        //    try
        //    {
        //        List<string> pathsToCreatedPdfs = _pdfSplitter.SplitByKeyword(keyword);
        //    }
        //    finally
        //    {
        //        // cleanup
        //    }
        //}

        [Theory]
        [InlineData(2, 5, 6)]
        public void Split_by_pages(params int[] lastPageNumbers)
        {
            List<string> pathsToCreatedPdfs = _pdfSplitter.SplitByPages(lastPageNumbers);

            for (int i = 0; i < lastPageNumbers.Length; i++)
            {
                int startPage = (i == 0) ? 1 : lastPageNumbers[i];
                int lastPage = lastPageNumbers[i + 1];
                AssertPdfsAreEqual(_pdfSplitter.InputFilePath, startPage, lastPage, pathsToCreatedPdfs[i]);
            }
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 5)]
        public void Create_copy_for_range(int startPage, int endPage)
        {
            string pathToCreatedPdf = _pdfSplitter.CreateCopyForRange(startPage, endPage);

            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, startPage, endPage, pathToCreatedPdf);

            DeleteCreatedDirectory();
        }

        private void DeleteCreatedDirectory()
        {
            Directory.Delete(_pdfSplitter.OutputDirectory, recursive: true);
        }

        private void AssertPdfsAreEqual(string srcPdf, int srcStartPage, int srcEndPage, string destPdf)
        {
            string pdf1Text = GetPdfText(srcPdf, srcStartPage, srcEndPage);
            string pdf2Text = GetPdfText(destPdf);
            Assert.Equal(pdf1Text, pdf2Text);
        }

        private string GetPdfText(string path, int startPage = 1, int? endPage = null)
        {
            StringBuilder pdfText = new StringBuilder();

            using (PdfReader pdfReader = new PdfReader(path))
            {
                if (endPage == null)
                {
                    endPage = pdfReader.NumberOfPages;
                }

                for (int page = startPage; page <= endPage; page++)
                {
                    pdfText.Append(PdfTextExtractor.GetTextFromPage(pdfReader, page));
                }
            }

            return pdfText.ToString();
        }
    }
}