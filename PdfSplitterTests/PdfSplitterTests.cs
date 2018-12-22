using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using PdfSplitter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace PdfSplitterTests
{
    public class PdfSplitterTests : IDisposable
    {
        private readonly PdfSplitter.PdfSplitter _pdfSplitter;

        public PdfSplitterTests()
        {
            string inputFilePath = Resources.PdfTestPath;
            string outputDirectory = Resources.OutputDirectory;
            _pdfSplitter = new PdfSplitter.PdfSplitter(inputFilePath, outputDirectory);
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(_pdfSplitter.OutputDirectory, recursive: true);
            }
            catch { } // if the test created a directory, delete it, but we don't care otherwise
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

        [Fact]
        public void Split_on_keyword()
        {
            string keyword = "Adobe PDF Library customers"; // appears on page 2, 5, and 6

            List<string> pathsToCreatedPdfs = _pdfSplitter.SplitOnKeyword(keyword);

            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 1, 1, pathsToCreatedPdfs[0]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 2, 4, pathsToCreatedPdfs[1]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 5, 5, pathsToCreatedPdfs[2]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 6, 6, pathsToCreatedPdfs[3]);
        }

        [Fact]
        public void Split_on_pages()
        {
            int[] pageNumbers = new int[] { 2, 3, 6 };
            List<string> pathsToCreatedPdfs = _pdfSplitter.SplitOnPages(pageNumbers);

            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 1, 1, pathsToCreatedPdfs[0]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 2, 2, pathsToCreatedPdfs[1]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 3, 5, pathsToCreatedPdfs[2]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 6, 6, pathsToCreatedPdfs[3]);
        }

        [Fact]
        public void Split_on_pages_with_first_page_specified()
        {
            int[] pageNumbers = new int[] { 1, 2, 3, 6 };
            List<string> pathsToCreatedPdfs = _pdfSplitter.SplitOnPages(pageNumbers);

            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 1, 1, pathsToCreatedPdfs[0]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 2, 2, pathsToCreatedPdfs[1]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 3, 5, pathsToCreatedPdfs[2]);
            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, 6, 6, pathsToCreatedPdfs[3]);
        }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 5)]
        public void Create_copy_for_range(int startPage, int endPage)
        {
            string pathToCreatedPdf = _pdfSplitter.CreateCopyForRange(startPage, endPage);

            AssertPdfsAreEqual(_pdfSplitter.InputFilePath, startPage, endPage, pathToCreatedPdf);
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