using System;
using System.Collections.Generic;
using System.Text;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using System.IO;
using Syncfusion.Drawing;

namespace DAMS_Proiect
{
    class PDFClass
    {
        public void CreatePdfFile(Project currentProject,string path)
        { 
            PdfDocument document = new PdfDocument();
            int nrOfPages = (int)(currentProject.tasksList.Count / 7) + 1;
            PdfGraphics graphics;
            PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, 11);

            PdfPage[] listOfPages = new PdfPage[nrOfPages];
            for (int  k = 1;k<=nrOfPages; k++)
            {
                listOfPages[k-1] = document.Pages.Add();
                graphics = listOfPages[k - 1].Graphics;
                graphics.DrawString(CreateProjectStatusInfo(currentProject,k), font, PdfBrushes.Black, new PointF(0, 0));
            }

            List<char> fileNamePath = new List<char>();
            List<char> pdfFileName = new List<char>();
            string finalName = "";
            char c;
            int counter = 0;
            foreach (char ch in path)
                fileNamePath.Add(ch);

            fileNamePath.Reverse();
            while(true)
            {
                c = fileNamePath[counter];
                if (c.Equals('\\')) break;
                counter++;
                pdfFileName.Add(c);
            }

            pdfFileName.Reverse();           
            foreach (char ch in pdfFileName) finalName += ch;

            using (var stream = new MemoryStream())
            {
                document.Save(stream);               
                File.WriteAllBytes(path, stream.ToArray());
                stream.Close();
            }
            document.Close(true);
        }


        public string CreateProjectStatusInfo(Project currentProject,int segment)
        {
            StringBuilder sb = new StringBuilder();
            Project newProject = new Project();
            for (int i = segment * 7 - 7; i < segment * 7; i++)
                if(i< currentProject.tasksList.Count)
                    newProject.tasksList.Add(currentProject.tasksList[i]);

            foreach (Task t in newProject.tasksList)
            {
                 string s = string.Format("\nTask ID={0} Information:\n\tName: {1}\n\tStart: {2}\n\tFinish: {3}\n\tDuration: {4}\n\tComplete: {5}%\n\tPriority: {6}",
                    t.TaskId, t.Name, t.Start, t.Finish, t.Duration, t.Complete, t.Priority);
/*
                if (t.ResourceNames.Count > 0)
                {
                    Console.WriteLine("\tResource Names:");
                    foreach (string resource in t.ResourceNames)
                    {
                        s += string.Format("\t\t{0}", resource);
                    }
                }
*/
                 s += "\n";
                sb.Append(s);
            }
            return sb.ToString() ;
        }
    }
};
