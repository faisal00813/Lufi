using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lufi
{
    class LuceneManager
    {
        public Logger logger { get { return Logger.Instance; } }
        Lucene.Net.Store.Directory directory;
        Analyzer analyzer;

        private IndexWriter _writer;

        public IndexWriter Writer
        {
            get
            {
                if (_writer == null )
                {
                    _writer =new IndexWriter(IndexDirectory, IndexAnalyzer, new IndexWriter.MaxFieldLength(4));
                }

                return _writer;
            }

        }


        public Analyzer IndexAnalyzer
        {
            get { return analyzer; }

        }

        public Lucene.Net.Store.Directory IndexDirectory
        {
            get
            {
                return directory;
            }
        }

        private string _dirPath;

        public string DirPath
        {
            get { return _dirPath; }

        }

        public LuceneManager(string DirectoryPath)
        {
            _dirPath = DirectoryPath;
            directory = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(DirPath));
            analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_29);
        }

        public void OpenDirectory()
        {
            directory = Lucene.Net.Store.FSDirectory.Open(new System.IO.DirectoryInfo(DirPath));
        }
        public void CloseDirectory()
        {
            directory.Close();
        }

        public bool CloseWriter()
        {
            try
            {
                if (Writer != null)
                {
                    Writer.Optimize();
                    Writer.Flush(false, true, true);
                    Writer.Close();
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                logger.WriteToLog(ex.Message);
                return false;
            }
        }

        public bool FushAndOptimize() {

            try
            {
                if (Writer != null)
                {
                    Writer.Optimize();
                    Writer.Flush(false, true, true);
                    
                    return true;
                }
                return false;

            }
            catch (Exception ex)
            {
                logger.WriteToLog(ex.Message);
                return false;
            }
        }
        public bool InsertDocument(FileFolder file)
        {
            try
            {
                if (file != null)
                {
                    Document doc = new Document();
                    doc.Add(new Field("FileName", file.Name, Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("FilePath", file.FilePath, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("FileType", Enum.GetName(typeof(FileFolder.FileType), file.Type), Field.Store.YES, Field.Index.NOT_ANALYZED));
                    Writer.AddDocument(doc);
                    //Console.WriteLine(string.Format("File {0} added to index", file.FilePath));
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                logger.WriteToLog(ex.Message);
                return false;
            }


        }
        public bool UpdateDocument(string Value, FileFolder file)
        {
            try
            {
                if (file != null)
                {
                    Document doc = new Document();
                    doc.Add(new Field("FileName", file.Name, Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("FilePath", file.FilePath, Field.Store.YES, Field.Index.NOT_ANALYZED));
                    doc.Add(new Field("FileType", Enum.GetName(typeof(FileFolder.FileType), file.Type), Field.Store.YES, Field.Index.NOT_ANALYZED));


                    Writer.UpdateDocument(new Term("FilePath", Value), doc);
                    //Console.WriteLine(string.Format("File {0} update to index", file.FilePath));
                    return true;
                }

                return false;

            }
            catch (Exception ex)
            {
                logger.WriteToLog(ex.Message);
                return false;
            }

        }
        public bool DeleteDocument(FileFolder file)
        {
            try
            {
                if (file!=null)
                {
                    Writer.DeleteDocuments(new Term("FilePath", file.FilePath));
                    //Console.WriteLine(string.Format("File {0} deleted from index", file.FilePath));
                    return true;

                }
                return false;
            }
            catch (Exception ex)
            {
                logger.WriteToLog(ex.Message);
                return false;
            }

        }
    }
}
