using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotAutomatedClassCreator
{
     class dotClassContainer
    {
        public dotClassContainer(string className)
        {
            name = className;
            attributeNames = new List<string>();
            functionNames = new List<string>();
        }
        public string name { get; set; }
        public List<string> attributeNames { get; set; }
        public List<string> functionNames { get; set; }
        public override string ToString()
        {
            string attributeBody = string.Join("\n\t\t", attributeNames.ToArray());
            string functionBody = string.Join("\n\t\t", functionNames.ToArray());
            if(attributeBody.Length == 0)
            {
                attributeBody = "   ";
            }

            return $" \n\t{name} [\n\t\t" +
           $"label =  <<table border=\"0\" cellspacing=\"0\" cellborder=\"1\">\n\t\t" +
          $@"<tr> <td>" + "\t\t" + $"{name}" + "\t\t" + @"</td> </tr>" + "\n\t\t" +
          $"<tr><td>\n\t\t{attributeBody}" +
          "</td></tr><tr><td>\n" +
          "\n\t\t" + functionBody + "</td></tr>" +
          "</table>>\n\t]\n";
        }
        public dotClassContainer format()
        {
            List<string> [] containerLists =  {attributeNames , functionNames };
            for(int i=0;i< containerLists.Length;i++)
            {
                formatGivenBodyList(ref containerLists[i]);
            }
            attributeNames = containerLists[0];
            functionNames = containerLists[1];

            return this;
        }

        private void formatGivenBodyList(ref List<string> list)
        {
            list = createFormatedClassBlockForStringListWithSeperator(list, ':');
            list = createHTMLTableEndingAndLineFeedVersionFromStringList(list);
            List<string> nameTails = list.Select(x => x.Split(':').Last()).ToList();
            list = list.Select(x => x.Split(':').First()).ToList();
            nameTails = createFormatedClassBlockForStringListWithSeperator(nameTails, '@');
            for (int j = 0; j < nameTails.Count; j++)
            {
                list[j] += ':' + nameTails[j];
                list[j] = list[j].Replace("@", "");
            }
        }

        private List<string> createFormatedClassBlockForStringListWithSeperator(List<string> list, char seperator)
        {
            int tabLength = 4;
            string result = "";
            if (list.Count > 0)
            {   
                int maxNameLength = list.Max(x => x.Split(seperator)[0].Trim().Length);
                int maxTypeLength = list.Max(x => x.Split(seperator)[1].Trim().Length);
                maxNameLength += 4 - (maxNameLength % tabLength);
                maxTypeLength += 4 - (maxTypeLength % tabLength);
                for (int i = 0; i < list.Count; i++)
                {
                    var itemParts = list[i].Split(seperator);
                    var functionNameOffset = getTabOffsetForStringWithMaxLength(itemParts[0].Trim(), maxNameLength);
                    var functionTypeOffset = getTabOffsetForStringWithMaxLength(itemParts[1].Trim(), maxTypeLength);
                    list[i] = $"{itemParts[0].Trim()}{functionNameOffset}{seperator}{itemParts[1].Trim()}{functionTypeOffset}";
                }
            }
            return list;
        }


        private List<string> createHTMLTableEndingAndLineFeedVersionFromStringList(List<string> list)
        {
            List<string> newEndingVersionList = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                newEndingVersionList.Add($"{ list[i]}<br align=\"left\"/>\n\t\t");
            }
            return newEndingVersionList;
        }

        private string getTabOffsetForStringWithMaxLength(string selection, int maxLength)
        {
            int tabLength = 4;
            bool isOffsetTabNeeded = selection.Length % tabLength != 0;
            int tabCount = (int)Math.Floor((double)(maxLength - selection.Length) / tabLength);
            tabCount += isOffsetTabNeeded ? 1 : 0;
            return new String('\t', tabCount);
        }
    }



}
