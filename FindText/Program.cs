using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace FindText
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("请输入查找关键字,按回车进行查找");

            string strFolder = CombinePath(System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "TestExcel");
            string extension = "txt";
            bool searchOption = true;

            string findText = Console.ReadLine();//202007001	装备升星石
            string[] files = GetFileInfo(strFolder, extension, searchOption);

            Dictionary<string, List<string>> findlie = new Dictionary<string, List<string>>();
            foreach (string fileName in files)
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read);
                //中文乱码加上System.Text.Encoding.Default,或则 System.Text.Encoding.GetEncoding("GB2312")
                StreamReader m_streamReader = new StreamReader(fs);
                //使用StreamReader类来读取文件
                m_streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
                // 从数据流中读取每一行，直到文件的最后一行，并在richTextBox1中显示出内容

                List<string> fd = new List<string>();
                string strLine = m_streamReader.ReadLine();
                int i = 1;
                while (strLine != null)
                {
                    MatchCollection results = Regex.Matches(strLine, findText);
                    int m = results.Count;
                    if(m>0)
                    {
                        fd.Add(string.Format("第{0}行，找到{1}个关键字\n", i, m));
                    }
                    strLine = m_streamReader.ReadLine();
                    i++;
                }
                if (fd.Count > 0)
                    findlie.Add(fileName, fd);
            }

            try
            {

                if(findlie.Keys.Count>0)
                {
                    string savePath = string.Format("查找结果 {0}", string.Format("{0:yyyy年MM月dd日 HH时mm分ss秒}.txt", DateTime.Now));
                    StreamWriter writer = new StreamWriter(savePath, false, new UTF8Encoding(false));
                    StringBuilder LogContent = new StringBuilder();
                    LogContent.Append(string.Format("在所有配置表中，找到含有关键字    {0}    的配置表共有{1}个，如下：\n", findText, findlie.Count));
                    foreach (KeyValuePair<string,List<string>> kvp in findlie)
                    {
                        LogContent.Append(string.Format("\n在文件：{0}  中找到有关键字的行数为{1}，文件路径为：{2}：\n", Path.GetFileNameWithoutExtension(kvp.Key), kvp.Value.Count, kvp.Key));
                        foreach (string s in kvp.Value)
                        {
                            LogContent.Append(s);
                        }
                    }
                    writer.WriteLine(LogContent.ToString().Replace("\n", System.Environment.NewLine));
                    writer.Flush();
                    writer.Close();
                    Console.WriteLine("全部查找结果已导出文本文件，存储在本工具所在目录下，文件名为：\n\"{0}\"", savePath);
                }
                else
                {
                    Console.WriteLine("查找完毕，未找到有关键字： {0}  的记录", findText);
                }
               // Log(string.Format("全部错误信息已导出文本文件，文件名为\"{0}\"，存储在本工具所在目录下", fileName));

            }
            catch
            {
                Console.WriteLine("未知错误");
            }
            Console.WriteLine("按任意键退出");
            Console.ReadLine();
        }
        /// <summary>
        /// 根据自定路径搜索文件
        /// </summary>
        /// <param name="strFolder">路径</param>
        /// <param name="extension">要搜索的文件后缀</param>
        /// <param name="serchOption">是否搜索子目录</param>
        /// <returns></returns>
        static string[] GetFileInfo(string strFolder, string extension, bool serchOption)
        {
            SearchOption searchOption2 = SearchOption.TopDirectoryOnly;
            if (serchOption)
                searchOption2 = SearchOption.AllDirectories;

            return Directory.GetFiles(strFolder, "*." + extension, searchOption2);
        }
        /// <summary>
        /// 合并两个路径字符串，与.Net类库中的Path.Combine不同，本函数不会因为path2以目录分隔符开头就认为是绝对路径，然后直接返回path2
        /// </summary>
       static string CombinePath(string path1, string path2)
        {
            path1 = path1.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            path2 = path2.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
            if (path2.StartsWith(Path.DirectorySeparatorChar.ToString()))
                path2 = path2.Substring(1, path2.Length - 1);

            return Path.Combine(path1, path2);
        }
    }
}
