using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using System.Diagnostics;
using System.Linq;

namespace dotConverter
{
    class DotCreator
    {
        
        private List<string[]> directoryFiles;
        private static cppParser cppParser;
        public DotCreator(){
            directoryFiles = new List<string[]>();
            cppParser = new cppParser();
        }
        public void createClassDiagrammFromDirectory(string path){
            parseDirectoryCode(path);
            string dotDiagramm = createDotDiagrammString();
            path = path+$@"\prototyp.dot";
            generateDotFileWithPathAndDiagramm(path,dotDiagramm);
            generatePNGFromDotFilePath(path);
        }
        private void parseDirectoryCode(string path){
            string[] files = Directory.GetFiles(path);
            foreach (string filePath in files)
            {
                directoryFiles.Add(System.IO.File.ReadAllLines(filePath));
            }
        } 
        
        private string createDotDiagrammString(){
            string dotDiagramm = System.IO.File.ReadAllText(@"DotClassDiagrammHead.txt");
            var dotClasses = createDotClassesFromDirectory();
            var connections = getClassConnectionsFromClassList(dotClasses);                     
            foreach( var dotClass in dotClasses){
              dotDiagramm += dotClass.ToString();
            }
            dotDiagramm+="\n\n";
            foreach( var pair in connections){
              dotDiagramm += "\t"+pair.Item1+" -> "+pair.Item2+"\n";
            }
            dotDiagramm += "\n\t}\n}\n";
            return dotDiagramm;
        }

         private List<dotClassContainer> createDotClassesFromDirectory(){
            List<dotClassContainer> directoryClassList = new List<dotClassContainer>();
            foreach( string[] fileStrings in directoryFiles){
                var fileClasses = getDotClassesFromStringArray(fileStrings);
                directoryClassList.AddRange(fileClasses);
            }
            return directoryClassList;
        }

        private List<dotClassContainer> getDotClassesFromStringArray(string[] array){
            List<dotClassContainer> classList = new List<dotClassContainer>();   
            int currentClassIndex = -1;
            foreach( var line in array){      
                string className = cppParser.getClassNameFromLine(line);
                string attributeName = cppParser.getUMLAttributeFromLine(line);
                string functionName = cppParser.getFunctionNameFormLine(line);

                if(className != null){
                    currentClassIndex++;
                    dotClassContainer classConatiner = new dotClassContainer(className);
                    classList.Add(classConatiner);
                }
                else if (attributeName != null){
                    classList[currentClassIndex].attributeNames.Add(attributeName);
                }
                else if(functionName != null){
                    classList[currentClassIndex].functionNames.Add(functionName);
                }
            }
            return classList;
        }

        private List<Tuple<string,string>> getClassConnectionsFromClassList(List<dotClassContainer> classList){
            var connections =new List<Tuple<string,string>>();
            for(int i=0;i<classList.Count;i++){
                for(int j=i+1;j<classList.Count;j++){
                    foreach(string umlAttributeName in classList[j].attributeNames){
                        string className = classList[i].name;
                        string attributeName = umlAttributeName.Split(":").Last();
                        attributeName = attributeName.Replace("<u>","").Replace("</u>","").Trim();
                        if(className.Equals(attributeName)){
                            connections.Add(new Tuple<string, string>(classList[i].name,classList[j].name));
                        }
                    }
                }
            }
            return connections;
        }
        private void generateDotFileWithPathAndDiagramm(string path,string diagramm){
            using (System.IO.FileStream fs = System.IO.File.Create(path))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(diagramm); 
                fs.Write(bytes);
            }
        }
        

       

        private struct dotClassContainer{
            public dotClassContainer(string className){
                name = className;
                attributeNames = new List<string>();
                functionNames = new List<string>();
            }
            public string name { get;set; }
            public List<string> attributeNames { get; set;}
            public List<string> functionNames { get; set;}
            public override string ToString(){
                string attributeBody="";
                string functionBody="";
                foreach(var attribute in attributeNames){
                    attributeBody+=$"{attribute}<br align=\"left\"/>\n\t\t";
                }
                 foreach(var attribute in functionNames){
                    functionBody+=$"{attribute}<br align=\"left\"/>\n\t\t";
                }

                return  $" \n\t{name} [\n\t\t"+
               $"label =  <<table border=\"0\" cellspacing=\"0\" cellborder=\"1\">\n\t\t"+
              $@"<tr> <td>{name}</td> </tr>"+"\n\t\t"+
              $"<tr><td>{attributeBody}</td></tr>"+
              $"<tr><td>{functionBody}</td></tr>"+
              "</table>>\n\t]";
            } 
        } 
        private void generatePNGFromDotFilePath(string filePath){
            var dotDiagrammFilePath =new filePathMetaData(filePath);
            string argText = $@"-Tpng {dotDiagrammFilePath} -o {dotDiagrammFilePath.ToStringWithoutType()}.png";
            startDotProcessWithArg(argText);
        }

        private struct filePathMetaData{
            public filePathMetaData(string newFilePath){
                fileName = newFilePath.Split($@"\").Last();
                path = newFilePath.Remove(newFilePath.Length-fileName.Length);
                fileType =  fileName.Split('.').Last();
                fileName= fileName.Split('.').First();
            }
            public string path { get; }
            public string fileName { get; }
            public string fileType { get; }
            public override string ToString() => $@"{path}{fileName}.{fileType}";
            public string ToStringWithoutType() =>  $@"{path}{fileName}";
        } 

        private void startDotProcessWithArg(string arg){
            var proc = new Process 
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "dot.exe",
                    Arguments =  arg,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            proc.Start();
        }
      
    }
}