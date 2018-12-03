using iTextSharp.testutils;
using PdfSplitter;
using System;
using System.Collections.Generic;
using System.IO;
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

        //[Fact]
        //public void Split_by_pages()
        //{
        //    List<int> pagesToSplit = new List<int>() { 2, 5, 6 };

        //    try
        //    {
        //        List<string> pathsToCreatedPdfs = _pdfSplitter.SplitByPages(pagesToSplit);
        //    }
        //    finally
        //    {
        //        // cleanup
        //    }
        //}

        [Theory]
        [InlineData(1, 2)]
        [InlineData(3, 5)]
        public void Create_copy_for_range(int startPage, int endPage)
        {
            string pathToCreatedPdf = _pdfSplitter.CreateCopyForRange(startPage, endPage);

            //try
            //{
            using (FileStream _ = File.OpenRead(pathToCreatedPdf))
            {

            }

            CompareTool compareTool = new CompareTool();
            string comparison =
                compareTool.Compare(
                    pathToCreatedPdf,
                    _pdfSplitter.InputFilePath,
                    Resources.OutputDirectory,
                    "diff");
            Assert.Null(comparison);
            //AssertPdfCreatedForPages(pathToCreatedPdf, startPage, endPage);
            //}
            //finally
            //{
            //    Cleanup(pathToCreatedPdf);
            //}
        }

        private void AssertPdfCreatedForPages(string pathToCreatedPdf, int startPage, int endPage)
        {
            throw new NotImplementedException();
        }
    }
}