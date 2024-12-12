using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;


/*
Prompt to generate:

當然，以下是您的需求總結成條列式prompts：

1. **指定兩個.xml檔案進行分析**：
   - `all_5.xml`
   - `all_5.xml.old`

2. **找出AdapterId的變化**：
   - 比較兩個.xml檔案中的`AdapterId`節點，找出`LowPart`的變化（例如：`LowPart`由104256變成99511，`LowPart`由108444變成106226）。

3. **輸出AdapterId變化**：
   - 在分析後，輸出`AdapterId`變化的內容到控制台。

4. **將變化應用到指定的.xml檔案上**：
   - 指定的.xml檔案列表：
     - `center_2.xml`
     - `center_3.xml`
     - `only_1.xml`
     - `right_2.xml`
     - `right_3.xml`
     - `right_4.xml`
   - 修改這些.xml檔案中的`AdapterId`節點，根據分析出的變化進行更新。

5. **保留原始.xml檔案，並更名加上.old**：
   - 在修改前，將原始.xml檔案備份，並更名為`.old`。
*/

namespace AdapterIdChanged
{
    class Program
    {
        static void Main(string[] args)
        {
            string parentPath = "e:/Workspaces/Mastersign.DisplayManager/Mastersign.DisplayManager/bin/debug"; // 設定parent path
            string[] xmlFiles = { Path.Combine(parentPath, "all_5.xml"), Path.Combine(parentPath, "all_5.xml.old") };
            Dictionary<string, string> adapterIdChanges = AnalyzeAdapterIdChanges(xmlFiles[0], xmlFiles[1]);
            foreach (var change in adapterIdChanges)
            {
              Console.WriteLine($"Old LowPart: {change.Key}, New LowPart: {change.Value}");
            }

            string[] targetXmlFiles = new[] { "center_2.xml", "center_3.xml", "only_1.xml", "right_2.xml", "right_3.xml", "right_4.xml" }
                .Select(file => Path.Combine(parentPath, file))
                .ToArray();
            ApplyAdapterIdChanges(targetXmlFiles, adapterIdChanges);
        }

        static Dictionary<string, string> AnalyzeAdapterIdChanges(string newFile, string oldFile)
        {
            var newDoc = XDocument.Load(newFile);
            var oldDoc = XDocument.Load(oldFile);

            var newAdapterIds = newDoc.Descendants("AdapterId").Select(e => new
            {
                LowPart = e.Element("LowPart").Value,
                HighPart = e.Element("HighPart").Value
            }).ToList();

            var oldAdapterIds = oldDoc.Descendants("AdapterId").Select(e => new
            {
                LowPart = e.Element("LowPart").Value,
                HighPart = e.Element("HighPart").Value
            }).ToList();

            var changes = new Dictionary<string, string>();

            for (int i = 0; i < newAdapterIds.Count; i++)
            {
                if (newAdapterIds[i].LowPart != oldAdapterIds[i].LowPart)
                {
                    changes[oldAdapterIds[i].LowPart] = newAdapterIds[i].LowPart;
                }
            }

            return changes;
        }

        static void ApplyAdapterIdChanges(string[] targetFiles, Dictionary<string, string> changes)
        {
            foreach (var file in targetFiles)
            {
                var doc = XDocument.Load(file);
                var adapterIds = doc.Descendants("AdapterId");

                foreach (var adapterId in adapterIds)
                {
                    var lowPart = adapterId.Element("LowPart").Value;
                    if (changes.ContainsKey(lowPart))
                    {
                        adapterId.Element("LowPart").Value = changes[lowPart];
                    }
                }

                string backupFile = file + ".old";
                File.Copy(file, backupFile, true);
                doc.Save(file);
            }
        }
    }
}