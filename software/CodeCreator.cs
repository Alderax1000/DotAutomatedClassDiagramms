using System.Collections.Generic;
using System;
using System.Text;
using System.Linq;
namespace dotConverter
{
    class CodeCreator
    {


        public CodeCreator(string destPath, dotParser newParser)
        {
            classFileDestinationPath = destPath;
            parser = newParser;
        }

        public  string classFileDestinationPath;
        private dotParser parser;
        struct attribute
        {              public string protection;
            public string type;
            public string name;

            public string toString()
            {
                return $"{protection} {type} {name}";
            }
        }

        public void parseForClassesAndCreateFiles(string[] text)
        {
            if (parser.isFirstLineAGraphHeader(text[0]))
            {
                string[] modifiedText = parser.deleteLinesTillNextClass(text);
                List<string> newClass;
                while (modifiedText is not null)
                {
                    newClass = parser.parseLinesTillClassEnd(modifiedText);
                    createClassFileFromDotStringList(newClass);
                    modifiedText = modifiedText.Skip(1).ToArray();
                    modifiedText = parser.deleteLinesTillNextClass(modifiedText);
                }

            }

        }

        public void createClassFileFromDotStringList(List<string> dotStrings){
             var className = dotStrings[0].Trim().Substring(0, dotStrings[0].Trim().IndexOf(" "));
            string fileContent = String.Join(" ", createClass(dotStrings));
            using (System.IO.FileStream fs = System.IO.File.Create(classFileDestinationPath+$@"\{className}.cs"))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(fileContent); 
                fs.Write(bytes);
            }
        }

        public List<string> createClass(List<string> dotClass)
        {
            List<string> output = new List<string>();
            var className = dotClass[0].Trim().Substring(0, dotClass[0].Trim().IndexOf(" "));
            output.Add($"class {className}\n {{\n");
            for (int i = 1; i < dotClass.Count - 1; i++)
            {
                string targetLine = dotClass[i];
                if (targetLine.Contains("label"))
                {
                    var startIndex = targetLine.IndexOf(className) + className.Length + 1;
                    targetLine = targetLine.Substring(startIndex).Replace("}\"", "").Trim();
                    var classContent = splitAtributesAndFunctions(targetLine);
                    string[] attributes ;
                    for (int isFunctionContent = 0; isFunctionContent < 2; isFunctionContent++)
                    {
                        attributes = classContent[isFunctionContent].Split(@"\l");
                        foreach (var entry in attributes)
                        {
                            if (!entry.Equals(""))
                            {
                                if (isFunctionContent == 1)
                                {

                                    var attribute = convertDotStringToAtribute(entry);
                                    output.Add("\t" + attributeToFunctionString(attribute));
                                }
                                else
                                {
                                    var attribute = convertDotStringToAtribute(entry);
                                    output.Add("\t" + attribute.toString()+";\n");
                                }
                            }
                        }
                    }

                }
            }

            output.Add("}");
            return output;
        }

        private string attributeToFunctionString(attribute myAttribute)
        {
            string output = myAttribute.toString() + "\n\t{\n\n\t}\n";
            return output;
        }

        private string[] splitAtributesAndFunctions(string dotString)
        {
            return dotString.Split(@"|");
        }

        private attribute convertDotStringToAtribute(string dotString)
        {
            attribute myAttribute = new attribute();
            myAttribute.protection = dotString.Contains("+") ? "public" : "private";
            var trimedEntry = dotString.Replace(myAttribute.protection, "").Replace(":", " ").Trim();
            trimedEntry = trimedEntry.Remove(0, 1).Trim();
            myAttribute.name = trimedEntry.Substring(0, trimedEntry.IndexOf(" "));
            trimedEntry = trimedEntry.Replace(myAttribute.name, "").Trim();
            myAttribute.type = trimedEntry;
            return myAttribute;
        }
    }
}
